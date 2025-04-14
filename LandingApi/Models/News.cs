using System.ComponentModel.DataAnnotations.Schema;

namespace LandingApi.Models
{
    public class News
    {
        
        public int Id { get; set; }

        [Column("title")]
        public string? Title { get; set; }

        [Column("content")]
        public string? Content { get; set; }

        [Column("published_date")]
        public DateTime PublishedDate { get; set; }

        [Column("file_path")]
        public string? FilePath { get; set; }

        [Column("author_id")]
        public int AuthorId { get; set; }

        
        public User? Author { get; set; }
    }
}
