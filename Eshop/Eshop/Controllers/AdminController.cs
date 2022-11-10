using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Controllers
{
    public class AdminController : Controller
	{
		private readonly EshopContext _context;

		public AdminController(EshopContext context)
		{
			_context = context;
		}
		public IActionResult Index()
        {
            return View();
        }
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
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
		public IActionResult ListProducts()
		{
			var products = _context.Products.ToList();
			return View(products);
		}
	}
}
