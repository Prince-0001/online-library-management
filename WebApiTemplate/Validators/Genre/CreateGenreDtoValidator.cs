using FluentValidation;
using WebApiTemplate.DTOs.Genre;

namespace WebApiTemplate.Validators.Genre
{
    public class CreateGenreDtoValidator : AbstractValidator<CreateGenreDTO>
    {
        public CreateGenreDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Genre name is required.")
                .MaximumLength(100).WithMessage("Genre name cannot exceed 100 characters.");
        }
    }
}
