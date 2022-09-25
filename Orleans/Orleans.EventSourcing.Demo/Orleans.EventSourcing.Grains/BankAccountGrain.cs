using Orleans.EventSourcing.GrainInterfaces;
using Orleans.Providers;
using Orleans.Storage;

namespace Orleans.EventSourcing.Grains;

// [StorageProvider(ProviderName = "OrleansLocalStorage")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class BankAccountGrain : JournaledGrain<BankAccount, BankAccountEventBase>, IBankAccountGrain
{
    public async Task InitialAccount(decimal amount)
    {
        var bankNo = this.GetPrimaryKey();
        this.RaiseEvent(new InitialAccountEvent(bankNo, amount));
        await this.ConfirmEvents();
    }

    public Task<decimal> GetBalanceAsync()
    {
        return Task.FromResult(this.State.Balance);
    }

    public async Task<bool> PayAsync(PayRequest request)
    {
        if (State.Balance < request.Amount)
            return false;
        
        this.RaiseEvent(new PayEvent(request.Amount, request.BillNo));
        await this.ConfirmEvents();
        return true;
    }

    public async Task TransferIn(TransferRequest request)
    {
        this.RaiseEvent(new TransferInEvent(request.Amount, request.From));
        await this.ConfirmEvents();
    }

    public async Task<bool> TransferOut(TransferRequest request)
    {
        if (State.Balance < request.Amount)
            return false;

        this.RaiseEvent(new TransferOutEvent(request.Amount, request.To));
        await this.ConfirmEvents();
        
        var destAccountGrain = this.GrainFactory.GetGrain<IBankAccountGrain>(request.To);
        await destAccountGrain.TransferIn(request);
        return true;
    }
}