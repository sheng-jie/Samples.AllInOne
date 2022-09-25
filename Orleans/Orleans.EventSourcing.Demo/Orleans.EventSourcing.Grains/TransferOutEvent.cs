namespace Orleans.EventSourcing.Grains;

public class TransferOutEvent : BankAccountEventBase
{
    public Guid ToBankNo { get; }

    public TransferOutEvent(decimal amount, Guid toBankNo) : base(amount)
    {
        ToBankNo = toBankNo;
    }
}