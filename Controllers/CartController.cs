using System.Text;
using System.Text.Json;
using Holistica.Data;
using Holistica.Models;
using Holistica.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class CartController : Controller
{
    private const string CartSessionKey = "Cart";
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext dbContext)
    {
        _context = dbContext;
    }

    public IActionResult ViewCart()
    {
        var cartView = HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

        var cartViewModel = new CartItemViewModel()
        {
            CartItems = cartView,
            TotalPrice = cartView.Sum(item => item.Product.Price * item.Quantity)
        };

        return PartialView(cartViewModel);
    }

    public IActionResult AddToCart(Guid productId)
    {
        var productToAdd = _context.Products.Find(productId);

        var cartItems = HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

        var cartItem = cartItems.FirstOrDefault(ci => ci.Product.ProductId == productId);

        if (cartItem != null)
        {
            cartItem.Quantity++;
        }
        else
        {
            cartItems.Add(new CartItem
            {
                Product = productToAdd,
                Quantity = 1
            });
        }

        HttpContext.Session.Set(CartSessionKey, cartItems);

        return RedirectToAction("ViewCart");
    }

}