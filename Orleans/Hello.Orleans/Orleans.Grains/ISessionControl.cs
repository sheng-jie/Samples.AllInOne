using System.Threading.Tasks;

namespace Orleans.Grains
{
    /// <summary>
    /// 会话控制，超过指定人数，无法参与秒杀
    /// </summary>
    public interface ISessionControl : IGrainWithGuidKey
    {
        Task<bool> Login(string userId);
        Task Logout(string userId);
    }
}