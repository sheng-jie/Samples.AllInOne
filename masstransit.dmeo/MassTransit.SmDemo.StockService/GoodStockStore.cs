namespace MassTransit.SmDemo.StockService;

public static class GoodStockStore
{
    private static Dictionary<string, uint> _goodStocks;
    private static object lockObj = new object();

    static GoodStockStore()
    {
        _goodStocks = new Dictionary<string, uint>()
        {
            { "iphone 14", 10 },
            { "iphone 14 charge", 5 },
            { "ipad air", 5 },
        };
    }

    public static Dictionary<string, uint> GetStocks()
    {
        return _goodStocks;
    }

    /// <summary>
    /// 是否有库存
    /// </summary>
    /// <param name="goodId"></param>
    /// <param name="requireNum"></param>
    /// <returns></returns>
    public static bool HasStock(string goodId, uint requireNum)
    {
        if (_goodStocks.TryGetValue(goodId, out var remainStock))
        {
            return remainStock > requireNum;
        }

        return false;
    }

    /// <summary>
    /// 预留库存
    /// </summary>
    /// <param name="reserveItems"></param>
    /// <returns></returns>
    public static bool ReserveStock(Dictionary<string, uint> reserveItems)
    {
        lock (lockObj)
        {
            if (reserveItems.All(item => HasStock(item.Key, item.Value)))
            {
                foreach (var reserveItem in reserveItems)
                {
                    _goodStocks[reserveItem.Key] -= reserveItem.Value;
                }

                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// 返还库存
    /// </summary>
    /// <param name="returnItems"></param>
    /// <returns></returns>
    public static void ReturnStock(Dictionary<string, uint> returnItems)
    {
        foreach (var returnItem in returnItems)
        {
            _goodStocks[returnItem.Key] += returnItem.Value;
        }
    }
}