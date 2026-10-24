// Member 6 summary: registers integer Identity, notification services, admin authorization, and deactivates disabled-user cookies.
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Persistence;
using DiscoverEgypt.Application.Notifications;
using DiscoverEgypt.Infrastructure.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DiscoverEgyptDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<DiscoverEgyptDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.ConfigureApplicationCookie(options => options.Events.OnValidatePrincipal = async context =>
{
    var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
    var user = await userManager.GetUserAsync(context.Principal);
    if (user is null || !user.IsActive)
    {
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
