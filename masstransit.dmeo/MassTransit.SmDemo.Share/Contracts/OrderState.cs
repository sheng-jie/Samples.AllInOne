namespace MassTransit.SmDemo.Share.Contracts;

public enum OrderState
{
    Submitted,
    Paid,
    Shipped,
    Finished,
    Canceled
}