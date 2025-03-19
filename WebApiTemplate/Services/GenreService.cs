using WebApiTemplate.DTOs.Genre;
using WebApiTemplate.Models;
using WebApiTemplate.Repository.Database;
using Microsoft.EntityFrameworkCore;

namespace WebApiTemplate.Services
{
    public class GenreService : IGenreService
    {
        private readonly WenApiTemplateDbContext _context;

        public GenreService(WenApiTemplateDbContext context)
        {
            _context = context;
        }

        // ✅ Create Genre
        public async Task<GenreDto> CreateGenreAsync(CreateGenreDTO genreDto)
        {
            if (await _context.Genres.AnyAsync(g => g.Name.ToLower() == genreDto.Name.ToLower()))
                throw new InvalidOperationException("Genre already exists.");

            var genre = new Genre
            {
                Name = genreDto.Name
            };

            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            return new GenreDto { Id = genre.Id, Name = genre.Name };
        }

        // ✅ Update Genre
        public async Task<GenreDto> UpdateGenreAsync(Guid genreId, UpdateGenreDTO genreDto)
        {
            var genre = await _context.Genres.FindAsync(genreId);
            if (genre == null)
                throw new KeyNotFoundException("Genre not found.");

            genre.Name = genreDto.Name;
            await _context.SaveChangesAsync();

            return new GenreDto { Id = genre.Id, Name = genre.Name };
        }

        // ✅ Delete Genre
        public async Task<bool> DeleteGenreAsync(Guid genreId)
        {
            var genre = await _context.Genres.FindAsync(genreId);
            if (genre == null)
                throw new KeyNotFoundException("Genre not found.");

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return true;
        }

        // ✅ Get All Genres
        public async Task<List<GenreDto>> GetAllGenresAsync()
        {
            return await _context.Genres
                .Select(g => new GenreDto { Id = g.Id, Name = g.Name })
                .ToListAsync();
        }

        // ✅ Get Genre by Id
        public async Task<GenreDto> GetGenreByIdAsync(Guid genreId)
        {
            var genre = await _context.Genres.FindAsync(genreId);
            if (genre == null)
                throw new KeyNotFoundException("Genre not found.");

            return new GenreDto { Id = genre.Id, Name = genre.Name };
        }

        // ✅ Tag Genres to Book
        public async Task<bool> TagGenresAsync(TagGenreDto tagGenreDto)
        {
            var book = await _context.Books
                .Include(b => b.BookGenres)
                .FirstOrDefaultAsync(b => b.Id == tagGenreDto.BookId);

            if (book == null)
                throw new KeyNotFoundException("Book not found.");

            var genres = await _context.Genres
                .Where(g => tagGenreDto.GenreIds.Contains(g.Id))
                .ToListAsync();

            if (!genres.Any())
                throw new InvalidOperationException("Invalid Genre Id(s) provided.");

            foreach (var genre in genres)
            {
                if (!book.BookGenres.Any(bg => bg.GenreId == genre.Id))
                {
                    book.BookGenres.Add(new BookGenre { BookId = book.Id, GenreId = genre.Id });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // ✅ Untag Genres from Book
        public async Task<bool> UntagGenresAsync(TagGenreDto tagGenreDto)
        {
            var book = await _context.Books
                .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre)
                .FirstOrDefaultAsync(b => b.Id == tagGenreDto.BookId);

            if (book == null)
                throw new KeyNotFoundException("Book not found.");

            book.BookGenres.RemoveAll(bg => tagGenreDto.GenreIds.Contains(bg.GenreId));

            await _context.SaveChangesAsync();
            return true;
        }


       

       
    }
}
