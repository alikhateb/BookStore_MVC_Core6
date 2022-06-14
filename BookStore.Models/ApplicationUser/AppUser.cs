using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.ApplicationUser
{
    public class AppUser : IdentityUser
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }
        public byte[]? ProfilePicture { get; set; }

        [MaxLength(100)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? City { get; set; }

        [MaxLength(20)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
    }
}
