namespace BookStore.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(_context);
            CoverType = new CoverTypeRepository(_context);
            Product = new ProductRepository(_context);
            Company = new CompanyRepository(_context);
            ShoppingCartItem = new ShoppingCartItemRepository(_context);
            OrderDetail = new OrderDetailRepository(_context);
            Order = new OrderRepository(_context);
            ShippingCompany = new ShippingCompanyRepository(_context);
        }

        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartItemRepository ShoppingCartItem { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IShippingCompanyRepository ShippingCompany { get; private set; }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
