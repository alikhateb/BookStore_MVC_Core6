namespace BookStore.DataAccess.Repository
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
