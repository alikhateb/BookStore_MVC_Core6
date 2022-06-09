namespace BookStore.Models.ViewModels
{
    public class ShoppingCartVm
    {
        public IEnumerable<ShoppingCartItem>? CartItems { get; set; }
        public Order? Order { get; set; }
    }
}
