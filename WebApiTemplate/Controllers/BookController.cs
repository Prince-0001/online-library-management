using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiTemplate.DTOs;
using WebApiTemplate.Exceptions;
using WebApiTemplate.Services.Interfaces;

namespace WebApiTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IValidator<CreateBookDto> _validator;

        public BookController(IBookService bookService, IValidator<CreateBookDto> validator)
        {
            _bookService = bookService;
            _validator = validator;
        }

        // GET: api/book
        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        // GET: api/book/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                return Ok(book);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/book
        [HttpPost]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDto bookDto)
        {
            var validationResult = await _validator.ValidateAsync(bookDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var createdBook = await _bookService.CreateBookAsync(bookDto);
            return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook);
        }

        // PUT: api/book/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] CreateBookDto bookDto)
        {
            var validationResult = await _validator.ValidateAsync(bookDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                var updatedBook = await _bookService.UpdateBookAsync(id, bookDto);
                return Ok(updatedBook);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/book/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            try
            {
                await _bookService.DeleteBookAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET: api/Book/Author/{author}
        [HttpGet("Author/{author}")]
        public async Task<IActionResult> GetBooksByAuthor(string author)
        {
            var books = await _bookService.GetBooksByAuthorAsync(author);
            if (!books.Any())
                return NotFound($"No books found by author: {author}");
            return Ok(books);
        }

        // GET: api/Book/Publisher/{publisher}
        [HttpGet("Publisher/{publisher}")]
        public async Task<IActionResult> GetBooksByPublisher(string publisher)
        {
            var books = await _bookService.GetBooksByPublisherAsync(publisher);
            if (!books.Any())
                return NotFound($"No books found by publisher: {publisher}");
            return Ok(books);
        }

        // GET: api/Book/{bookId}
        [HttpGet("detail/{bookId}")]
        public async Task<IActionResult> GetBookDetails(Guid bookId)
        {
            var bookDetails = await _bookService.GetBookDetailsAsync(bookId);
            if (bookDetails == null)
                return NotFound($"Book with ID {bookId} not found.");
            return Ok(bookDetails);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBooks([FromQuery] BookFilterParams filterParams)
        {
            var result = await _bookService.GetBooksAsync(filterParams);
            return Ok(result);
        }
    }
}
