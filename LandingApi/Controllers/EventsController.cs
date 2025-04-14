using LandingApi.Data;
using LandingApi.DTOs;
using LandingApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LandingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EventsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? sortBy = "date_desc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Events
                .Include(e => e.Organizer)
                .AsQueryable();

            // Поиск
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e =>
                    e.Title.Contains(search) ||
                    e.Description.Contains(search));
            }

            // Сортировка
            query = sortBy switch
            {
                "date_asc" => query.OrderBy(e => e.EventDate),
                "title_asc" => query.OrderBy(e => e.Title),
                "title_desc" => query.OrderByDescending(e => e.Title),
                _ => query.OrderByDescending(e => e.EventDate) // date_desc по умолчанию
            };

            // Пагинация
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EventResponseDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    FilePath = e.FilePath,
                    OrganizerId = e.OrganizerId,
                    OrganizerName = e.Organizer.Name
                })
                .ToListAsync();

            return Ok(new
            {
                TotalItems = totalItems,
                Items = items,
                CurrentPage = page,
                PageSize = pageSize
            });
        }

        [HttpPost]
        [Authorize(Roles = "admin,editor")]
        public async Task<IActionResult> Create([FromForm] EventCreateDto model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var filePath = await SaveFile(model.File);

            var newEvent = new Event
            {
                Title = model.Title,
                Description = model.Description,
                EventDate = model.EventDate,
                FilePath = filePath,
                OrganizerId = userId
            };

            await _context.Events.AddAsync(newEvent);
            await _context.SaveChangesAsync();

            return Ok(new EventResponseDto
            {
                Id = newEvent.Id,
                Title = newEvent.Title,
                Description = newEvent.Description,
                EventDate = newEvent.EventDate,
                FilePath = newEvent.FilePath,
                OrganizerId = newEvent.OrganizerId
            });
        }

        private async Task<string?> SaveFile(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{uniqueFileName}";
        }
    }
}
