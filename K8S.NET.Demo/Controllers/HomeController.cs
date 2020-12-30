using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using K8s.NET.Demo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace K8s.NET.Demo.Controllers {
    public class HomeController : Controller {
        public IActionResult Index () {
            var hostname = Dns.GetHostName ();
            ViewBag.HostName = hostname;
            ViewBag.HostIp = Dns.GetHostAddresses (hostname).FirstOrDefault (ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return View ();
        }

        public IActionResult Privacy () {
            return View ();
        }

        public IActionResult CheckHealth () {
            if (new Random ().Next (100) > 50) {
                return Ok ("OK");
            } else {
                return BadRequest ();
            }
        }

        [ResponseCache (Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error () {
            return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}