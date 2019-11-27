using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Grains
{
    public class SessionControlGrain : Grain, ISessionControlGrain
    {
        private List<string> LoginUsers { get; set; } = new List<string>();

        public Task Login(string userId)
        {
            //获取当前Grain的身份标识(因为ISessionControlGrain身份标识为string类型，GetPrimaryKeyString()); 
            var appName = this.GetPrimaryKeyString();

            LoginUsers.Add(userId);

            Console.WriteLine($"Current active users count of {appName} is {LoginUsers.Count}");
            return Task.CompletedTask;
        }

        public Task Logout(string userId)
        {
            //获取当前Grain的身份标识
            var appName = this.GetPrimaryKey();
            LoginUsers.Remove(userId);

            Console.WriteLine($"Current active users count of {appName} is {LoginUsers.Count}");
            return Task.CompletedTask;
        }

        public Task<int> GetActiveUserCount()
        {
            return Task.FromResult(LoginUsers.Count);
        }

        public Task MockLogout()
        {
            var streamProvider = this.GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<string>(Guid.NewGuid(), "Logout");

            //随机logout
            Random r = new Random();
            RegisterTimer(s =>
            {
                var index = r.Next(this.LoginUsers.Count - 1);
                var removeItem = this.LoginUsers[index];
                this.LoginUsers.Remove(removeItem);
                return stream.OnNextAsync(removeItem);
            }, null, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000));

            return Task.CompletedTask;
        }


    }


}
