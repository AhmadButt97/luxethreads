using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuxeThreads.Data;
using LuxeThreads.Models;

namespace LuxeThreads.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        // ─── ADMIN DASHBOARD ───
        public IActionResult Index()
        {
            var vm = new AdminDashboardViewModel
            {
                TotalProducts = _db.Products.Count(),
                TotalOrders = _db.Orders.Count(),
                TotalUsers = _db.Users.Count(),
                TotalRevenue = _db.Orders
                                   .Where(o => o.Status == "Confirmed")
                                   .AsEnumerable()
                                   .Sum(o => o.Total),
                RecentOrders = _db.Orders
                                   .OrderByDescending(o => o.OrderDate)
                                   .Take(5)
                                   .ToList()
            };
            return View(vm);
        }

        // ─── PRODUCT MANAGEMENT ───
        public IActionResult Products()
        {
            var products = _db.Products.ToList();
            return View(products);
        }

        public IActionResult AddProduct()
        {
            return View(new Product());
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                TempData["FormErrors"] = "Product name is required.";
                return View(product);
            }

            product.Sizes = "S,M,L,XL";
            product.ColorHex = "#1a1a2e";
            product.AccentHex = "#c9a96e";
            product.IsFeatured = false;
            product.IsAvailable = true;
            product.CreatedAt = DateTime.Now;
            product.ImageUrl = string.IsNullOrWhiteSpace(product.ImageUrl)
                                  ? "/images/placeholder.jpg" : product.ImageUrl;
            product.Badge = product.Badge ?? "";
            product.OldPrice = product.OldPrice <= 0 ? 0 : product.OldPrice;

            _db.Products.Add(product);
            _db.SaveChanges();
            TempData["Message"] = "Product added successfully!";
            return RedirectToAction("Products");
        }

        public IActionResult EditProduct(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                TempData["FormErrors"] = "Product name is required.";
                return View(product);
            }

            var existing = _db.Products.Find(product.Id);
            if (existing == null) return NotFound();

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.OldPrice = product.OldPrice;
            existing.Category = product.Category;
            existing.Stock = product.Stock;
            existing.Badge = product.Badge ?? "";
            existing.ImageUrl = string.IsNullOrWhiteSpace(product.ImageUrl)
                                   ? existing.ImageUrl : product.ImageUrl;

            _db.SaveChanges();
            TempData["Message"] = "Product updated successfully!";
            return RedirectToAction("Products");
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var product = _db.Products.Find(id);
            if (product != null)
            {
                _db.Products.Remove(product);
                _db.SaveChanges();
                TempData["Message"] = "Product deleted!";
            }
            return RedirectToAction("Products");
        }

        // ─── ORDER MANAGEMENT ───
        public IActionResult Orders()
        {
            var orders = _db.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            return View(orders);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, string status)
        {
            var order = _db.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = status;
                _db.SaveChanges();
                TempData["Message"] = $"Order #{orderId} status updated to {status}";
            }
            return RedirectToAction("Orders");
        }
    }
}