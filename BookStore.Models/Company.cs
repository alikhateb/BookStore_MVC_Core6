namespace BookStore.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(200)]
        public string? Address { get; set; }

        [Required]
        [StringLength(50)]
        public string? City { get; set; }

        [Required]
        [StringLength(50)]
        public string? State { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Phone")]
        public string? PhoneNumber { get; set; }
    }
}
