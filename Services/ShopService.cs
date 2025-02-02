using System.Net;
using Holistica.Data;
using Holistica.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Holistica.Services
{
    public class ShopService(ApplicationDbContext context)
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<Product>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetSpecificProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                throw new HttpIOException((HttpRequestError)HttpStatusCode.NotFound, "Product not found");
            }

            return product;
        }

        public async Task<IActionResult> AddProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new OkResult();

        }

        public async Task<IActionResult> ChangeQuantity(string productId, int quantity)
        {
            var neededProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == Guid.Parse(productId));

            if (neededProduct != null)
            {
                neededProduct.Quantity = quantity;
                await _context.SaveChangesAsync();
                return new OkResult();
            }

            return new BadRequestResult();
        }

        public async Task<List<Product>> Search(string input)
        {
            List<Product> products;

            if (string.IsNullOrWhiteSpace(input))
            {
                products = await _context.Products.ToListAsync();
            }
            else
            {
                products = await _context.Products
                    .Where(p => p.Name.Contains(input))
                    .ToListAsync();
            }

            return products;
        }

        public async Task<IActionResult> Remove(string id)
        {
            var product = await _context.Products.FindAsync(Guid.Parse(id));

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return new OkResult();
        }

        public async Task<IActionResult> ChangeProduct(string id, string name, string description, decimal price, int quantity, string imageUrl)
        {
            var product = await _context.Products.FindAsync(Guid.Parse(id));

            if (product != null)
            {
                product.Name = name;
                product.Description = description;
                product.Price = price;
                product.Quantity = quantity;
                product.ImageUrl = imageUrl;
                await _context.SaveChangesAsync();
                return new OkResult();
            }

            return new BadRequestResult();
        }

    }
}
