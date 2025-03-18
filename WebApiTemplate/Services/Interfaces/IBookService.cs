
using WebApiTemplate.DTOs;
using WebApiTemplate.Models;

namespace WebApiTemplate.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDTO>> GetBooksAsync();
        Task<BookDTO> GetBookByIdAsync(Guid id);
        Task<BookDTO> CreateBookAsync(CreateBookDto bookDto);
        Task<BookDTO> UpdateBookAsync(Guid id, CreateBookDto bookDto);
        Task<bool> DeleteBookAsync(Guid id);
        Task<List<Book>> GetBooksByPublisherAsync(string publisher);
        Task<List<Book>> GetBooksByAuthorAsync(string author);
        Task<object?> GetBookDetailsAsync(Guid bookId);
    }
}
