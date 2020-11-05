using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Markdig.Renderers.Html;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MovieHub.Areas.Identity;
using MovieHub.Data;
using MovieHub.Models;
using MovieHub.Models.CrossRefModels;

namespace MovieHub.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<MHUser> _manager;
        private readonly IHostingEnvironment _environment;

        public MoviesController(ApplicationDbContext context, UserManager<MHUser> manager, IHostingEnvironment environment)
        {
            _context = context;
            _manager = manager;
            _environment = environment;
        }
        
        
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }
        [Authorize]
        public IActionResult CreateMovie()
        {
            return View(new Movie());
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMovie([Bind("Id,Title,Text,Poster")] Movie movie)
        {
            ModelState.Clear();
            TryValidateModel(movie);
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                foreach (var image in files)
                {
                    if (image != null && image.Length > 0)
                    {
                        var file = image;
                        var uploads = Path.Combine(_environment.ContentRootPath, "wwwroot", "images", "movies");

                        if (file.Length > 0)
                        {
                            await using (var fileStream =
                                new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                movie.Poster = file.FileName;
                            }
                        }
                    }
                }
                //movie.FormatedText = Markdig.Markdown.ToHtml(post.Text);
                _context.Add(movie);
                await _context.SaveChangesAsync();
            }

            return View("Details", movie);
        }
        public IActionResult Details(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return View("Details", movie);
        }
        
        
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }
            var oldMovie = await _context.Movies.FirstOrDefaultAsync(p => p.Id == id);

            _context.Entry(oldMovie).State = EntityState.Detached;
            
            ModelState.Clear();
            TryValidateModel(movie);
            
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
                            var uploads = Path.Combine(_environment.ContentRootPath, "wwwroot", "images", "movies");

                            if (file.Length > 0)
                            {
                                await using var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create);
                                await file.CopyToAsync(fileStream);
                                movie.Poster = file.FileName;
                            }
                        }
                    }
                    
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View("Details", movie);
        }
        
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            
            return View(movie);
        }
        [Authorize]
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddCategory(string categoryId)
        {
            var split = categoryId.Split(",");
            var idCategory = int.Parse(split[0]);
            var idMovie = int.Parse(split[1]);
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == idMovie);
            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == idCategory);
            
            var movieCategory = new MovieCategory()
            {
                 CategoryId = idCategory, MovieId = idMovie,
                 Category = category, Movie = movie
            };
            movie.MovieCategories.Add(movieCategory);
            _context.Add(movieCategory);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> AddPerson(string personId)
        {
            var split = personId.Split(",");
            var idPerson = int.Parse(split[0]);
            var idMovie = int.Parse(split[1]);
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == idMovie);
            var person = await _context.Persons.FirstOrDefaultAsync(m => m.Id == idPerson);
            
            var moviePerson = new MoviePerson()
            {
                PersonId = idPerson, MovieId = idMovie,
                Person = person, Movie = movie
            };
            movie.MoviePersons.Add(moviePerson);
            _context.Add(moviePerson);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<string> GetCategory(int categoryId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            return category.Name;
        }
        private async Task<MHUser> GetCurrentUser()
        {
            return await _manager.GetUserAsync(HttpContext.User);
        }
        private async Task<IList<string>> GetUserRoles(MHUser user)
        {
            return await _manager.GetRolesAsync(user);
        }

        private async Task<bool> IsAdmin(MHUser user)
        {
            var roles = await GetUserRoles(user);
            return roles.Any(s => s == "Admin");
        }
        [HttpPost]
        [Authorize]
        [Route("Movies/Details/{idMovie?}/Review")]
        public async Task<IActionResult> AddReview(int idMovie, Review review)
        {
            var user = await GetCurrentUser();
            Console.WriteLine(user);
            if (ModelState.IsValid)
            {
                Console.WriteLine("hell");
                review.Author = user;
                Console.WriteLine(review.Author);
                review.DoC = DateTime.Now;
                review.FormatedText = Markdig.Markdown.ToHtml(review.Text);
                _context.Add(review);
                await _context.SaveChangesAsync();
                
            }
            return RedirectToAction(nameof(Details), new {id = idMovie});
        }
    }
}