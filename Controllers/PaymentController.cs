﻿using System.Net.Mail;
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
        SendEmail();
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

    public static void SendEmail()
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Your Name", "your-email@gmail.com"));
        email.To.Add(new MailboxAddress("Recipient Name", "recipient-email@example.com"));
        email.Subject = "Subject of the Email";
        email.Body = new TextPart("plain")
        {
            Text = "This is the body of the email."
        };

        using (var smtp = new SmtpClient())
        {
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("your-email@gmail.com", "your-email-password");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
