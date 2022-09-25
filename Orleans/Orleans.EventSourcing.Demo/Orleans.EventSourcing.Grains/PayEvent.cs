namespace Orleans.EventSourcing.Grains;

public class PayEvent : BankAccountEventBase
{
    public string BillNo { get; }

    public PayEvent(decimal amount, string billNo) : base(amount)
    {
        BillNo = billNo;
    }
}

public class InitialAccountEvent : BankAccountEventBase
{
    public Guid BankNo { get; }
    public InitialAccountEvent(Guid bankNo, decimal amount) : base(amount)
    {
        BankNo = bankNo;
    }
}