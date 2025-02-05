using System.Diagnostics;
using System.Globalization;
using Holistica.Data;
using Holistica.Models;
using Holistica.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Holistica.Controllers
{
    public class ShopController : Controller
    {
        public readonly ShopService _shopService;
        public ShopController(ShopService service)
        {
            _shopService = service;
        }

        public async Task<ActionResult> IndexAsync()
        {
            var products = await _shopService.GetProducts();
            return PartialView(products);
        }

        [HttpPost]
        [Route("Shop/Details/")]
        public async Task<ActionResult> Details(string productId)
        {
            var product = await _shopService.GetSpecificProduct(Guid.Parse(productId));

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
            if(ModelState.IsValid)
            {
                await _shopService.AddProduct(product);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ChangeQuantity(string productId)
        {
            var neededProduct = await _shopService.GetSpecificProduct(Guid.Parse(productId));
            return PartialView(neededProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeQuantity(string productId, int quantity)
        {
            await _shopService.ChangeQuantity(productId, quantity);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string input)
        {
            var products = await _shopService.Search(input);

            return PartialView("ProductListPartial", products);
        }

        public async Task<IActionResult> Remove(string productId)
        {
            await _shopService.Remove(productId);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Change(string productId)
        {
            var product = await _shopService.GetSpecificProduct(Guid.Parse(productId));
            return PartialView(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(string productId, string description, string imageurl, string name, string price, int quantity)
        {
            price = price.Replace(",", ".");

            if (!decimal.TryParse(price, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedPrice))
            {
                return BadRequest("Invalid price format.");
            }

            var product = await _shopService.ChangeProduct(productId, name, description, parsedPrice, quantity, imageurl);
            return RedirectToAction("Index");
        }
    }
    
}