using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ASP_GCH1107_NguyenVanTruong.Models;
using ASP_GCH1107_NguyenVanTruong.Data;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace ASP_GCH1107_NguyenVanTruong.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public object Session { get; private set; }

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var webApplication10Context = _context.Product.Include(p => p.Category);
        return View(await webApplication10Context.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Product == null)
        {
            return NotFound();
        }

        var product = await _context.Product
            .Include(b => b.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> AddToCart(CartModel model)
    {
        var cart = HttpContext.Session.GetString("cart");

        if (cart == null)
        {
            var product = _context.Product.Find(model.Id);

            if (product != null)
            {
                var listCart = new List<Cart>
            {
                new Cart
                {
                    Product = product,
                    Quantity = model.Quantity
                }
            };

                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(listCart));
            }
        }
        else
        {
            var dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart) ?? new List<Cart>();

            var existingCartItem = dataCart.FirstOrDefault(c => c.Product != null && c.Product.Id == model.Id);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += model.Quantity;
            }
            else
            {
                var product = _context.Product.Find(model.Id);

                if (product != null)
                {
                    dataCart.Add(new Cart
                    {
                        Product = product,
                        Quantity = model.Quantity
                    });
                }
            }

            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
        }

        return RedirectToAction("Cart");
    }

    public IActionResult Cart()
    {
        var cart = HttpContext.Session.GetString("cart");
        if (cart != null)
        {
            List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

            if (dataCart.Count > 0)
            {
                ViewBag.carts = dataCart;
                return View();
            }
            return RedirectToAction(nameof(Cart));
        }
        return RedirectToAction(nameof(Cart));
    }

    public async Task<IActionResult> RemoveCart(int id)
    {
        var cart = HttpContext.Session.GetString("cart");
        if (cart != null)
        {
            List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

            for (int i = 0; i < dataCart.Count; i++)
            {
                if (dataCart[i].Product.Id == id)
                {
                    dataCart.RemoveAt(i);
                }
            }
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));

            return RedirectToAction("Cart");
        }
        return RedirectToAction("Cart");
    }

    public IActionResult UpdateCart(CartModel model)
    {
        var cart = HttpContext.Session.GetString("cart");
        if (cart != null)
        {
            List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
            if (model.Quantity > 0)
            {
                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Product.Id == model.Id)
                    {
                        dataCart[i].Quantity = model.Quantity;
                    }
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
            }
            return RedirectToAction(nameof(Cart));
        }
        return BadRequest();
    }

    public async Task<IActionResult> StoreOrder()
    {
        var cart = HttpContext.Session.GetString("cart");

        if (cart != null)
        {
            List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

            foreach (var cartItem in dataCart)
            {
                Order newOrder = new Order
                {
                    Submitted = DateTime.Now,
                    TotalPrice = (int)(cartItem.Product.Price * cartItem.Quantity),
                    ProductId = cartItem.Product.Id
                };

                _context.Order.Add(newOrder);
            }

            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("cart");

            return RedirectToAction(nameof(ShowOrder));
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ShowOrder()
    {
        var ordersWithProducts = await _context.Order
            .Join(
                _context.Product,
                order => order.ProductId,
                product => product.Id,
                (order, product) => new
                {
                    Order = order,
                    Product = product
                }
            )
            .ToListAsync();

        ViewBag.OrdersWithProducts = ordersWithProducts;

        decimal totalOrderPrice = ordersWithProducts.Sum(op => op.Order.TotalPrice);
        ViewBag.TotalOrderPrice = totalOrderPrice;

        return View();
    }


}




