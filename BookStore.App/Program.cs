using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

//register Db options 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseLazyLoadingProxies()
           .UseSqlServer(builder.Configuration.GetConnectionString("cs"))
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

//register UserManager<IdentityUser> == repository for IdentityUser class - RoleManager<IdentityRole> - SignInManager<IdentityUser> 
builder.Services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()    //add UserStore class - RoleStore class that interact with database 
                .AddDefaultTokenProviders()
                .AddDefaultUI();

//register IunitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//configure and bind properities stripe from app settings to stripe object in Utility
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

//register IEmailSender
builder.Services.AddSingleton<IEmailSender, EmailSender>();

//configure path when authorization is required
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.LoginPath = $"/Identity/Account/Login";
//    options.LogoutPath = $"/Identity/Account/Logout";
//    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
//});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "788840455858078";
    options.AppSecret = "435ac457a004af61784ba3dc397db7c3";
});

builder.Services.AddScoped<IDbInitializer, DbInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

SeedDatabase();

//configure Stripe api key in pipeline
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:Secretkey").Get<string>();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDatabase()
{
    using var scope = app.Services.CreateScope();
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer.Initialize();
}