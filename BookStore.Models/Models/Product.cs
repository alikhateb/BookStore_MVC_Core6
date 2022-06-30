namespace BookStore.Models.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [Required]
        [MaxLength(300)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ISBN { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Author { get; set; }

        [Required]
        [Range(1, 10000)]
        public double Price { get; set; }

        [Required]
        [Range(1, 10000)]
        [Display(Name = "Price for 51-100")]
        public double Price50 { get; set; }

        [Required]
        [Range(1, 10000)]
        [Display(Name = "Price for 100+")]
        public double Price100 { get; set; }

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        [Display(Name = "Cover Type")]
        public int CoverTypeId { get; set; }
        public virtual CoverType? CoverType { get; set; }
    }
}
