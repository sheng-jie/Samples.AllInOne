using System.Threading.Tasks;

namespace Orleans.Grains
{
    /// <summary>
    /// 会话控制
    /// </summary>
    public interface ISessionControlGrain : IGrainWithStringKey
    {
        Task Login(string userId);
        Task Logout(string userId);
        Task<int> GetActiveUserCount();
        Task MockLogout();
    }
}