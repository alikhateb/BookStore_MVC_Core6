namespace BookStore.DataAccess.IRepository
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Order FindLastOrder(Expression<Func<Order, bool>> filter);
        void UpdateStatus(int id, string orderStatus, string paymentStatus);
        void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
    }
}
