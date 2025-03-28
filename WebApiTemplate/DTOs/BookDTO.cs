﻿namespace WebApiTemplate.DTOs
{
        public class BookDTO
        {
            public Guid Id { get; set; }
            public string ISBN { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Author { get; set; } = string.Empty;
            public List<string> Genres { get; set; } = new();
            public string Publisher { get; set; } = string.Empty;
            public int PublicationYear { get; set; }
        }
    

}
