using System.ComponentModel.DataAnnotations;

namespace LandingApi.DTOs
{
    public class NewsCreateModel
    {
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Content { get; set; }
        public IFormFile? File { get; set; }
    }
}
