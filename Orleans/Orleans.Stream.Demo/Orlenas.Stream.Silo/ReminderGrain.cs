using System;
using System.Threading.Tasks;
using Orleans.Runtime;
using Orleans.Stream.Grain;

namespace Orleans.Stream.Silo
{
    public class ReminderGrain : Orleans.Grain, IReminderGrain, IRemindable
    {
        private int _value;

        public override async Task OnActivateAsync()
        {
            await RegisterOrUpdateReminder(nameof(IncrementAsync), TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
            await base.OnActivateAsync();
        }

        public Task<int> GetValueAsync() => Task.FromResult(_value);

        private Task IncrementAsync()
        {
            _value += 1;
            return Task.CompletedTask;
        }

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            switch (reminderName)
            {
                case nameof(IncrementAsync):
                    await IncrementAsync();
                    break;

                default:
                    await UnregisterReminder(await GetReminder(reminderName));
                    break;
            }
        }
    }
}