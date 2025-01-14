using System.Diagnostics;
using Holistica.Data;
using Holistica.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Holistica.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public ActionResult Index()
        {
            var products = new List<Product>
            {
                new Product { ProductId = Guid.NewGuid(), Name = "Med1", ImageUrl = "1.webp", Price = 10.99m },
                new Product { ProductId = Guid.NewGuid(), Name = "Med2", ImageUrl = "2.webp", Price = 15.99m },
                new Product { ProductId = Guid.NewGuid(), Name = "Med3", ImageUrl = "3.webp", Price = 20.99m },
                new Product { ProductId = Guid.NewGuid(), Name = "Med3", ImageUrl = "3.webp", Price = 20.99m },
            };

            return PartialView(products);
        }


        public ActionResult Details(Guid id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return PartialView(product);
        }

        [Authorize]
        public ActionResult AddProduct()
        {
            return PartialView();
        }
    }
}
