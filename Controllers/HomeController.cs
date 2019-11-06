using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RSPatients.Models;

namespace RSPatients.Controllers
{
    public class HomeController : Controller
    {
        //Action to view home page
        public IActionResult Index()
        {
            return View();
        }

        //Action to view About page
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        //Action to view contact page
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        //Action to view privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        //Action to view error page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
