using BookStore.Models.ApplicationUser;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        [Required]
        [StringLength(450)]
        public string? AppUserId { get; set; }
        public virtual AppUser? AppUser { get; set; }

        [Range(1, 1000, ErrorMessage = "orders should be not less than 1 and grater than 1000")]
        public int Count { get; set; }

        [NotMapped]
        public double Price { get; set; }
    }
}
