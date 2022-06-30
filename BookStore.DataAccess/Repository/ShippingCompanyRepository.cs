namespace BookStore.DataAccess.Repository
{
    public class ShippingCompanyRepository : BaseRepository<ShippingCompany>, IShippingCompanyRepository
    {
        public ShippingCompanyRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
