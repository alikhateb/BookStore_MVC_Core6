namespace BookStore.Models.Models
{
    public class ShippingCompany
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(150)]
        public string? Address { get; set; }

        [Required]
        [MaxLength(20)]
        public string? Phone { get; set; }
    }
}
