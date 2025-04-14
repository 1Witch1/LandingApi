using System.ComponentModel.DataAnnotations;

namespace LandingApi.DTOs
{
    public class EventCreateDto
    {
        [Required] public string Title { get; set; }
        [Required] public string Description { get; set; }
        [Required] public DateTime EventDate { get; set; }
        public IFormFile? File { get; set; }
    }

    public class EventResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string? FilePath { get; set; }
        public int OrganizerId { get; set; }
        public string? OrganizerName { get; set; }
    }
}
