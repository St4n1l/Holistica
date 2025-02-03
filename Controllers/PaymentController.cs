using Holistica.Extension;
using Holistica.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
public class PaymentController : Controller
{
    private readonly StripeSettings _stripeSettings;
    private readonly CartService _cartService;
    public PaymentController(IOptions<StripeSettings> stripeSettings, CartService cartService)
    {
        _stripeSettings = stripeSettings.Value;
        _cartService = cartService;
        Console.WriteLine($"Loaded Stripe Secret Key: {_stripeSettings.SecretKey}");
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

    }

    [HttpGet]
    public IActionResult Checkout()
    {
        ViewBag.PublishableKey = _stripeSettings.PublishableKey;
        return PartialView();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCheckoutSession()
    {
        var cartItems = _cartService.GetCartItems();

        var lineItems = cartItems.Select(ci => new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmount = (long)(_cartService.GetTotalPrice() * 100),
                Currency = "bgn",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = ci.Product.Name,
                },
            },
            Quantity = ci.Quantity,

        }).ToList();

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = $"{Request.Scheme}://{Request.Host}/Payment/Success",
            CancelUrl = $"{Request.Scheme}://{Request.Host}/Payment/Cancel",

        };

        try
        {
            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            return Json(new { id = session.Id });
        }
        catch (StripeException e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    public IActionResult Success()
    {
        return PartialView();
    }

    public IActionResult Cancel()
    {
        return PartialView();
    }
}
