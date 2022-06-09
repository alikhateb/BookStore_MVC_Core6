using BookStore.DataAccess.Data;
using BookStore.DataAccess.IRepository;
using BookStore.Models;

namespace BookStore.DataAccess.Repository
{
    public class ShoppingCartItemRepository : BaseRepository<ShoppingCartItem>, IShoppingCartItemRepository
    {
        private readonly AppDbContext _context;

        public ShoppingCartItemRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public int DecrementCount(ShoppingCartItem item, int count)
        {
            item.Count -= count;
            if (item.Count <= 1)
            {
                item.Count = 1;
            }
            return item.Count;
        }

        public int IncrementCount(ShoppingCartItem item, int count)
        {
            item.Count += count;
            if (item.Count >= 1000)
            {
                item.Count = 1000;
            }
            return item.Count;
        }
    }
}
