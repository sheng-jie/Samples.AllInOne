﻿namespace MassTransit.CourierDemo.OrderService.Domains;

public class OrderItem
{
    public OrderItem(string orderId, string skuId, decimal price, uint qty)
    {
        OrderId = orderId;
        SkuId = skuId;
        Price = price;
        Qty = qty;
    }

    public int Id { get; set; }
    public string OrderId { get; set; }
    public string SkuId { get; set; }
    public decimal Price { get; set; }
    public uint Qty { get; set; }
}