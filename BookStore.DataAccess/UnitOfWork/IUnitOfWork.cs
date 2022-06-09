using BookStore.DataAccess.IRepository;
using BookStore.Models;

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
        void Save();
    }
}