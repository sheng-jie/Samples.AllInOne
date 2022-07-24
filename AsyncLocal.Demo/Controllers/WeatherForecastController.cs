using AsyncLocal.Demo.Tenant;
using Microsoft.AspNetCore.Mvc;

namespace AsyncLocal.Demo.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ICurrentTenant _currentTenant;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,ICurrentTenant currentTenant)
    {
        _logger = logger;
        _currentTenant = currentTenant;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public  IEnumerable<WeatherForecast> Get()
    {
        _logger.LogWarning($"before:{_currentTenant.TenantCode}");

        using (_currentTenant.Use(null))
        {
            _logger.LogWarning($"use1: {_currentTenant?.TenantCode}");

            DoSomething();
        }
        
        _logger.LogWarning($"use1 after: {_currentTenant?.TenantCode}");
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    private void DoSomething()
    {
        using (_currentTenant.Use("234"))
        {
            _logger.LogWarning($"use2: {_currentTenant.TenantCode}");
        }
        _logger.LogWarning($"use2 after: {_currentTenant.TenantCode}");
    }
}
