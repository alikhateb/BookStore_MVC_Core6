namespace BookStore.DataAccess.IRepository
{
    public interface IShoppingCartItemRepository : IBaseRepository<ShoppingCartItem>
    {
        int IncrementCount(ShoppingCartItem item, int count);
        int DecrementCount(ShoppingCartItem item, int count);
    }
}
