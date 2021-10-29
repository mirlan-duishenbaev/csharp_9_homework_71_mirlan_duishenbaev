using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyChat.Models;
using MyChat.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        UserService userService;

        public HomeController(ILogger<HomeController> logger, UserService service)
        {
            _logger = logger;
            userService = service;
        }

        public IActionResult Index()
        {
            return RedirectToAction("ChatRoom", "Messages");
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
