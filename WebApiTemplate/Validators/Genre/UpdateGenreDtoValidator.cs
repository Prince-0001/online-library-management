using FluentValidation;
using WebApiTemplate.DTOs.Genre;

namespace WebApiTemplate.Validators.Genre
{
    public class UpdateGenreDtoValidator : AbstractValidator<UpdateGenreDTO>
    {
        public UpdateGenreDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Genre name is required.")
                .MaximumLength(100).WithMessage("Genre name cannot exceed 100 characters.");
        }
    }
}
