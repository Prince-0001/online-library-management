using FluentValidation;
using WebApiTemplate.DTOs;

namespace WebApiTemplate.Validators
{
    public class BookValidator : AbstractValidator<CreateBookDto>
    {
        public BookValidator()
        {
            RuleFor(book => book.ISBN).NotEmpty().WithMessage("ISBN is required.");
            RuleFor(book => book.Title).NotEmpty().WithMessage("Title is required.");
            RuleFor(book => book.Author).NotEmpty().WithMessage("Author is required.");
            RuleFor(book => book.Genre).NotEmpty().WithMessage("Genre is required.");
            RuleFor(book => book.Publisher).NotEmpty().WithMessage("Publisher is required.");
            RuleFor(book => book.PublicationYear).GreaterThan(0).WithMessage("Publication Year must be greater than 0.");
        }
    }
}
