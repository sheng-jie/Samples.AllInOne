namespace MassTransit.SmDemo.StockService;

public static class GoodStockStore
{
    private static Dictionary<string, uint> _goodStocks;

    static GoodStockStore()
    {
        _goodStocks = new Dictionary<string, uint>()
        {
            { "iphone 14", 10 },
            { "iphone 14 Charge", 5 },
            { "ipad air", 5 },
        };
    }

    /// <summary>
    /// 是否有库存
    /// </summary>
    /// <param name="goodId"></param>
    /// <param name="requireNum"></param>
    /// <returns></returns>
    public static bool HasStock(string goodId,uint requireNum)
    {
        if (_goodStocks.TryGetValue(goodId, out var remainStock))
        {
            return remainStock > requireNum;
        }

        return false;
    }

    /// <summary>
    /// 扣减库存
    /// </summary>
    /// <param name="goodId"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static bool DeductStock(string goodId, uint num)
    {
        if (HasStock(goodId, num))
        {
            _goodStocks[goodId] -= num;
            return true;
        }

        return false;
    }
    /// <summary>
    /// 返还库存
    /// </summary>
    /// <param name="goodId"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static void ReturnStock(string goodId, uint num)
    {
        _goodStocks[goodId] += num;
    }
}