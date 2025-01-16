using System.Diagnostics;
using Holistica.Cookie;
using Holistica.Data;
using Holistica.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? CookieHelper.GetCartCookie(HttpContext);

            cart ??= new List<CartItem>();

            var existingCartItem = cart.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity++;
            }
            else
            {
                var newCartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = 1
                };

                cart.Add(newCartItem);
            }

            HttpContext.Session.Set("Cart", cart); 
            CookieHelper.SetCartCookie(HttpContext, cart);

            return RedirectToAction("Index", "Cart");
        }


        public IActionResult Index()
        {
            var cartItems = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();

            var cart = new Cart
            {
                Items = cartItems
            };

            return PartialView(cart);
        }
    }
}
