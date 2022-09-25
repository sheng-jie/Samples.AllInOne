namespace Orleans.EventSourcing.GrainInterfaces;
public interface IBankAccountGrain : IGrainWithGuidKey
{
    /// <summary>
    /// 账号初始化
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    Task InitialAccount(decimal amount);
    /// <summary>
    /// 获取余额
    /// </summary>
    /// <returns></returns>
    Task<decimal> GetBalanceAsync();
    /// <summary>
    /// 支付
    /// </summary>
    /// <param name="amount"></param>
    Task<bool> PayAsync(PayRequest request);

    /// <summary>
    /// 转入
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    Task TransferIn(TransferRequest request);

    /// <summary>
    /// 转出
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<bool> TransferOut(TransferRequest request);
}
