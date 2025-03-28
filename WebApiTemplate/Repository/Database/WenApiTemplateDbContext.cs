﻿using Humanizer;
using System.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiTemplate.Models;

namespace WebApiTemplate.Repository.Database
{
    public class WenApiTemplateDbContext : IdentityDbContext<IdentityUser>
    {
        public WenApiTemplateDbContext(DbContextOptions<WenApiTemplateDbContext> options) : base(options) { }

        // DbSet for Book and Review Models
        public DbSet<Book> Books { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public DbSet<BookGenre> BookGenres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Ensure Identity Models are created

            // Book Indexing
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.Title);

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.Author);

            // Review Configuration
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent user deletion from cascading

            modelBuilder.Entity<Review>()
                .HasIndex(r => r.Rating); // Optional: Index rating for faster lookups

            // Unique Review Constraint (User can review a book only once)
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.BookId })
                .IsUnique();

            // Many - to - Many Configuration for Book and Genre
            modelBuilder.Entity<BookGenre>()
                .HasKey(bg => new { bg.BookId, bg.GenreId });

            modelBuilder.Entity<BookGenre>()
                .HasOne(bg => bg.Book)
                .WithMany(b => b.BookGenres)
                .HasForeignKey(bg => bg.BookId);

            modelBuilder.Entity<BookGenre>()
                .HasOne(bg => bg.Genre)
                .WithMany(g => g.BookGenres)
                .HasForeignKey(bg => bg.GenreId);

            // Unique constraint on Genre Name
            modelBuilder.Entity<Genre>()
                .HasIndex(g => g.Name)
                .IsUnique();
        }
    }
}
