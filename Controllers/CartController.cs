using System.Diagnostics;
using Holistica.Data;
using Holistica.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Holistica.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public CartController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public async Task<IActionResult> AddToCartAsync(Guid productId)
        {

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("GuestUserId")))
            {
                var guestUserId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("GuestUserId", guestUserId);

            }

            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");

            cart ??= new List<CartItem>();

            var existingCartItem = cart.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingCartItem != null)
            {
                // If the product is already in the cart, increase the quantity
                existingCartItem.Quantity++;
            }
            else
            {
                // If the product is not in the cart, add it
                var newCartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = 1
                };
                cart.Add(newCartItem);
            }

            // Save the updated cart back to the session
            HttpContext.Session.Set("Cart", cart);

            // Optionally, you can return a success message or redirect to the cart page
            return RedirectToAction("Index", "Cart"); // Redirect to the cart view
        }

    }

        public IActionResult Index()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new Product { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
