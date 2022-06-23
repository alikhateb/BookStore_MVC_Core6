using BookStore.Models.ApplicationUser;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string? AppUserId { get; set; }
        public virtual AppUser? AppUser { get; set; }

        [StringLength(20)]
        [Display(Name = "Order Date")]
        public string OrderDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");

        [StringLength(20)]
        [Display(Name = "Shipping Date")]
        public string? ShippingDate { get; set; }

        [Display(Name = "Total Price")]
        public double TotalPrice { get; set; }

        [StringLength(200)]
        [Display(Name = "Order Status")]
        public string? OrderStatus { get; set; }

        [StringLength(200)]
        [Display(Name = "Payment Status")]
        public string? PaymentStatus { get; set; }

        [StringLength(200)]
        [Display(Name = "Tracking Number")]
        public string? TrackingNumber { get; set; }

        [StringLength(200)]
        public string? Carrier { get; set; }

        [Display(Name = "Payment Date")]
        [StringLength(20)]
        public string? PaymentDate { get; set; }

        [Display(Name = "Payment Due Date")]
        [StringLength(20)]
        public string? PaymentDueDate { get; set; }

        [StringLength(200)]
        public string? SessionId { get; set; }

        [StringLength(200)]
        public string? PaymentIntentId { get; set; }

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
        [Display(Name = "Postal Code")]
        [StringLength(20)]
        public string? PostalCode { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Phone")]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string? Email { get; set; }
    }
}
