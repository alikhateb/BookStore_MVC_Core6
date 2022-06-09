using BookStore.DataAccess.Data;
using BookStore.DataAccess.IRepository;
using BookStore.DataAccess.Repository;
using BookStore.Models;

namespace BookStore.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Category = new BaseRepository<Category>(_context);
            CoverType = new BaseRepository<CoverType>(_context);
            Product = new BaseRepository<Product>(_context);
            Company = new BaseRepository<Company>(_context);
            ShoppingCartItem = new ShoppingCartItemRepository(_context);
            OrderDetail = new BaseRepository<OrderDetail>(_context);
            Order = new OrderRepository(_context);
        }

        public IBaseRepository<Category> Category { get; private set; }
        public IBaseRepository<CoverType> CoverType { get; private set; }
        public IBaseRepository<Product> Product { get; private set; }
        public IBaseRepository<Company> Company { get; private set; }
        public IShoppingCartItemRepository ShoppingCartItem { get; private set; }
        public IBaseRepository<OrderDetail> OrderDetail { get; private set; }
        public IOrderRepository Order { get; private set; }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
