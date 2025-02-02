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
}