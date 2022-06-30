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
#pragma warning disable CS8603 // Possible null reference return.
            return _context.Orders.OrderBy(order => order.Id).LastOrDefault(filter);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public void UpdateStatus(int id, string? orderStatus = null, string? paymentStatus = null)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Order order = _context.Orders.SingleOrDefault(item => item.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            if (orderStatus is not null)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                order.OrderStatus = orderStatus;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
            if (paymentStatus is not null)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                order.PaymentStatus = paymentStatus;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Order order = _context.Orders.SingleOrDefault(item => item.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            order.SessionId = sessionId;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            order.PaymentIntentId = paymentIntentId;
        }
    }
}
