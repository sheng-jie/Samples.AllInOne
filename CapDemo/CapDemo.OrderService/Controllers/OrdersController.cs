using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CapDemo.OrderService.Data;
using CapDemo.OrderService.Domains;
using DotNetCore.CAP;
using CapDemo.Shared;
using CapDemo.Shared.Models;

namespace CapDemo.OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly ICapPublisher _capPublisher;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(OrderDbContext context, ICapPublisher capPublisher,ILogger<OrdersController> logger)
        {
            _context = context;
            _capPublisher = capPublisher;
            _logger = logger;
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(string id)
        {
            var order = await _context.Order.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(CreateOrderDto orderDto)
        {
            var shoppingItems =
                orderDto.ShoppingCartItems.Select(item => new ShoppingCartItem(item.SkuId, item.Price, item.Qty));
            var order = new Order(orderDto.CustomerId).NewOrder(shoppingItems.ToArray());
            
            try
            {
                using (var trans = _context.Database.BeginTransaction(_capPublisher, autoCommit: false))
                {
                    _context.Order.Add(order);

                    var deduceDto = new DeduceInventoryDto()
                    {
                        OrderId = order.OrderId,
                        DeduceStockItems = order.OrderItems.Select(
                            item => new DeduceStockItem(item.SkuId, item.Qty, item.Price)).ToList()
                    };
                    await _capPublisher.PublishAsync(TopicConsts.DeduceInventoryCommand,deduceDto,
                        callbackName: TopicConsts.CancelOrderCommand);
                    await _context.SaveChangesAsync();
                    await trans.CommitAsync();
                }
                
                _logger.LogInformation($"Order [{order.OrderId}] created successfully!");
            }
            catch (DbUpdateException)
            {
                if (OrderExists(order.OrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(string id)
        {
            return _context.Order.Any(e => e.OrderId == id);
        }
    }
}