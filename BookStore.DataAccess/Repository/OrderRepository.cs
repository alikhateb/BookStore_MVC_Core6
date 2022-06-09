using BookStore.DataAccess.Data;
using BookStore.DataAccess.IRepository;
using BookStore.Models;
using System.Linq.Expressions;

namespace BookStore.DataAccess.Repository
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Order FindLastOrder(Expression<Func<Order, bool>> filter)
        {
            return _context.Orders.OrderBy(order => order.Id).LastOrDefault(filter);
        }

        public void UpdateStatus(int id, string orderStatus, string paymentStatus)
        {
            Order order = _context.Orders.SingleOrDefault(item => item.Id == id);
            order.OrderStatus = orderStatus;
            order.PaymentStatus = paymentStatus;
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            Order order = _context.Orders.SingleOrDefault(item => item.Id == id);
            order.SessionId = sessionId;
            order.PaymentIntentId = paymentIntentId;
            order.PaymentDate = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
        }
    }
}
