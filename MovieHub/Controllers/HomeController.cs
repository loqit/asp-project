using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieHub.Areas.Identity;
using MovieHub.Data;
using MovieHub.Models;


namespace MovieHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<MHUser> _manager;
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _environment;

        public HomeController(ILogger<HomeController> logger, UserManager<MHUser> manager, ApplicationDbContext context
        , IHostingEnvironment environment)
        {
            _logger = logger;
            _manager = manager;
            _context = context;
            _environment = environment;
        }
        [Authorize]
        public IActionResult CreatePost()
        {
            return View(new Post());
        }
        
        private async Task<MHUser> GetCurrentUser()
        {
            return await _manager.GetUserAsync(HttpContext.User);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost([Bind("Id,Title,Text,Img,Author")] Post post)
        {
            var author = await GetCurrentUser();
            Console.WriteLine(author);
            post.Author = author;
            Console.WriteLine(post.Author);
            post.DOC = DateTime.UtcNow;
            
            ModelState.Clear();
            TryValidateModel(post);
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                foreach (var image in files)
                {
                    if (image != null && image.Length > 0)
                    {
                        var file = image;
                        var uploads = Path.Combine(_environment.ContentRootPath, "wwwroot", "images", "posts");

                        if (file.Length > 0)
                        {
                            await using var fileStream =
                                new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create);
                            await file.CopyToAsync(fileStream);
                            post.Img = file.FileName;
                        }
                    }
                }
                post.FormatedText = Markdig.Markdown.ToHtml(post.Text);
                _context.Add(post);
                await _context.SaveChangesAsync();
            }

            return View("Details", post);
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Posts.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public IActionResult Details(int id)
        {
            var post = _context.Posts.FirstOrDefault(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            return View("Details",post);
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
        
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Console.WriteLine("1");
            var user = await GetCurrentUser();
            Console.WriteLine("2");
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            //Console.WriteLine(post.Author);
            if (post == null)
            {
                return NotFound();
            }
            Console.WriteLine("Author");
            Console.WriteLine(post.Author);
            Console.WriteLine("User");
            Console.WriteLine(user);
            
            if (post.Author != user && !(await IsAdmin(user)))
            {
                return NotFound();
            }
            Console.WriteLine("4");
            return View(post);
        }
        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post)
        {
            var user = await GetCurrentUser();
            if (id != post.Id)
            {
                return NotFound();
            }
            var oldPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            post.Author = oldPost.Author;

            _context.Entry(oldPost).State = EntityState.Detached;
            
            if (post.Author != user && !await IsAdmin(user))
            {
                return NotFound();
            }
            ModelState.Clear();
            TryValidateModel(post);
            
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
                            var uploads = Path.Combine(_environment.ContentRootPath, "wwwroot", "images", "posts");

                            if (file.Length > 0)
                            {
                                await using var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create);
                                await file.CopyToAsync(fileStream);
                                post.Img = file.FileName;
                            }
                        }
                    }

                    post.DOC = DateTime.UtcNow;
                    post.FormatedText = Markdig.Markdown.ToHtml(post.Text);
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View("Details", post);
        }
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUser();
            if (post.Author != user && !await IsAdmin(user))
            {
                return NotFound();
            }

            return View(post);
        }
        [Authorize]
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await GetCurrentUser();
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            if (post.Author != user && !await IsAdmin(user))
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        
        [HttpPost]
        [Route("Home/MarkdownToHtml")]
        public string MarkdownToHtml()
        {
            Console.WriteLine("hell");
            using var reader = new StreamReader(Request.Body);
            var body = reader.ReadToEnd();
            var html = Markdig.Markdown.ToHtml(body);
            return html;

        }
        [HttpPost]
        [Authorize]
        [Route("Home/Details/{idPost?}/Comment")]
        public async Task<IActionResult> CreateComment(int idPost, Comment comment)
        {
            var user = await GetCurrentUser();
            comment.Author = user;
            comment.DOC = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                Console.WriteLine("hell");
            }
            
            return RedirectToAction(nameof(Details), new {id = idPost});
        }
        
    }
}