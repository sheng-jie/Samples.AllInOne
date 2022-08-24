using MassTransit;

namespace Masstransit.FirstActivity.Activities
{
    public class FirstActivityActivity :
        IActivity<FirstActivityArguments, FirstActivityLog>
    {
        private readonly ILogger<FirstActivityActivity> _logger;

        public FirstActivityActivity(ILogger<FirstActivityActivity> logger)
        {
            _logger = logger;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<FirstActivityArguments> context)
        {
            await Task.Delay(100);

            _logger.LogWarning($"Execute activity:{context.Arguments.Value}");


            if (DateTime.Now.Millisecond % 2 == 1)
            {
                return context.Completed<FirstActivityLog>(new
                {
                    Value = context.Arguments.Value
                });
            }

            return context.Faulted();
        }

        public async Task<CompensationResult> Compensate(CompensateContext<FirstActivityLog> context)
        {
            await Task.Delay(100);

            return context.Compensated();
        }
    }
}