namespace BookStore.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DbInitializer(
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager,
            ApplicationDbContext context)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            //apply migrations if they are not applied
            try
            {
                _context.Database.EnsureCreated();
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //create roles if they are not created
            if (!_roleManager.RoleExistsAsync(StaticDetails.Role_Admin).GetAwaiter().GetResult())   //add roles
            {
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_User_Comp)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_User_Indi)).GetAwaiter().GetResult();

                //if roles are not created, then we will create admin user as well
                var user = new AppUser
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    PhoneNumber = "01022330555",
                    Address = "cairo",
                    City = "cairo",
                    State = "cairo",
                    PostalCode = "12345"
                };

                _userManager.CreateAsync(user, "Admin123@").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, StaticDetails.Role_Admin).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
