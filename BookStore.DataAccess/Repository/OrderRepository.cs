namespace BookStore.DataAccess.Repository
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Order FindLastOrder(Expression<Func<Order, bool>> filter)
        {
            return _context.Orders.OrderBy(order => order.Id).LastOrDefault(filter);
        }

        public void UpdateStatus(int id, string? orderStatus = null, string? paymentStatus = null)
        {
            Order order = _context.Orders.SingleOrDefault(item => item.Id == id);
            if (orderStatus is not null)
            {
                order.OrderStatus = orderStatus;
            }
            if (paymentStatus is not null)
            {
                order.PaymentStatus = paymentStatus;
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            Order order = _context.Orders.SingleOrDefault(item => item.Id == id);
            order.SessionId = sessionId;
            order.PaymentIntentId = paymentIntentId;
        }
    }
}
