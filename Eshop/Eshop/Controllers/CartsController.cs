using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eshop.Data;
using Eshop.Models;
using System.Net;
using Eshop.Helpers;
using Microsoft.CodeAnalysis;
using NuGet.Protocol;
using System.Xml.Linq;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace Eshop.Controllers
{
    public class CartsController : Controller
    {
        private readonly EshopContext _context;

        public CartsController(EshopContext context)
        {
            _context = context;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var eshopContext = _context.Carts.Include(c => c.Account).Include(c => c.Product).Where(u => u.Account.Id == id);
            return View(await eshopContext.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Account)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccountId,ProductId,Quantity")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", cart.ProductId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountId,ProductId,Quantity")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", cart.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", cart.ProductId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Account)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Carts == null)
            {
                return Problem("Entity set 'EshopContext.Carts'  is null.");
            }
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //public List<Cart> carts
        //{
        //    get
        //    {
        //        var data = HttpContext.Session.Get<List<Cart>>("Giohang");
        //        if (data == null)
        //        {
        //            data = new List<Cart>();
        //        }
        //        return data;
        //    }
        //}

        //public IActionResult AddToCart(int Id)
        //{
        //    //lấy id thằng đăng nhập
        //    var id = HttpContext.Session.GetInt32("UserId");
        //    //lấy giỏ hàng của thằng đăng nhập
        //    var carts = _context.Carts.Include(c => c.Product).Include(c => c.Account).Where(u => u.Account.Id == id).ToList();
        //    //kiểm tra các sản phẩm trong giỏ hàng
        //    var item = _context.Carts.Include(c => c.Product).SingleOrDefault(u => u.AccountId == id);
        //    var item2 = carts.SingleOrDefault(u => u.ProductId == Id);
        //    var productDetail = _context.Products.SingleOrDefault(u => u.Id == Id);

        //    if (item == null)
        //    {
        //        item = new Cart
        //        {

        //            ProductId = Id,
        //            Quantity = 1,
        //            AccountId = item.AccountId,
        //        };
        //        carts.Add(item);
        //    }
        //    else
        //    {
        //        item2.Quantity++;
        //    }
        //    _context.SaveChanges();

        //    HttpContext.Session.Set("Giohang", carts);

        //    return RedirectToAction("Index","Carts");
        //}
        public IActionResult AddToCart(int Id)
        {
            //lấy id thằng đăng nhập
            int? id = HttpContext.Session.GetInt32("UserId");
            //lấy giỏ hàng của thằng đăng nhập
            //var carts = _context.Carts.Include(c => c.Product).Include(c => c.Account).Where(u => u.Account.Id == id).ToList();
            //kiểm tra các sản phẩm trong giỏ hàng
            var item = _context.Carts.Include(c => c.Product).FirstOrDefault(u => u.AccountId == id);
            var item2 = _context.Carts.FirstOrDefault(u => u.ProductId == Id && u.AccountId == id);
            var productDetail = _context.Products.FirstOrDefault(u => u.Id == Id);

            if (item != null)
            {
                if (item2 == null)
                {
                    var item3 = new Cart();

                    item3.AccountId = id.Value;
                    item3.ProductId = Id;
                    item3.Quantity = 1;

                    _context.Carts.Add(item3);
                }
                else
                {
                    item2.Quantity++;
                }

            }
            else
            {
                //tạo mới đối tượng cart item

                var item3 = new Cart();

                item3.AccountId = id.Value;
                item3.ProductId = Id;
                item3.Quantity = 1;

                _context.Carts.Add(item3);
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Carts");
        }

        public IActionResult Purchase()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Purchase(string Address, string Phone)
        {

            //if (string.IsNullOrEmpty(Address) && string.IsNullOrEmpty(Phone))
            //{
            //    ViewBag.msg = "Ko có gì đẻ thanh toán cả";
            //    return RedirectToAction("Index", "Carts");
            //}

            //thêm  hóa đơn 
            if (ModelState.IsValid)
            {
                ViewBag.errormsg = "Ko có gì đẻ thanh toán cả";

                return RedirectToAction("Index", "Products");
            }
            else
            {
                var id = HttpContext.Session.GetInt32("UserId");
                //include account product vao cart
                var carts = _context.Carts.Include(c => c.Product).Include(c => c.Account).Where(u => u.Account.Id == id);
                var _account = _context.Accounts.FirstOrDefault(u => u.Id == id);
                var total = carts.Sum(u => u.Product.Price * u.Quantity);
                var invoice = new Invoice
                {
                    Code = DateTime.Now.ToString("yyyyMMddhhmmss"),
                    AccountId = _account.Id,
                    IssuedDate = DateTime.Now,
                    ShippingAddress = Address,
                    ShippingPhone = Phone,
                    Total = total,
                    Status = true,

                };
                _context.Invoices.Add(invoice);
                _context.SaveChanges();


                //thêm vào chi tiết hóa đơn
                foreach (var item in carts)
                {
                    InvoiceDetail detail = new InvoiceDetail
                    {
                        InvoiceId = invoice.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                    };
                    _context.InvoiceDetails.Add(detail);

                    //giảm số lượng sản phẩm trong product khi thanh toán
                    item.Product.Stock -= item.Quantity;
                    _context.Products.Update(item.Product);
                    _context.Carts.Remove(item);
                }
                _context.SaveChanges();
                ViewBag.msg = "Đặt hàng thành công!";
            }
            return RedirectToAction("Index", "Products");

        }

        public IActionResult DeteleAll()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            //include account product vao cart
            var carts = _context.Carts.Include(c => c.Product).Include(c => c.Account).Where(u => u.Account.Id == id);
            var accountid = _context.Accounts.FirstOrDefault(u => u.Id == id);
            var total = carts.Sum(u => u.Product.Price * u.Quantity);
            foreach (var item in carts)
            {
                _context.Carts.Remove(item);
            }
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }
    }
}
