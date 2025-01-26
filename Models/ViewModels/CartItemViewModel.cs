namespace Holistica.Models.ViewModels
{
    public class CartItemViewModel
    {
        public List<CartItem> CartItems { get; set; }

        public decimal TotalPrice { get; set; } = 0;
    }
}
