using Holistica.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Holistica.Controllers
{
    public class PaymentController : Controller
    {
        private readonly StripeSettings _stripeSettings;
        public PaymentController(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
        }

        public void ProcessPayment()
        {
            var secretKey = _stripeSettings.SecretKey;
        }
    }
}
