namespace BookStore.DataAccess.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> filter)
        {
            return _context.Set<T>().Where(filter);
        }

        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var navigationProperty in navigationProperties)
            {
                query = query.Include(navigationProperty);
            }
            return query;
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = _context.Set<T>().Where(filter);
            foreach (var navigationProperty in navigationProperties)
            {
                query = query.Include(navigationProperty);
            }
            return query;
        }

        public T FindObject(Expression<Func<T, bool>> filter)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return _context.Set<T>().SingleOrDefault(filter);
#pragma warning restore CS8603 // Possible null reference return.

        }

        public T FindObject(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = _context.Set<T>().Where(filter);
            foreach (var navigationProperty in navigationProperties)
            {
                query = query.Include(navigationProperty);
            }
#pragma warning disable CS8603 // Possible null reference return.
            return query.SingleOrDefault(filter);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }
    }
}
