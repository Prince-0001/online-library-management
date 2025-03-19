using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiTemplate.DTOs.Genre;
using WebApiTemplate.Services;

namespace WebApiTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Only Admins can manage genres
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        // ✅ Create Genre
        [HttpPost]
        public async Task<IActionResult> CreateGenre([FromBody] CreateGenreDTO createGenreDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _genreService.CreateGenreAsync(createGenreDto);
            return CreatedAtAction(nameof(GetGenreById), new { genreId = result.Id }, result);
        }

        // ✅ Update Genre
        [HttpPut("{genreId}")]
        public async Task<IActionResult> UpdateGenre(Guid genreId, [FromBody] UpdateGenreDTO updateGenreDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _genreService.UpdateGenreAsync(genreId, updateGenreDto);
            return Ok(result);
        }

        // ✅ Delete Genre
        [HttpDelete("{genreId}")]
        public async Task<IActionResult> DeleteGenre(Guid genreId)
        {
            await _genreService.DeleteGenreAsync(genreId);
            return NoContent();
        }

        // ✅ Get All Genres
        [HttpGet]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _genreService.GetAllGenresAsync();
            return Ok(genres);
        }

        // ✅ Get Genre by Id
        [HttpGet("{genreId}")]
        public async Task<IActionResult> GetGenreById(Guid genreId)
        {
            var genre = await _genreService.GetGenreByIdAsync(genreId);
            return Ok(genre);
        }

        // ✅ Tag Genres to Book
        [HttpPost("tag")]
        public async Task<IActionResult> TagGenres([FromBody] TagGenreDto tagGenreDto)
        {
            await _genreService.TagGenresAsync(tagGenreDto);
            return Ok("Genres tagged successfully.");
        }

        // ✅ Untag Genres from Book
        [HttpPost("untag")]
        public async Task<IActionResult> UntagGenres([FromBody] TagGenreDto tagGenreDto)
        {
            await _genreService.UntagGenresAsync(tagGenreDto);
            return Ok("Genres untagged successfully.");
        }
    }
}
