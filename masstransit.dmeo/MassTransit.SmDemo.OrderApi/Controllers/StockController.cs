using MassTransit.SmDemo.Share.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit.SmDemo.OrderApi.Controllers;

[ApiController]
[Route("[controller]")]
public class StockController : ControllerBase
{
    private readonly IRequestClient<ICheckStockRequest> _checkStockRequestClient;
    private readonly ILogger<StockController> _logger;

    public StockController(IRequestClient<ICheckStockRequest> checkStockRequestClient, ILogger<StockController> logger)
    {
        _checkStockRequestClient = checkStockRequestClient;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var stocks= await _checkStockRequestClient.GetResponse<ICheckStockResponse>(new {});

        return Ok(stocks.Message.Stocks);
    }
}