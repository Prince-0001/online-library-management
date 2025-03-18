namespace WebApiTemplate.DTOs
{
    public class ReviewDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }=string.Empty;
        public int Rating { get; set; }
        public Guid BookId { get; set; }
        public String UserId { get; set; }= string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
