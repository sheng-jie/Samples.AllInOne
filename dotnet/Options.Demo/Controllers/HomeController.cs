using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Options.Demo.Models;

namespace Options.Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptions<DeveloperOptions> _developerOptions;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IOptions<DeveloperOptions> developerOptions,ILogger<HomeController> logger)
        {
            _developerOptions = developerOptions;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
