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
    public class PersonsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<MHUser> _manager;
        private readonly IHostingEnvironment _environment;

        public PersonsController(ApplicationDbContext context, UserManager<MHUser> manager, IHostingEnvironment environment)
        {
            _context = context;
            _manager = manager;
            _environment = environment;
        }
        
        
        public async Task<IActionResult> Index()
        {
            return View(await _context.Persons.ToListAsync());
        }
        [Authorize]
        public IActionResult Create()
        {
            return View(new Person());
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Img,Name,Biog")] Person person)
        {
            ModelState.Clear();
            TryValidateModel(person);
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                foreach (var image in files)
                {
                    if (image != null && image.Length > 0)
                    {
                        var file = image;
                        var uploads = Path.Combine(_environment.ContentRootPath, "wwwroot", "images", "persons");

                        if (file.Length > 0)
                        {
                            await using (var fileStream =
                                new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                person.Img = file.FileName;
                            }
                        }
                    }
                }
                _context.Add(person);
                await _context.SaveChangesAsync();
            }
            return View("Details", person);
        }
        public IActionResult Details(int id)
        {
            var person= _context.Persons.FirstOrDefault(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            return View("Details", person);
        }
        
        
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var person = await _context.Persons.FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }
        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.Id == id);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Person person)
        {
            if (id != person.Id)
            {
                return NotFound();
            }
            var oldPerson = await _context.Persons.FirstOrDefaultAsync(p => p.Id == id);

            _context.Entry(oldPerson).State = EntityState.Detached;
            
            ModelState.Clear();
            TryValidateModel(person);
            
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
                            var uploads = Path.Combine(_environment.ContentRootPath, "wwwroot", "images", "persons");

                            if (file.Length > 0)
                            {
                                await using var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create);
                                await file.CopyToAsync(fileStream);
                                person.Img = file.FileName;
                            }
                        }
                    }
                    
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View("Details", person);
        }
        
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            
            return View(person);
        }
        [Authorize]
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.Persons.FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}