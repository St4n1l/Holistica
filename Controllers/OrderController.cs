using System.Diagnostics;
using Holistica.Data;
using Holistica.Models;
using Microsoft.AspNetCore.Mvc;

namespace Holistica.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public OrderController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new Product { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
