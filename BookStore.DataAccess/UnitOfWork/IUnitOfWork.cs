namespace BookStore.DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Category> Category { get; }
        IBaseRepository<CoverType> CoverType { get; }
        IBaseRepository<Product> Product { get; }
        IBaseRepository<Company> Company { get; }
        IShoppingCartItemRepository ShoppingCartItem { get; }
        IBaseRepository<OrderDetail> OrderDetail { get; }
        IOrderRepository Order { get; }
        IBaseRepository<ShippingCompany> ShippingCompany { get; }
        void SaveChanges();
    }
}