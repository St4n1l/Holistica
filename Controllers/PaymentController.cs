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
            SuccessUrl = $"{Request.Scheme}://{Request.Host}/Payment/Success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{Request.Scheme}://{Request.Host}/Payment/Cancel",
        };

        try
        {
            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            return Redirect(session.Url);
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
    public IActionResult GatherInfo()
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
            HttpContext.Session.SetString("PaymentMethod", model.PaymentMethod);
        }

        if (model.PaymentMethod == "card")
        {
            // Instead of redirecting (which causes a GET), return a view that auto-posts to CreateCheckoutSession
            return View("ProcessData");
        }

        // For cash (cod) orders, send emails and redirect appropriately
        return RedirectToAction("OnDelivery");
    }





    public IActionResult OnDelivery()
    {
        SendEmailToMe(HttpContext.Session.GetString("PaymentMethod"));
        SendEmailToCustomer();

        return RedirectToAction("Index", "Shop");
    }



    public void SendEmailToMe(string paymentMethod)
    {
        var items = String.Join(", ", _cartService.GetCartItems());
        var text = $"New order from {HttpContext.Session.GetString("CustomerName")} with the following items: {items}\n" +
                   $"Phone: {HttpContext.Session.GetString("CustomerPhone")}\n" +
                   $"Name: {HttpContext.Session.GetString("CustomerName")}\n" +
                   $"Address: { HttpContext.Session.GetString("CustomerAddress")}\n" +
                   $"Payment: {paymentMethod}";

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

    public void SendEmailToCustomer()
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Holistica", _gmailSettings.Email));
        email.To.Add(new MailboxAddress(HttpContext.Session.GetString("CustomerName"), HttpContext.Session.GetString("CustomerEmail")));
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
