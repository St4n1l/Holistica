using System.Net.Mail;
using Holistica.Extension;
using Holistica.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using Stripe;
using Stripe.Checkout;
using MailKit.Net.Smtp;
using MailKit.Security;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Holistica.Models.ViewModels;
public class PaymentController : Controller
{
    private readonly StripeSettings _stripeSettings;
    private readonly CartService _cartService;
    private readonly GmailSettings _gmailSettings;
    public PaymentController(IOptions<StripeSettings> stripeSettings, CartService cartService, IOptions<GmailSettings> gmailSettings)
    {
        _stripeSettings = stripeSettings.Value;
        _cartService = cartService;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        _gmailSettings = gmailSettings.Value;

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
                UnitAmount = (long)(ci.Product.Price * 100),
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

    [Route("Payment/")]
    public IActionResult GatherInfo(string customerEmail, string customerName, string customerAddress, string customerPhone)
    {
        return PartialView();
    }


    [HttpPost]
    public IActionResult ProcessCheckout(CheckOutViewModel model)
    {
        if (ModelState.IsValid)
        {
            HttpContext.Session.SetString("CustomerEmail", model.Email);
            HttpContext.Session.SetString("CustomerName", model.Name);
            HttpContext.Session.SetString("CustomerPhone", model.Phone);
            HttpContext.Session.SetString("CustomerAddress", model.Address);

            string paymentMethod = model.PaymentMethod;

            if (paymentMethod == "card")
            {
                return RedirectToAction("CreateCheckoutSession");
            }
            else if (paymentMethod == "cod")
            {
                SendEmailToCustomer(model.Email, model.Name);
                SendEmailToMe(model.Name,model.Address,model.Phone);
                return RedirectToAction("Success");
            }
        }

        return RedirectToAction("Index", "Shop");
    }



    public void SendEmailToMe(string name, string address, string phone)
    {
        var items = String.Join(", ", _cartService.GetCartItems());
        var text = $"New order from {name} with the following items: {items}\n" +
                   $"Phone: {HttpContext.Session.GetString("CustomerPhone")}\n" +
                   $"Name: {HttpContext.Session.GetString("CustomerName")}\n" +
                   $"Address: { HttpContext.Session.GetString("CustomerAddress")}";

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Holistica", _gmailSettings.Email));
        email.To.Add(new MailboxAddress("Me", _gmailSettings.Email));
        email.Subject = "New Order";
        email.Body = new TextPart("plain")
        {
            Text = text
        };


        using (var smtp = new SmtpClient())
        {
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_gmailSettings.Email, _gmailSettings.EmailPassword);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
        
    }

    public void SendEmailToCustomer(string customerEmail, string recipientName)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Holistica", _gmailSettings.Email));
        email.To.Add(new MailboxAddress(recipientName, customerEmail));
        email.Subject = "Order confirmation";
        email.Body = new TextPart("plain")
        {
            Text = "Your order has been successfully places. Thank you for shopping at Holistica!"
        };


        using (var smtp = new SmtpClient())
        {
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_gmailSettings.Email, _gmailSettings.EmailPassword);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

    }
}
