using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using Stripe;
using Stripe.Checkout;
using MailKit.Net.Smtp;
using MailKit.Security;
using Holistica.Models.ViewModels;
using Holistica.Extension;

namespace Holistica.Services
{
    public class PaymentService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly GmailSettings _gmailSettings;
        private readonly CartService _cartService;

        public PaymentService(IOptions<StripeSettings> stripeSettings, CartService cartService, IOptions<GmailSettings> gmailSettings)
        {
            _stripeSettings = stripeSettings.Value;
            _cartService = cartService;
            _gmailSettings = gmailSettings.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }


        public string GetStripeKey()
        {
            return _stripeSettings.PublishableKey;
        }

        public async Task<Session> CreateCheckoutSessionAsync(HttpRequest request)
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
                SuccessUrl = $"{request.Scheme}://{request.Host}/Payment/Success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{request.Scheme}://{request.Host}/Payment/Cancel",
            };

            var service = new SessionService();
            return await service.CreateAsync(options);
        }

        public void SendOrderEmails(CheckOutViewModel order)
        {
            string items = string.Join(", ", _cartService.GetCartItems());
            string orderText = $"New order from {order.Name} with the following items: {items}\n" +
                               $"Phone: {order.Phone}\n" +
                               $"Address: {order.Address}\n" +
                               $"Payment: {order.PaymentMethod}";
            SendEmail(_gmailSettings.Email, "New Order", orderText);
            SendEmail(order.Email, "Order Confirmation", "Your order has been successfully placed. Thank you for shopping at Holistica!");
        }

        private void SendEmail(string recipient, string subject, string message)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Holistica", _gmailSettings.Email));
            email.To.Add(new MailboxAddress(recipient, recipient));
            email.Subject = subject;
            email.Body = new TextPart("plain") { Text = message };

            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(_gmailSettings.Email, _gmailSettings.EmailPassword);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }
    }
}
