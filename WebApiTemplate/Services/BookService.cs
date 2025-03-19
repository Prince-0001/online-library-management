

using Microsoft.EntityFrameworkCore;
using WebApiTemplate.DTOs;
using WebApiTemplate.Exceptions;
using WebApiTemplate.Models;
using WebApiTemplate.Repository.Database;
using WebApiTemplate.Services.Interfaces;

namespace WebApiTemplate.Services
{
    public class BookService : IBookService
    {
        private readonly WenApiTemplateDbContext _context;

        public BookService(WenApiTemplateDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookDTO>> GetAllBooksAsync()
        {
            return await _context.Books
                .Select(book => new BookDTO
                {
                    Id = book.Id,
                    ISBN = book.ISBN,
                    Title = book.Title,
                    Author = book.Author,
                    Genres = book.BookGenres.Select(bg => bg.Genre!.Name).ToList(),
                    Publisher = book.Publisher,
                    PublicationYear = book.PublicationYear
                })
                .ToListAsync();
        }

        public async Task<BookDTO> GetBookByIdAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                throw new NotFoundException("Book not found");

            return new BookDTO
            {
                Id = book.Id,
                ISBN = book.ISBN,
                Title = book.Title,
                Author = book.Author,
                Genres = book.BookGenres.Select(bg => bg.Genre!.Name).ToList(),
                Publisher = book.Publisher,
                PublicationYear = book.PublicationYear
            };
        }

        public async Task<Review> CreateReviewAsync(Guid bookId, string userId, CreateReviewDTO reviewDto)
        {
            // Check if the book exists
            bool bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
            {
                throw new NotFoundException($"Book with ID {bookId} not found.");
            }

            // Check for duplicate review (optional based on your business logic)
            bool reviewExists = await _context.Reviews.AnyAsync(r => r.BookId == bookId && r.UserId == userId);
            if (reviewExists)
            {
                throw new InvalidOperationException("You have already reviewed this book.");
            }

            var review = new Review
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                UserId = userId,
                Content = reviewDto.Content,
                Rating = reviewDto.Rating,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }


        public async Task<BookDTO> UpdateBookAsync(Guid id, CreateBookDto bookDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                throw new NotFoundException("Book not found");

            book.ISBN = bookDto.ISBN;
            book.Title = bookDto.Title;
            book.Author = bookDto.Author;
            book.Publisher = bookDto.Publisher;
            book.PublicationYear = bookDto.PublicationYear;
            book.BookGenres.Clear();
            foreach (var genreId in bookDto.GenreIds)
            {
                var genre = await _context.Genres.FindAsync(genreId);
                if (genre != null)
                {
                    book.BookGenres.Add(new BookGenre { BookId = book.Id, GenreId = genre.Id });
                }
            }

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return new BookDTO
            {
                Id = book.Id,
                ISBN = book.ISBN,
                Title = book.Title,
                Author = book.Author,
                Genres = book.BookGenres.Select(bg => bg.Genre!.Name).ToList(),
                Publisher = book.Publisher,
                PublicationYear = book.PublicationYear
            };
        }

        public async Task<bool> DeleteBookAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                throw new NotFoundException("Book not found");
            

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BookDTO> CreateBookAsync(CreateBookDto bookDto)
        {
            // Check if ISBN already exists
            if (await _context.Books.AnyAsync(b => b.ISBN == bookDto.ISBN))
            {
                throw new InvalidOperationException("A book with the same ISBN already exists.");
            }

            // Map DTO to Book entity
            var book = new Book
            {
                Id = Guid.NewGuid(),
                ISBN = bookDto.ISBN,
                Title = bookDto.Title,
                Author = bookDto.Author,
                Publisher = bookDto.Publisher,
                PublicationYear = bookDto.PublicationYear
            };
            foreach (var genreId in bookDto.GenreIds)
            {
                var genre = await _context.Genres.FindAsync(genreId);
                if (genre != null)
                {
                    book.BookGenres.Add(new BookGenre { BookId = book.Id, GenreId = genre.Id });
                }
            }

            // Save to database
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Return as DTO
            return new BookDTO
            {
                Id = book.Id,
                ISBN = book.ISBN,
                Title = book.Title,
                Author = book.Author,
                Genres = book.BookGenres.Select(bg => bg.Genre!.Name).ToList(),
                Publisher = book.Publisher,
                PublicationYear = book.PublicationYear
            };
        }

        // Get books by author
        public async Task<List<Book>> GetBooksByAuthorAsync(string author)
        {
            return await _context.Books
                .Where(b => b.Author.ToLower().Contains(author.ToLower()))
                .ToListAsync();
        }

        // Get books by publisher
        public async Task<List<Book>> GetBooksByPublisherAsync(string publisher)
        {
            return await _context.Books
                .Where(b => b.Publisher.ToLower().Contains(publisher.ToLower()))
                .ToListAsync();
        }

        // Get book details with reviews and average rating
        public async Task<object?> GetBookDetailsAsync(Guid bookId)
        {
            var book = await _context.Books
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null) return null;

            var averageRating = book.Reviews.Any() ? book.Reviews.Average(r => r.Rating) : 0;

            return new
            {
                book.Id,
                book.ISBN,
                book.Title,
                book.Author,
                Genres = book.BookGenres.Select(bg => bg.Genre?.Name).ToList(),
                book.Publisher,
                book.PublicationYear,
                AverageRating = averageRating,
                Reviews = book.Reviews.Select(r => new
                {
                    r.Id,
                    r.Content,
                    r.Rating,
                    r.UserId,
                    r.CreatedAt
                })
            };
        }

        public async Task<PagedResult<BookDTO>> GetBooksAsync(BookFilterParams filterParams)
        {
            var query = _context.Books.AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(filterParams.Title))
                query = query.Where(b => b.Title.Contains(filterParams.Title));

            if (!string.IsNullOrEmpty(filterParams.Author))
                query = query.Where(b => b.Author.Contains(filterParams.Author));

            if (!string.IsNullOrEmpty(filterParams.Publisher))
                query = query.Where(b => b.Publisher.Contains(filterParams.Publisher));

            if (filterParams.PublicationYear.HasValue)
                query = query.Where(b => b.PublicationYear == filterParams.PublicationYear.Value);

            if (filterParams.Genres != null && filterParams.Genres.Any())
            {
                query = query.Where(b => b.BookGenres.Any(bg => filterParams.Genres.Contains(bg.Genre!.Name)));
            }

            // Total Count before Pagination
            int totalCount = await query.CountAsync();

            // Pagination
            var books = await query
                .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
                .Take(filterParams.PageSize)
                .Select(book => new BookDTO
                {
                    Id = book.Id,
                    ISBN = book.ISBN,
                    Title = book.Title,
                    Author = book.Author,
                    Genres = book.BookGenres.Select(bg => bg.Genre!.Name).ToList(),
                    Publisher = book.Publisher,
                    PublicationYear = book.PublicationYear
                })
                .ToListAsync();

            return new PagedResult<BookDTO>
            {
                Items = books,
                TotalCount = totalCount,
                PageNumber = filterParams.PageNumber,
                PageSize = filterParams.PageSize
            };
        }


        }
}

