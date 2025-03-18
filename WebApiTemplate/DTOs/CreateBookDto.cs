namespace WebApiTemplate.DTOs
{
    public class CreateBookDto
    {
        public string ISBN { get; set; }=string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
    }
}
