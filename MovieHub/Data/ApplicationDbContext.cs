using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieHub.Areas.Identity;
using MovieHub.Models;
using MovieHub.Models.CrossRefModels;

namespace MovieHub.Data
{
    public class ApplicationDbContext : IdentityDbContext<MHUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Movie - Category
            modelBuilder.Entity<MovieCategory>()
                .HasKey(t => new {t.MovieId, t.CategoryId});
            
            modelBuilder.Entity<MovieCategory>()
                .HasOne(mc => mc.Movie)
                .WithMany(mc => mc.MovieCategories)
                .HasForeignKey(mc => mc.MovieId);

            modelBuilder.Entity<MovieCategory>()
                .HasOne(mc => mc.Category)
                .WithMany(mc => mc.MovieCategories)
                .HasForeignKey(mc => mc.CategoryId);
            
            
            // Movie - Person
            
            modelBuilder.Entity<MoviePerson>()
                .HasKey(t => new {t.MovieId, t.PersonId});
            
            modelBuilder.Entity<MoviePerson>()
                .HasOne(mp => mp.Movie)
                .WithMany(mp => mp.MoviePersons)
                .HasForeignKey(mp => mp.MovieId);

            modelBuilder.Entity<MoviePerson>()
                .HasOne(mp => mp.Person)
                .WithMany(mp => mp.MoviePersons)
                .HasForeignKey(mp => mp.PersonId);

            modelBuilder.Entity<Post>()
                .HasMany(c => c.Comments)
                .WithOne(p => p.Post); 
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbSet<MovieCategory> MovieCategories { get; set; }
        public DbSet<MoviePerson> MoviePersons { get; set; }
    }
}