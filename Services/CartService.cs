//using Holistica.Data;
//using Holistica.Models;

//namespace Holistica.Services;

//public class CartService
//{
//    private const string CartSessionKey = "Cart";
//    private readonly ApplicationDbContext _context;
//    private readonly IHttpContextAccessor _httpContextAccessor;

//    public CartService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
//    {
//        _context = dbContext;
//        _httpContextAccessor = httpContextAccessor;
//    }

//    private ISession Session => _httpContextAccessor.HttpContext.Session;
//    public List<CartItem> GetCartItems()
//    {
//        var cart = Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

//        return cart;
//    }

//    public decimal GetTotalPrice()
//    {
//        var cart = Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
//        var price = cart.Sum(ci => ci.Product.Price * ci.Quantity);

//        return price;
//    }

//    public async Task AddToCartAsync(Guid productId)
//    {
//        var productToAdd = await _context.Products.FindAsync(productId);

//        if (productToAdd == null)
//        {
//            return;
//        }

//        var cartItems = GetCartItems();

//        var cartItem = cartItems.FirstOrDefault(ci => ci.Product.ProductId == productId);

//        if (cartItem != null)
//        {
//            cartItem.Quantity++;
//        }
//        else
//        {
//            cartItems.Add(new CartItem
//            {
//                Product = productToAdd,
//                Quantity = 1
//            });
//        }

//        Session.Set(CartSessionKey, cartItems);
//    }
//}



using Holistica.Data;
using Holistica.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Holistica.Services;

public class CartService
{
    private const string CartSessionKey = "Cart";
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _context = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession Session => _httpContextAccessor.HttpContext.Session;

    public List<CartItem> GetCartItems()
    {
        var cart = Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

        return cart;
    }

    public decimal GetTotalPrice()
    {
        var cart = GetCartItems();
        return cart.Sum(ci => ci.Product?.Price * ci.Quantity ?? 0);
    }

    public async Task AddToCartAsync(Guid productId)
    {
        var productToAdd = await _context.Products.FindAsync(productId);
        if (productToAdd == null) return;

        var cartItems = GetCartItems();
        var cartItem = cartItems.FirstOrDefault(ci => ci.ProductId == productId);

        if (cartItem != null)
        {
            cartItem.Quantity++;
        }
        else
        {
            cartItems.Add(new CartItem
            {
                ProductId = productId,
                Product = productToAdd,
                Quantity = 1
            });
        }

        Session.Set(CartSessionKey, cartItems);
    }

    public void ChangeQuantity(Guid productId, int adjustment)
    {
        var cartItems = GetCartItems();
        var cartItem = cartItems.FirstOrDefault(ci => ci.ProductId == productId);

        if (cartItem != null)
        {
            cartItem.Quantity += adjustment;
        }

        Session.Set(CartSessionKey, cartItems);
    }

    public void Remove(Guid productId)
    {
        var cartItems = GetCartItems();
        var cartItem = cartItems.FirstOrDefault(ci => ci.ProductId == productId);

        if (cartItem != null)
        {
            cartItems.Remove(cartItem);
        }

        Session.Set(CartSessionKey, cartItems);
    }
}
