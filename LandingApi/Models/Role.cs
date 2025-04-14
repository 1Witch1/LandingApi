using System.ComponentModel.DataAnnotations.Schema;

namespace LandingApi.Models
{
    public class Role
    {
        
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("code")]
        public string? Code { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
