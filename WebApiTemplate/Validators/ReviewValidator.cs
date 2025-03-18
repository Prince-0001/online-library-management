using FluentValidation;
using WebApiTemplate.DTOs;

namespace WebApiTemplate.Validators
{
    public class ReviewValidator : AbstractValidator<CreateReviewDTO>
    {
        public ReviewValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MaximumLength(500).WithMessage("Content cannot exceed 500 characters.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        }
    }
}
