using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// DbContext (SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

// Add ASP.NET Core Identity (registers UserManager, SignInManager, etc.)
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// нашите услуги
builder.Services.AddScoped<ICounterService, CounterService>();
builder.Services.AddTransient<IFileStorageService, FileStorageService>();
builder.Services.AddTransient<IWordPlaceholderService, WordPlaceholderService>();
// word to pdf Service
builder.Services.AddTransient<IWordToPdfService, WordToPdfService>();

var app = builder.Build();

// Ensure DB and seed Counter row (lightweight, no migrations required)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    db.Database.EnsureCreated();

    if (!db.Counters.Any())
    {
        db.Counters.Add(new Counter { Id = 1, Value = 0 });
        db.SaveChanges();
    }
}

app.UseStaticFiles();

app.UseRouting();

// must be added so Identity works and UserManager can be resolved
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();