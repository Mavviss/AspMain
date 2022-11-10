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
using static System.Net.WebRequestMethods;

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


        public IActionResult Purchase()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Purchase(string Address, string Phone)
        {
            var id = HttpContext.Session.GetInt32("UserId");
            //include account product vao cart
            var carts = _context.Carts.Include(c => c.Product).Include(c => c.Account).Where(u => u.Account.Id == id);
            var accountid = _context.Accounts.FirstOrDefault(u => u.Id == id);
            var total = carts.Sum(u => u.Product.Price * u.Quantity);
            //thêm  hóa đơn 
            var invoice = new Invoice
            {
                Code = DateTime.Now.ToString("yyyyMMddhhmmss"),
                AccountId = accountid.Id,
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

            return View();

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
