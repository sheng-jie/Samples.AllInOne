namespace Orleans.EventSourcing.Grains;

public class BankAccountEventBase
{
    public BankAccountEventBase(decimal amount)
    {
        Amount = amount;
        Date = DateTime.Now;
    }

    public decimal Amount { get; }
    public DateTime Date { get; }
}