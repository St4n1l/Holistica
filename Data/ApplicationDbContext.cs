using Holistica.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Holistica.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Cart) // ApplicationUser has one Cart
                .WithOne(c => c.User)
                .HasForeignKey<ApplicationUser>(u => u.CartId)
                .OnDelete(DeleteBehavior.Cascade); // Delete cart when user is deleted

            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems) // Cart has many CartItems
                .WithOne(ci => ci.Cart) // CartItem has one Cart
                .HasForeignKey(ci => ci.CartId) // Foreign key in CartItem
                .OnDelete(DeleteBehavior.Cascade); // Delete cart items when cart is deleted

            // Relationship between CartItem and Product
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product) // CartItem has one Product
                .WithMany() // Product can be in many CartItems
                .HasForeignKey(ci => ci.ProductId) // Foreign key in CartItem
                .OnDelete(DeleteBehavior.Cascade); // Delete cart item when product is deleted
        }
    }


}
