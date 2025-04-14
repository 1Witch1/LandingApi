using System.ComponentModel.DataAnnotations;

namespace LandingApi.DTOs
{
    public class RegisterModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public int RoleId { get; set; }
    }
}
