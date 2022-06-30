namespace BookStore.DataAccess.Repository
{
    public class CoverTypeRepository : BaseRepository<CoverType>, ICoverTypeRepository
    {
        public CoverTypeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
