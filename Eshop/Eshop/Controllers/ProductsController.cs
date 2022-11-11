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
using Eshop.Helpers;
using Microsoft.Extensions.Hosting;

namespace Eshop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly EshopContext _context;
        public const string CartSessionKey = "CartId";
		private readonly IWebHostEnvironment _environment;
		public ProductsController(EshopContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {

            var products = _context.Products.ToList();

            ViewBag.ProducTypes = _context.ProductTypes.ToList();

            ViewBag.idUser = HttpContext.Session.GetString("IdUser");
            ViewBag.User = HttpContext.Session.GetString("User");

            return View(products);

        }

        public async Task<IActionResult> Homeadmin()
        {

            return View();

        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {

         

            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

		// GET: Products/Create
		public IActionResult Create()
        {
           

            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name");
			return View();
		}

		// POST: Products/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,SKU,Name,Description,Price,Stock,ProductTypeId,ImageFile,Status")] Product product)
		{
          

            if (product !=null)
			{
				_context.Add(product);
				await _context.SaveChangesAsync();

				if (product.ImageFile != null)
				{
					var fileName = product.Id.ToString() + Path.GetExtension(product.ImageFile.FileName);
					var uploadFolder = Path.Combine(_environment.WebRootPath, "images", "product");
					var uploadPath = Path.Combine(uploadFolder, fileName);
					using (FileStream fs = System.IO.File.Create(uploadPath))
					{
						product.ImageFile.CopyTo(fs);
						fs.Flush();
					}
					product.Image = fileName;
					_context.Products.Update(product);
					_context.SaveChanges();
				}

				
			}
			ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
			//return View(product);
            return RedirectToAction("ListProducts", "Admin", product);
		}

		// GET: Products/Edit/5
		public async Task<IActionResult> Edit(int? id)
        {

            if (id == null || _context.Products == null)
			{
				return NotFound();
			}

			var product = await _context.Products.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}
			ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
			return View(product);
		}

		// POST: Products/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,SKU,Name,Description,Price,Stock,ProductTypeId,ImageFile,Image,Status")] Product product, IFormFile file)
		{

            if (id != product.Id)
			{
				return NotFound();
			}
			var productedit = await _context.Products.FindAsync(id);

			if (product.ImageFile != null)
					{
						var fileName = product.Id.ToString() + Path.GetExtension(product.ImageFile.FileName);
						var uploadFolder = Path.Combine(_environment.WebRootPath, "images", "product");
						var uploadPath = Path.Combine(uploadFolder, fileName);
						using (FileStream fs = System.IO.File.Create(uploadPath))
						{
							product.ImageFile.CopyTo(fs);
							fs.Flush();
						}
							productedit.Image = fileName;
						
					}
			productedit.SKU = product.SKU;
			productedit.Description = product.Description;
			productedit.Price = product.Price;
			productedit.Name =product.Name;
			productedit.Stock = product.Stock;
			productedit.ProductTypeId = product.ProductTypeId;
			productedit.Status = product.Status;
			_context.Products.Update(productedit);
			_context.SaveChanges();
			ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
			return RedirectToAction("ListProducts", "Admin");
		}

		// GET: Products/Delete/5
		public async Task<IActionResult> Delete(int? id)
        {

            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

			var product = await _context.Products.FindAsync(id);
			if (product != null)
			{
				_context.Products.Remove(product);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction("ListProducts", "Admin");
		}

        //// POST: Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Products == null)
        //    {
        //        return Problem("Entity set 'EshopContext.Products'  is null.");
        //    }
        //    var product = await _context.Products.FindAsync(id);
        //    if (product != null)
        //    {
        //        _context.Products.Remove(product);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("ListProducts","Admin");
        //}
        //[HttpPost]
        public async Task<IActionResult> Search(String SearchString)
        {
            ViewBag.ProducTypes = _context.ProductTypes.ToList();

            if (!String.IsNullOrEmpty(SearchString))
            {
				var product = _context.Products.Where(u => u.Name.Contains(SearchString));
				return View("Index" ,product);
            }
			return View("Index");
        }

        public IActionResult Searchtype(string name, int producttype)
        {
            ViewBag.ProducTypes = _context.ProductTypes.ToList();

            var type = _context.Products.Where(u => u.ProductTypeId == producttype).ToList();
			return View("Index",type);
			
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
