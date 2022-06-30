namespace BookStore.DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        ICoverTypeRepository CoverType { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        IShoppingCartItemRepository ShoppingCartItem { get; }
        IOrderDetailRepository OrderDetail { get; }
        IOrderRepository Order { get; }
        IShippingCompanyRepository ShippingCompany { get; }
        void SaveChanges();
    }
}