namespace WebApiTemplate.DTOs.Genre
{
    public class TagGenreDto
    {


        public Guid BookId { get; set; }
        public List<Guid> GenreIds { get; set; } = new List<Guid>();
    }
}
