using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Holistica.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required] 
        public string LastName { get; set; } = null!;

        public Guid? CartId { get; set; }

        [ForeignKey("CartId")]
        public Cart Cart { get; set; } = null!;
    }
}
