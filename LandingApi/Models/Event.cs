using System.ComponentModel.DataAnnotations.Schema;

namespace LandingApi.Models
{
    public class Event
    {
        
        public int Id { get; set; }

        [Column("title")]
        public string? Title { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("event_date")]
        public DateTime EventDate { get; set; }

        [Column("file_path")]
        public string? FilePath { get; set; }

        [Column("organizer_id")]
        public int OrganizerId { get; set; }

        
        public User? Organizer { get; set; }
    }
}
