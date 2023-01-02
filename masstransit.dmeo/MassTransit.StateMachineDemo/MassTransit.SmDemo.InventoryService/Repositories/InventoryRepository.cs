using System.Collections.Concurrent;

namespace MassTransit.SmDemo.InventoryService.Repositories;

public static class InventoryRepository
{
    private static readonly ConcurrentDictionary<string, uint> _inventories;

    static InventoryRepository()
    {
        _inventories = new ConcurrentDictionary<string, uint>();
        _inventories.TryAdd("iphone-13-128g-black", 10);
        _inventories.TryAdd("iphone-13-256g-red", 10);
    }

    public static uint GetStock(string skuId)
    {
        if (_inventories.TryGetValue(skuId, out var current)) return current;
        return 0;
    }

    public static bool TryDeduceStock(string skuId, uint qty)
    {
        if (_inventories.TryGetValue(skuId, out uint current))
        {
            if (current >= qty)
            {
                return _inventories.TryUpdate(skuId, current - qty, current);
            }
        }

        return false;
    }

    public static bool ReturnStock(string skuId, uint qty)
    {
        if (_inventories.TryGetValue(skuId, out uint current))
        {
            return _inventories.TryUpdate(skuId, current + qty, current);
        }

        return false;
    }
}