using Holistica.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
public class PaymentController : Controller
{
    private readonly StripeSettings _stripeSettings;

    public PaymentController(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings.Value;
        Console.WriteLine($"Loaded Stripe Secret Key: {_stripeSettings.SecretKey}"); // Debugging
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
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = 2000,
                        Currency = "bgn",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Test Product",
                        },
                    },
                    Quantity = 1,
                },
            },
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
