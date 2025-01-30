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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product)
        {
           

            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Shop");
            }

            return View(product);
        }

        public async Task<IActionResult> AddQuantity(string productId)
        {
            var neededProduct = await _context.Products.FindAsync(Guid.Parse(productId));
            return PartialView(neededProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuantity(string productId, int quantity)
        {
            var neededProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == Guid.Parse(productId));

            if (neededProduct != null)
            {
                neededProduct.Quantity = quantity;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Shop");

            }

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string input)
        {
            List<Product> products;

            if (string.IsNullOrWhiteSpace(input))
            {
                products = await _context.Products.ToListAsync();
            }
            else
            {
                products = await _context.Products
                    .Where(p => p.Name.Contains(input))
                    .ToListAsync();
            }

            return PartialView("ProductListPartial", products);
        }
    }
    
}