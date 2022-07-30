namespace MassTransit.SmDemo.Share.Contracts;

public interface IReserveStockRequest
{
    public Dictionary<string, uint> Items { get;}
}

public interface IReserveStockResponse
{
    public bool IsSucceed { get; set; }

    public string Message { get; set; }
}