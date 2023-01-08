using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetCore.Cap.WebApiDemo;
using DotNetCore.Cap.WebApiDemo.Data;
using DotNetCore.CAP;
using System.Text.Json;

namespace DotNetCore.Cap.WebApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherForecastsController : ControllerBase
    {
        private readonly WeatherDbContext _context;
        private readonly ICapPublisher _capPublisher;

        public WeatherForecastsController(WeatherDbContext context,ICapPublisher capPublisher)
        {
            _context = context;
            this._capPublisher = capPublisher;
        }

        // GET: api/WeatherForecasts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> GetWeatherForecast()
        {
            return await _context.WeatherForecast.ToListAsync();
        }

        // GET: api/WeatherForecasts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WeatherForecast>> GetWeatherForecast(int id)
        {
            var weatherForecast = await _context.WeatherForecast.FindAsync(id);

            if (weatherForecast == null)
            {
                return NotFound();
            }

            return weatherForecast;
        }

        // PUT: api/WeatherForecasts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWeatherForecast(int id, WeatherForecast weatherForecast)
        {
            if (id != weatherForecast.Id)
            {
                return BadRequest();
            }

            _context.Entry(weatherForecast).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeatherForecastExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/WeatherForecasts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<WeatherForecast>> PostWeatherForecast(WeatherForecast weatherForecast)
        {
            using (var trans = _context.Database.BeginTransaction(_capPublisher, autoCommit: false))
            {
                _context.WeatherForecast.Add(weatherForecast);
                await _context.SaveChangesAsync();
                await _capPublisher.PublishAsync("weather.forecast", weatherForecast, callbackName: "weather.forecast.done");
                await trans.CommitAsync();
            }

            return CreatedAtAction("GetWeatherForecast", new { id = weatherForecast.Id }, weatherForecast);
        }

        // DELETE: api/WeatherForecasts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeatherForecast(int id)
        {
            var weatherForecast = await _context.WeatherForecast.FindAsync(id);
            if (weatherForecast == null)
            {
                return NotFound();
            }

            _context.WeatherForecast.Remove(weatherForecast);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WeatherForecastExists(int id)
        {
            return _context.WeatherForecast.Any(e => e.Id == id);
        }
    }
}
