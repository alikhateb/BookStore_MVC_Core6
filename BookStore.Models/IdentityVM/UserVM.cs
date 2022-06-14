namespace BookStore.Models.IdentityVM
{
    public class UserVM
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public IEnumerable<string>? Roles { get; set; }
    }
}
