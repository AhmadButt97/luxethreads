using LuxeThreads.Data;
using LuxeThreads.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=luxethreads.db"));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "LuxeThreads.Cookie";
        config.LoginPath = "/Account/Login";
        config.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Products.Any())
    {
        db.Products.AddRange(
            // ── T-SHIRTS ──
            new Product { Id = 1, Name = "Obsidian Oversized Tee", Description = "Premium heavyweight cotton, dropped shoulders, minimalist cut.", Price = 89.00m, Category = "T-Shirts", ImageUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=600&q=80", Badge = "NEW", Stock = 50, IsAvailable = true, IsFeatured = true },
            new Product { Id = 2, Name = "Shadow Stripe Longsleeve", Description = "Tonal stripe pattern on 240gsm organic cotton.", Price = 99.00m, OldPrice = 129.00m, Category = "T-Shirts", ImageUrl = "https://images.unsplash.com/photo-1618354691373-d851c5c3a990?w=600&q=80", Badge = "SALE", Stock = 60, IsAvailable = true, IsFeatured = true },
            new Product { Id = 3, Name = "Cloud Cotton Crew", Description = "Ultra-soft 100% pima cotton with relaxed boxy fit.", Price = 75.00m, Category = "T-Shirts", ImageUrl = "https://images.unsplash.com/photo-1583743814966-8936f5b7be1a?w=600&q=80", Badge = "", Stock = 80, IsAvailable = true, IsFeatured = false },
            new Product { Id = 4, Name = "Matte Black Polo", Description = "Pique cotton polo with contrast tipping. Timeless and refined.", Price = 110.00m, Category = "T-Shirts", ImageUrl = "https://images.unsplash.com/photo-1598033129183-c4f50c736f10?w=600&q=80", Badge = "NEW", Stock = 35, IsAvailable = true, IsFeatured = false },
            // ── HOODIES ──
            new Product { Id = 5, Name = "Marble Wash Hoodie", Description = "French terry fleece with custom marble-dye finish.", Price = 149.00m, Category = "Hoodies", ImageUrl = "https://images.unsplash.com/photo-1556821840-3a63f15732ce?w=600&q=80", Badge = "HOT", Stock = 30, IsAvailable = true, IsFeatured = true },
            new Product { Id = 6, Name = "Phantom Zip Hoodie", Description = "Full-zip heavyweight fleece with kangaroo pocket.", Price = 169.00m, Category = "Hoodies", ImageUrl = "https://images.unsplash.com/photo-1509942774463-acf339cf87d5?w=600&q=80", Badge = "NEW", Stock = 25, IsAvailable = true, IsFeatured = true },
            new Product { Id = 7, Name = "Acid Wash Pullover", Description = "Vintage-inspired acid wash on premium loopback cotton.", Price = 139.00m, OldPrice = 179.00m, Category = "Hoodies", ImageUrl = "https://images.unsplash.com/photo-1565693413579-8ff3fdc1b03b?w=600&q=80", Badge = "SALE", Stock = 20, IsAvailable = true, IsFeatured = false },
            new Product { Id = 8, Name = "Oversized Blank Hoodie", Description = "Minimal 400gsm cotton blend, dropped shoulders, unbranded.", Price = 125.00m, Category = "Hoodies", ImageUrl = "https://images.unsplash.com/photo-1542327897-d73f4005b533?w=600&q=80", Badge = "", Stock = 45, IsAvailable = true, IsFeatured = false },
            // ── JACKETS ──
            new Product { Id = 9, Name = "Raw Edge Denim Jacket", Description = "Selvedge denim with intentional raw hem detailing.", Price = 219.00m, Category = "Jackets", ImageUrl = "https://images.unsplash.com/photo-1551537482-f2075a1d41f2?w=600&q=80", Badge = "NEW", Stock = 15, IsAvailable = true, IsFeatured = true },
            new Product { Id = 10, Name = "Utility Vest", Description = "Multi-pocket tactical vest in waxed cotton.", Price = 189.00m, Category = "Jackets", ImageUrl = "https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=600&q=80", Badge = "HOT", Stock = 20, IsAvailable = true, IsFeatured = true },
            new Product { Id = 11, Name = "Waxed Trucker Jacket", Description = "Classic trucker silhouette in Italian waxed canvas.", Price = 259.00m, Category = "Jackets", ImageUrl = "https://images.unsplash.com/photo-1544022613-e87ca75a784a?w=600&q=80", Badge = "NEW", Stock = 12, IsAvailable = true, IsFeatured = false },
            new Product { Id = 12, Name = "Bomber in Onyx", Description = "MA-1 inspired bomber in heavyweight ripstop nylon.", Price = 235.00m, OldPrice = 289.00m, Category = "Jackets", ImageUrl = "https://images.unsplash.com/photo-1548126032-079a0fb0099d?w=600&q=80", Badge = "SALE", Stock = 18, IsAvailable = true, IsFeatured = false },
            // ── BOTTOMS ──
            new Product { Id = 13, Name = "Slim Noir Trousers", Description = "Technical stretch fabric with tapered silhouette.", Price = 129.00m, OldPrice = 179.00m, Category = "Bottoms", ImageUrl = "https://images.unsplash.com/photo-1509631179647-0177331693ae?w=600&q=80", Badge = "SALE", Stock = 25, IsAvailable = true, IsFeatured = true },
            new Product { Id = 14, Name = "Silk Touch Cargo", Description = "Lightweight nylon-blend cargo with minimal hardware.", Price = 159.00m, Category = "Bottoms", ImageUrl = "https://images.unsplash.com/photo-1624378439575-d8705ad7ae80?w=600&q=80", Badge = "", Stock = 40, IsAvailable = true, IsFeatured = false },
            new Product { Id = 15, Name = "Phantom Shorts", Description = "Board-short inspired silhouette with mesh liner.", Price = 79.00m, Category = "Bottoms", ImageUrl = "https://images.unsplash.com/photo-1591195853828-11db59a44f43?w=600&q=80", Badge = "NEW", Stock = 45, IsAvailable = true, IsFeatured = false },
            new Product { Id = 16, Name = "Pleated Wide-Leg Trousers", Description = "Relaxed wide-leg cut in Japanese wool-blend fabric.", Price = 175.00m, Category = "Bottoms", ImageUrl = "https://images.unsplash.com/photo-1473966968600-fa801b869a1a?w=600&q=80", Badge = "NEW", Stock = 22, IsAvailable = true, IsFeatured = false }
        );
        db.SaveChanges();
    }

    // ✅ FIXED: credentials match what's shown in the login view
    if (!db.Users.Any(u => u.Role == "Admin"))
    {
        db.Users.Add(new AppUser
        {
            FullName = "Admin",
            Email = "admin@luxethreads.com",
            Password = "Admin@123",
            Role = "Admin",
            CreatedAt = DateTime.Now
        });
        db.SaveChanges();
    }

    // Seed a demo user
    if (!db.Users.Any(u => u.Role == "User"))
    {
        db.Users.Add(new AppUser
        {
            FullName = "Demo User",
            Email = "user@luxethreads.com",
            Password = "User@123",
            Role = "User",
            CreatedAt = DateTime.Now
        });
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();