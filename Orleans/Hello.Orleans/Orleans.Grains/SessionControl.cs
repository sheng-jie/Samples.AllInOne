using System.Threading.Tasks;

namespace Orleans.Grains
{
    /// <summary>
    /// 会话控制，超过指定人数，无法参与秒杀
    /// </summary>
    public class SessionControl : Grain<SessionStatistics>, ISessionControl
    {
        /// <summary>
        /// 最大并发用户数
        /// </summary>
        private const int MaxConcurrencyNum = 50;

        public Task<bool> Login(string userId)
        {
            if (MaxConcurrencyNum > this.State.Count)
            {
                this.State.LoginUsers.Add(userId);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task Logout(string userId)
        {
            this.State.LoginUsers.Remove(userId);
            return Task.CompletedTask;
        }
    }
}