using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiTemplate.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string ISBN { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        [Required]
        public string Publisher { get; set; } = string.Empty;

        [Required]
        public int PublicationYear { get; set; }

        // ✅ Navigation Property for Reviews
        public List<Review> Reviews { get; set; } = new();

        // ✅ Many-to-Many Relationship with Genre
        public List<BookGenre> BookGenres { get; set; } = new();
    }
}
