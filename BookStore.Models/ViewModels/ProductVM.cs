namespace BookStore.Models.ViewModels
{
    public class ProductVM
    {
        public Product? Product { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
        public IEnumerable<CoverType>? CoverTypes { get; set; }
    }
}
