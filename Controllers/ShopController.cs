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

        public async Task<ActionResult> IndexAsync()
        {
            var products = await _context.Products.ToListAsync();
            return PartialView(products);
        }


        public async Task<ActionResult> Details(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
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

        public ActionResult AddQuantity()
        {
            return PartialView();
        }
    }
}