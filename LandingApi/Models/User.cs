using System.ComponentModel.DataAnnotations.Schema;

namespace LandingApi.Models
{
    public class User
    {
        
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("password")]
        public string? Password { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        
        public Role? Role { get; set; }
        public ICollection<News> News { get; set; } = new List<News>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
