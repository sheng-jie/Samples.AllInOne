namespace Orleans.EventSourcing.Grains;

public class TransferInEvent : BankAccountEventBase
{
    public Guid FromBankNo { get; }

    public TransferInEvent(decimal amount, Guid fromBankNo) : base(amount)
    {
        FromBankNo = fromBankNo;
    }
}