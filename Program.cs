using System.Globalization;
using Holistica.Data;
using Holistica.Extension;
using Holistica.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<ShopService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true;

});

var config = builder.Configuration;
var stripePublishableKey = config["Stripe:PublishableKey"];
var stripeSecretKey = config["Stripe:SecretKey"];
var emailUsername = config["AppKey:Email"];
var emailPassword = config["AppKey:EmailPassword"];
builder.Services.Configure<StripeSettings>(config.GetSection("Stripe"));
builder.Services.Configure<GmailSettings>(config.GetSection("AppKey"));

var cultureInfo = new CultureInfo("bg-BG");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


var app = builder.Build();
app.UseSession();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    builder.Configuration.AddUserSecrets<Program>();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

app.Run();