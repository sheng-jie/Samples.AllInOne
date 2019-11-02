using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Orleans.Grains
{
    
    /// <summary>
    /// 登录状态
    /// </summary>
    public class LoginState
    {
        public List<string> LoginUsers { get; set; } = new List<string>();

        public int Count => LoginUsers.Count;
    }

    /// <summary>
    /// 有状态的Grain
    /// </summary>
    public class SessionControlStateGrain : Grain<LoginState>, ISessionControlGrain
    {
        public async Task Login(string userId)
        {
            var appName = this.GetPrimaryKeyString();
            this.State.LoginUsers.Add(userId);
            await this.WriteStateAsync();

            Console.WriteLine($"Current active users count of {appName} is {this.State.Count}");
        }

        public Task Logout(string userId)
        {
            //获取当前Grain的身份标识
            var appName = this.GetPrimaryKey();
            this.State.LoginUsers.Remove(userId);
            this.WriteStateAsync();

            Console.WriteLine($"Current active users count of {appName} is {this.State.Count}");
            return Task.CompletedTask;
        }

        public Task<int> GetActiveUserCount()
        {
            return Task.FromResult(this.State.Count);
        }
    }
}