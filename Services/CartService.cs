using System.Text;
using System.Text.Json;
using Holistica.Data;
using Holistica.Models;
using Holistica.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
        var cart = Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        var price = cart.Sum(ci => ci.Product.Price * ci.Quantity);

        return price;
    }

    public void AddToCart(Guid productId)
    {
        var productToAdd = _context.Products.Find(productId);

        if (productToAdd == null)
        {
            return;
        }

        var cartItems = GetCartItems();

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

        Session.Set(CartSessionKey, cartItems);
    }
}