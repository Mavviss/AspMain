using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net;
using Microsoft.EntityFrameworkCore.Update;

namespace Eshop.Controllers
{
    public class AccountsController : Controller
    {
        private readonly EshopContext _context;
        private readonly IWebHostEnvironment _environment;
        public AccountsController(EshopContext context)
        {
            _context = context;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            var accounts = _context.Accounts.ToList();
            return View(accounts);  
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        public async Task<IActionResult> UserDetails()
        {

            var ID = HttpContext.Session.GetInt32("UserId");
            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == ID );
    

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult UserCreate()
        {
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,Email,Phone,Address,FullName,IsAdmin,Avatar,Status")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCreate([Bind("Id,Username,Password,Email,Phone,Address,FullName,Avatar,IsAdmin,Status")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }
        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }
        public async Task<IActionResult> UserEdit(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }
        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,Email,Phone,Address,FullName,IsAdmin,Avatar,Status")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
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
            return View(account);
        }
      
        public async Task<IActionResult> UserEdit(int id, [Bind("Id,Address,Avatar,Phone,Password,Email,FullName,UserName")] Account account, IFormFile file)

        {
            if (id != account.Id)
            {
                return NotFound();
            }
            var accountID = await _context.Accounts.FindAsync(id);

            if (account.Avatar != null)
            {
                var fileName = account.Id.ToString() + Path.GetExtension(account.ImageFile.FileName);
                var uploadFolder = Path.Combine(_environment.WebRootPath, "images", "avatar");
                var uploadPath = Path.Combine(uploadFolder, fileName);
                using (FileStream fs = System.IO.File.Create(uploadPath))
                {
                    account.ImageFile.CopyTo(fs);
                    fs.Flush();
                }
                accountID.Avatar = fileName;

            }
            accountID.Status = account.Status;
            accountID.Username = account.Username;
            accountID.Password = account.Password;
            accountID.Email = account.Email;
            accountID.Address = account.Address;
            accountID.Phone= account.Phone;
            accountID.FullName= account.FullName;
            _context.Accounts.Update(accountID);
            _context.SaveChanges();
            return RedirectToAction("UserDetails", "Account");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Accounts == null)
            {
                return Problem("Entity set 'EshopContext.Account'  is null.");
            }
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                
                _context.Accounts.Remove(account);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
          return _context.Accounts.Any(e => e.Id == id);
        }
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Login([Bind("Username,Password")] Account account)
		{
            var accounts = _context.Accounts.FirstOrDefault(a => (account.Username == a.Username && account.Password == a.Password));
            
            if (accounts!=null)
			{
                HttpContext.Session.SetString("User", accounts.FullName);

                HttpContext.Session.SetInt32("UserId", accounts.Id);

                if (accounts.IsAdmin == true)
                {
					HttpContext.Session.SetString("IdUser","admin");
					return RedirectToAction("Index", "Admin", "account");
				}
                
				return RedirectToAction("Index","Products");
			}
			else
			{
				ViewBag.ErrorMsg = "Login failed!";
				return View();
			}
		}
		public ActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Index", "Products");
		}


	}
}
