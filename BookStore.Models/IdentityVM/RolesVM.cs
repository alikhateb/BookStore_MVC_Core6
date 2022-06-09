using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.IdentityVM
{
    public class RolesVM
    {
        [Required,
         StringLength(50)]
        public string? Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
