using WebApiTemplate.DTOs;
using WebApiTemplate.Models;

namespace WebApiTemplate.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetReviewsByBookIdAsync(Guid bookId);
        Task<ReviewDTO> GetReviewByIdAsync(Guid reviewId);
        Task<ReviewDTO> CreateReviewAsync(Guid bookId, string userId, CreateReviewDTO reviewDto);
        Task<ReviewDTO> UpdateReviewAsync(Guid reviewId, string userId, UpdateReviewDTO reviewDto);
        Task<bool> DeleteReviewAsync(Guid reviewId, string userId);
        
    }
}
