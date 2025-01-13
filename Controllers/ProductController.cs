using System.Diagnostics;
using Holistica.Data;
using Holistica.Models;
using Microsoft.AspNetCore.Mvc;

namespace Holistica.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public ProductController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        //public IActionResult Index()
        //{
        //    var products = new Product()
        //    {
        //        Description = "asddas",
        //        ImageUrl = "asd",
        //        Name = "Meow",
        //        Price= 2.12M,
        //        ProductId = Guid.NewGuid(),

        //    }

        //    return View(products);
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
