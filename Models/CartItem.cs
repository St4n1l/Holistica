using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Holistica.Models
{
    public class CartItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; }

        [Required]
        [ForeignKey("ProductId")]
        public Product Product{ get; set; }

        public Guid CartId { get; set; }


        [Required]
        [ForeignKey("CartId")]
        public Cart Cart { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
