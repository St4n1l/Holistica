namespace Holistica.Models
{
    public class Cart
    {
        public List<CartItem> Items = new List<CartItem>();

        public void AddItem(Guid productId, int quantity)
        {
            var existingItem = Items.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity; // Update quantity if the item already exists
            }
            else
            {
                Items.Add(new CartItem { ProductId = productId, Quantity = quantity }); // Add new item
            }
        }

        // Helper method to remove a product from the cart
        public void RemoveItem(Guid productId)
        {
            var itemToRemove = Items.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                Items.Remove(itemToRemove);
            }
        }

        // Helper method to update the quantity of a product in the cart
        public void UpdateQuantity(Guid productId, int quantity)
        {
            var itemToUpdate = Items.FirstOrDefault(item => item.ProductId == productId);
            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity = quantity;
            }
        }
    }
}
