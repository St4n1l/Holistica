using System.Text.Json.Serialization;

namespace Holistica.Models
{
    [Serializable]
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        
    }
}
