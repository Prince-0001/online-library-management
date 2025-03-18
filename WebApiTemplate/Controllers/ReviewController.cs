using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiTemplate.DTOs;
using WebApiTemplate.Exceptions;
using WebApiTemplate.Services.Interfaces;

namespace WebApiTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IReviewService reviewService, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        // ✅ GET: api/review/book/{bookId}
        [HttpGet("book/{bookId}")]
        [Authorize]
        public async Task<IActionResult> GetReviewsByBookId(Guid bookId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByBookIdAsync(bookId);
                if (!reviews.Any())
                {
                    _logger.LogWarning("No reviews found for BookId: {BookId}", bookId);
                    return NotFound("No reviews found for the specified book.");
                }

                _logger.LogInformation("Fetched {Count} reviews for BookId: {BookId}", reviews.Count(), bookId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reviews for BookId: {BookId}", bookId);
                return StatusCode(500, "An error occurred while fetching reviews.");
            }
        }

        // ✅ GET: api/review/{reviewId}
        [HttpGet("{reviewId}")]
        [Authorize]
        public async Task<IActionResult> GetReviewById(Guid reviewId)
        {
            try
            {
                var review = await _reviewService.GetReviewByIdAsync(reviewId);
                _logger.LogInformation("Fetched ReviewId: {ReviewId}", reviewId);
                return Ok(review);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Review not found. ReviewId: {ReviewId}", reviewId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching review. ReviewId: {ReviewId}", reviewId);
                return StatusCode(500, "An error occurred while fetching the review.");
            }
        }

        // ✅ POST: api/review/book/{bookId}
        [HttpPost("book/{bookId}")]
        [Authorize(Roles = "User, Author, Admin")]
        public async Task<IActionResult> CreateReview(Guid bookId, [FromBody] CreateReviewDTO createReviewDto)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId not found in claims.");
                    return Unauthorized("User authentication failed.");
                }

                var review = await _reviewService.CreateReviewAsync(bookId, userId, createReviewDto);
                _logger.LogInformation("Review created for BookId: {BookId} by UserId: {UserId}", bookId, userId);

                return CreatedAtAction(nameof(GetReviewById), new { reviewId = review.Id }, review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review for BookId: {BookId}", bookId);
                return StatusCode(500, "An error occurred while creating the review.");
            }
        }

        // ✅ PUT: api/review/{reviewId}
        [HttpPut("{reviewId}")]
        [Authorize(Roles = "User, Author, Admin")]
        public async Task<IActionResult> UpdateReview(Guid reviewId, [FromBody] UpdateReviewDTO updateReviewDto)
        {
            string? userId = null; // Declare it outside the try block

            try
            {
                userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId not found in claims.");
                    return Unauthorized("User authentication failed.");
                }

                var updatedReview = await _reviewService.UpdateReviewAsync(reviewId, userId, updateReviewDto);
                _logger.LogInformation("ReviewId: {ReviewId} updated successfully by UserId: {UserId}", reviewId, userId);

                return Ok(updatedReview);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Review not found. ReviewId: {ReviewId}", reviewId);
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized update attempt. ReviewId: {ReviewId} by UserId: {UserId}", reviewId, userId);
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review. ReviewId: {ReviewId}", reviewId);
                return StatusCode(500, "An error occurred while updating the review.");
            }
        }

        [HttpDelete("{reviewId}")]
        [Authorize(Roles = "User, Author, Admin")]
        public async Task<IActionResult> DeleteReview(Guid reviewId)
        {
            string? userId = null; // Declare it outside the try block

            try
            {
                userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId not found in claims.");
                    return Unauthorized("User authentication failed.");
                }

                var result = await _reviewService.DeleteReviewAsync(reviewId, userId);
                if (!result)
                {
                    _logger.LogWarning("Review not found or unauthorized. ReviewId: {ReviewId}", reviewId);
                    return NotFound("Review not found or unauthorized.");
                }

                _logger.LogInformation("ReviewId: {ReviewId} deleted successfully by UserId: {UserId}", reviewId, userId);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Review not found. ReviewId: {ReviewId}", reviewId);
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized deletion attempt. ReviewId: {ReviewId} by UserId: {UserId}", reviewId, userId);
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review. ReviewId: {ReviewId}", reviewId);
                return StatusCode(500, "An error occurred while deleting the review.");
            }
        }

    }
}
