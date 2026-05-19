using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuxeThreads.Data;
using LuxeThreads.Models;
using System.Text.Json;

namespace LuxeThreads.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        // ─── HOME ───
        public IActionResult Index()
        {
            var featured = _db.Products.Where(p => p.IsFeatured).ToList();
            return View(featured);
        }

        // ─── SHOP ───
        public IActionResult Shop(string? category, string? search)
        {
            var query = _db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category == category);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search) || p.Category.Contains(search));

            var vm = new ShopViewModel
            {
                Products = query.ToList(),
                Category = category,
                Search = search,
                Categories = _db.Products.Select(p => p.Category).Distinct().ToList()
            };
            return View(vm);
        }

        // ─── PRODUCT DETAIL ───
        public IActionResult Product(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null) return NotFound();
            var related = _db.Products.Where(p => p.Category == product.Category && p.Id != id).Take(3).ToList();
            ViewBag.Related = related;
            return View(product);
        }

        // ─── CART ───
        public IActionResult Cart()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, string size, int quantity = 1)
        {
            var product = _db.Products.Find(productId);
            if (product == null) return NotFound();

            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.ProductId == productId && c.Size == size);
            if (existing != null)
                existing.Quantity += quantity;
            else
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    Size = size,
                    Quantity = quantity,
                    Price = product.Price,
                    ColorHex = product.ColorHex,
                    AccentHex = product.AccentHex,
                    ImageUrl = product.ImageUrl
                });

            SaveCart(cart);
            TempData["Message"] = $"{product.Name} added to cart!";
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId, string size)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.ProductId == productId && c.Size == size);
            SaveCart(cart);
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, string size, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId && c.Size == size);
            if (item != null)
            {
                if (quantity <= 0) cart.Remove(item);
                else item.Quantity = quantity;
            }
            SaveCart(cart);
            return RedirectToAction("Cart");
        }

        // ─── CHECKOUT ───
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Cart");

            var vm = new CheckoutViewModel
            {
                CartItems = cart,
                Total = cart.Sum(c => c.Price * c.Quantity)
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult PlaceOrder(CheckoutViewModel vm)
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Cart");

            var order = new Order
            {
                UserEmail = vm.Email,
                CustomerName = vm.FullName,
                Address = vm.Address,
                City = vm.City,
                CardLast4 = vm.CardNumber.Length >= 4 ? vm.CardNumber[^4..] : "****",
                Total = cart.Sum(c => c.Price * c.Quantity),
                Status = "Confirmed",
                OrderDate = DateTime.Now,
                Items = cart.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    ProductName = c.ProductName,
                    Size = c.Size,
                    Quantity = c.Quantity,
                    Price = c.Price
                }).ToList()
            };

            _db.Orders.Add(order);
            _db.SaveChanges();
            SaveCart(new List<CartItem>());

            return RedirectToAction("OrderSuccess", new { id = order.Id });
        }

        public IActionResult OrderSuccess(int id)
        {
            var order = _db.Orders.Include(o => o.Items).FirstOrDefault(o => o.Id == id);
            if (order == null) return RedirectToAction("Index");
            return View(order);
        }

        // ─── CART HELPERS ───
        private List<CartItem> GetCart()
        {
            var json = HttpContext.Session.GetString("Cart");
            return string.IsNullOrEmpty(json)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
        }
    }
}