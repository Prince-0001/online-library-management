using WebApiTemplate.DTOs;
using WebApiTemplate.Exceptions;
using WebApiTemplate.Models;
using WebApiTemplate.Repository.Database;
using WebApiTemplate.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace WebApiTemplate.Services
{
    public class ReviewService : IReviewService
    {
        private readonly WenApiTemplateDbContext _context;

        public ReviewService(WenApiTemplateDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsByBookIdAsync(Guid bookId)
        {
            return await _context.Reviews
                .Where(r => r.BookId == bookId)
                .Select(r => new ReviewDTO
                {
                    Id = r.Id,
                    Content = r.Content,
                    Rating = r.Rating,
                    BookId = r.BookId,
                    UserId = r.UserId,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<ReviewDTO> GetReviewByIdAsync(Guid reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                throw new NotFoundException("Review not found.");

            return new ReviewDTO
            {
                Id = review.Id,
                Content = review.Content,
                Rating = review.Rating,
                BookId = review.BookId,
                UserId = review.UserId,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task<ReviewDTO> CreateReviewAsync(Guid bookId, string userId, CreateReviewDTO reviewDto)
        {
            var review = new Review
            {
                Id = Guid.NewGuid(),
                Content = reviewDto.Content,
                Rating = reviewDto.Rating,
                BookId = bookId,
                UserId = userId
            };

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return new ReviewDTO
            {
                Id = review.Id,
                Content = review.Content,
                Rating = review.Rating,
                BookId = review.BookId,
                UserId = review.UserId,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task<ReviewDTO> UpdateReviewAsync(Guid reviewId, string userId, UpdateReviewDTO reviewDto)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                throw new NotFoundException("Review not found.");
            if (review.UserId != userId)
                throw new UnauthorizedAccessException("You can only edit your own reviews.");

            review.Content = reviewDto.Content;
            review.Rating = reviewDto.Rating;
            review.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ReviewDTO
            {
                Id = review.Id,
                Content = review.Content,
                Rating = review.Rating,
                BookId = review.BookId,
                UserId = review.UserId,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task<bool> DeleteReviewAsync(Guid reviewId, string userId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                throw new NotFoundException("Review not found.");
            if (review.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own reviews.");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
