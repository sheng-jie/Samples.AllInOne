using DotNetCore.CAP;
using System.Text.Json;

namespace DotNetCore.Cap.WebApiDemo.Consumers
{
    public class WeatherConsumer : ICapSubscribe
    {
        private readonly ILogger<WeatherConsumer> _logger;

        public WeatherConsumer(ILogger<WeatherConsumer> logger)
        {
            this._logger = logger;
        }

        [CapSubscribe("weather.forecast")]
        public WeatherForecast Display(WeatherForecast forecast)
        {
            _logger.LogWarning("display:{0}", JsonSerializer.Serialize(forecast));

            return forecast;
        }

        // [CapSubscribe("weather.forecast.done")]
        // public void Done(WeatherForecast forecast)
        // {
        //     _logger.LogWarning("done:{0}", JsonSerializer.Serialize(forecast));
        // }
    }
}