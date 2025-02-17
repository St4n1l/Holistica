using System.Security.Principal;
using Holistica.Models.ViewModels;
using Holistica.Services;
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

    public async Task<IActionResult> AddToCart(Guid productId)
    {
        await _cartService.AddToCartAsync(productId);
        return RedirectToAction("ViewCart");
    }

    public async Task<IActionResult> AddToCartThroughDetails(Guid productId)
    {
        await _cartService.AddToCartAsync(productId);
        return RedirectToAction("Index", "Shop");
    }

    public IActionResult ChangeQuantity(string productId, int adjustment)
    {
        _cartService.ChangeQuantity(Guid.Parse(productId), adjustment);
        return RedirectToAction("ViewCart");
    }

    public IActionResult Remove(string productId)
    {
        _cartService.Remove(Guid.Parse(productId));
        return RedirectToAction("ViewCart");
    }
}