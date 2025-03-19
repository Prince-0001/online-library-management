using WebApiTemplate.DTOs;
using WebApiTemplate.DTOs.Genre;

namespace WebApiTemplate.Services
{
    public interface IGenreService
    {
        Task<GenreDto> CreateGenreAsync(CreateGenreDTO genreDto);
        Task<GenreDto> UpdateGenreAsync(Guid genreId, UpdateGenreDTO genreDto);
        Task<bool> DeleteGenreAsync(Guid genreId);
        Task<List<GenreDto>> GetAllGenresAsync();
        Task<GenreDto> GetGenreByIdAsync(Guid genreId);
        Task<bool> TagGenresAsync(TagGenreDto tagGenreDto);
        Task<bool> UntagGenresAsync(TagGenreDto tagGenreDto);
    }
}
