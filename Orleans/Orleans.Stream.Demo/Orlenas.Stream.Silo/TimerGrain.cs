using System;
using System.Threading.Tasks;
using Orleans.Stream.Grain;

namespace Orleans.Stream.Silo
{
    public class TimerGrain : Orleans.Grain, ITimerGrain
    {
        private int _value;

        public Task<int> GetValueAsync()
        {
            return Task.FromResult(_value);
        }

        public override Task OnActivateAsync()
        {
            RegisterTimer(_ =>
            {
                ++_value;
                return Task.CompletedTask;
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            return base.OnActivateAsync();
        }
    }
}