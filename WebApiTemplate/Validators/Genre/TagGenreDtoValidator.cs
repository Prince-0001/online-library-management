using FluentValidation;
using WebApiTemplate.DTOs.Genre;
namespace WebApiTemplate.Validators.Genre
{
    public class TagGenreDtoValidator : AbstractValidator<TagGenreDto>
    {
        public TagGenreDtoValidator()
        {
            RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("BookId is required.");

            RuleFor(x => x.GenreIds)
                .NotEmpty().WithMessage("At least one GenreId is required.");
        }

    }

}

