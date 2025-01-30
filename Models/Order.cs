namespace Holistica.Models
{
    public class Order
    {
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public string Status { get; set; } = null!;
        public decimal Price { get; set; } = 0;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PaymentStatus { get; set; } = "Pending";
        public DateTime OrderDate { get; set; }
    }
}
