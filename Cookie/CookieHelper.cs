using System.Text.Json;
using Holistica.Models;

namespace Holistica.Cookie;

public static class CookieHelper
{
    public static void SetCartCookie(HttpContext context, List<CartItem> cart)
    {
        var cartJson = JsonSerializer.Serialize(cart);
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(7),
            IsEssential = true // Mark the cookie as essential
        };

        context.Response.Cookies.Append("Cart", cartJson, cookieOptions);
    }

    public static List<CartItem> GetCartCookie(HttpContext context)
    {
        var cartJson = context.Request.Cookies["Cart"];
        if (string.IsNullOrEmpty(cartJson))
        {
            return new List<CartItem>();
        }
        return JsonSerializer.Deserialize<List<CartItem>>(cartJson);
    }

    public static void ClearCartCookie(HttpContext context)
    {
        context.Response.Cookies.Delete("Cart");
    }
}