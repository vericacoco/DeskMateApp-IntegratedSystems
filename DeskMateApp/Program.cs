using DeskMateApp.Domain.Identity;
using DeskMateApp.Repository.Data;
using DeskMateApp.Service.Implementation;
using DeskMateApp.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddTransient<IOfficeLocationService, OfficeLocationService>();
builder.Services.AddTransient<IDeskService, DeskService>();
builder.Services.AddTransient<IReservationService, ReservationService>();
builder.Services.AddTransient<IAmenityService, AmenityService>();
builder.Services.AddMemoryCache();

var holidaysBaseUrl = builder.Configuration["ExternalApis:HolidaysBaseUrl"];

builder.Services.AddHttpClient<IHolidayService, HolidayService>(client =>
{
    client.BaseAddress = new Uri(holidaysBaseUrl!);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await ApplicationDbContextSeeder.SeedDatabase(context, userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
