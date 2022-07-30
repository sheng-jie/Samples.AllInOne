namespace MassTransit.SmDemo.Share.Contracts;

public class OrderItem
{
    public string GoodId { get; set; }
    public uint Num { get; set; }
    public double Price { get; set; }

    public OrderItem(string goodId, uint num, double price)
    {
        GoodId = goodId;
        Num = num;
        Price = price;
    }
}