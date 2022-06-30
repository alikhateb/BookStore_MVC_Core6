namespace BookStore.Models.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Created Date")]
        [StringLength(50)]
        public string CreatedDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
    }
}
