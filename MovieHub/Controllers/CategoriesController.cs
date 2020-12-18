using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MovieHub.Areas.Identity;
using MovieHub.Data;
using MovieHub.Models;

namespace MovieHub.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IHostingEnvironment _environment;
        private readonly UserManager<MHUser> _manager;
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context, UserManager<MHUser> manager, IHostingEnvironment environment)
        {
            _context = context;
            _manager = manager;
            _environment = environment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }
        
        [Authorize]
        public IActionResult Create()
        {
            return View(new Category());
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            ModelState.Clear();
            TryValidateModel(category);
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                foreach (var image in files)
                {
                    if (image != null && image.Length > 0)
                    {
                        var file = image;
                        var uploads = Path.Combine(_environment.ContentRootPath, "wwwroot", "images", "categories");

                        if (file.Length <= 0) continue;
                        await using var fileStream =
                            new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create);
                        await file.CopyToAsync(fileStream);
                        category.Img = file.FileName;
                    }
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
            }
            return View("Details", category);
        }
        public IActionResult Details(int id)
        {
            var category = _context.Categories.FirstOrDefault(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View("Details", category);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            var oldCategory = await _context.Categories.FirstOrDefaultAsync(p => p.Id == id);

            _context.Entry(oldCategory).State = EntityState.Detached;
            
            ModelState.Clear();
            TryValidateModel(category);
            
            if (ModelState.IsValid)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;
                    foreach (var image in files)
                    {
                        if (image != null && image.Length > 0)
                        {
                            var file = image;
                            var uploads = Path.Combine(_environment.ContentRootPath, "wwwroot", "images", "categories");

                            if (file.Length > 0)
                            {
                                await using var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create);
                                await file.CopyToAsync(fileStream);
                                category.Img = file.FileName;
                            }
                        }
                    }
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View("Details", category);
        }
        
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category= await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            
            return View(category);
        }
        [Authorize]
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}