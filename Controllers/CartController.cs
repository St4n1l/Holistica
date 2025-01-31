using System.Text;
using System.Text.Json;
using Holistica.Data;
using Holistica.Models;
using Holistica.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class CartController : Controller
{
    private readonly CartService _cartService;
    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    public IActionResult ViewCart()
    {
        var items = _cartService.GetCartItems();

        var cartViewModel = new CartItemViewModel()
        {
            CartItems = items,
            TotalPrice = _cartService.GetTotalPrice()
        };

        return PartialView(cartViewModel);
    }

    public IActionResult AddToCart(Guid productId)
    {
        _cartService.AddToCart(productId);
        return RedirectToAction("ViewCart");
    }

}