using System.ComponentModel.DataAnnotations;

namespace LandingApi.DTOs
{
    public class LoginModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
