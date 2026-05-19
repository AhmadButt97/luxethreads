using System.ComponentModel.DataAnnotations;
using LuxeThreads.Data;

namespace LuxeThreads.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public string Category { get; set; } = "";
        public string Sizes { get; set; } = "S,M,L,XL";
        public int Stock { get; set; } = 100;
        public string ColorHex { get; set; } = "#1a1a2e";
        public string AccentHex { get; set; } = "#c9a96e";
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string ImageUrl { get; set; } = "/images/placeholder.jpg";
        public string Badge { get; set; } = "";
        public decimal OldPrice { get; set; } = 0;
        public bool IsAvailable { get; set; } = true;
    }

    public class Order
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string CardLast4 { get; set; } = "";
        public decimal Total { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string Size { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }

    public class AppUser
    {
        public int Id { get; set; }
        [Required] public string Email { get; set; } = "";
        [Required] public string Password { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string Size { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ColorHex { get; set; } = "#1a1a2e";
        public string AccentHex { get; set; } = "#c9a96e";
        public string ImageUrl { get; set; } = "/images/placeholder.jpg";
    }

    // ✅ FIXED: Removed [Required] and [EmailAddress] — controller validates manually
    public class LoginViewModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required] public string FullName { get; set; } = "";
        [Required][EmailAddress] public string Email { get; set; } = "";
        [Required][MinLength(6)] public string Password { get; set; } = "";
        [Required][Compare("Password")] public string ConfirmPassword { get; set; } = "";
    }

    public class CheckoutViewModel
    {
        [Required] public string FullName { get; set; } = "";
        [Required][EmailAddress] public string Email { get; set; } = "";
        [Required] public string Address { get; set; } = "";
        [Required] public string City { get; set; } = "";
        [Required] public string ZipCode { get; set; } = "";
        [Required] public string CardNumber { get; set; } = "";
        [Required] public string CardExpiry { get; set; } = "";
        [Required] public string CardCvv { get; set; } = "";
        public decimal Total { get; set; }
        public List<CartItem> CartItems { get; set; } = new();
    }

    public class ShopViewModel
    {
        public List<Product> Products { get; set; } = new();
        public string? Category { get; set; }
        public string? Search { get; set; }
        public List<string> Categories { get; set; } = new();
    }

    public class AdminDashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Order> RecentOrders { get; set; } = new();
        public List<Product> Products { get; set; } = new();
    }

    public static class DataSeeder
    {
        public static void SeedOrders(AppDbContext context) { }
        public static void Seed(AppDbContext context) { SeedOrders(context); }
    }
}