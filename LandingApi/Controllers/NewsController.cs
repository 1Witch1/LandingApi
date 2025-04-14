using LandingApi.DTOs;
using LandingApi.Interfaces;
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
    public class NewsController : ControllerBase
    {
        private readonly IRepository<News> _newsRepository;

        public NewsController(IRepository<News> newsRepository)
        {
            _newsRepository = newsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string search, [FromQuery] string sort, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            {
                var baseQuery = _newsRepository.AsQueryable()
                    .Include(n => n.Author);

                var filteredQuery = !string.IsNullOrEmpty(search)
                    ? baseQuery.Where(n => n.Title.Contains(search) || n.Content.Contains(search))
                    : baseQuery;

                var sortedQuery = sort switch
                {
                    "date_asc" => filteredQuery.OrderBy(n => n.PublishedDate),
                    "date_desc" => filteredQuery.OrderByDescending(n => n.PublishedDate),
                    _ => filteredQuery.OrderByDescending(n => n.PublishedDate)
                };

                var totalItems = await sortedQuery.CountAsync();
                var items = await sortedQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new
                {
                    TotalItems = totalItems,
                    Items = items,
                    CurrentPage = page,
                    PageSize = pageSize
                });
            }
        }
            [HttpPost]
            [Authorize(Roles = "admin,editor")]
            public async Task<IActionResult> Create([FromForm] NewsCreateModel model)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var filePath = await SaveFile(model.File);

                var news = new News
                {
                    Title = model.Title,
                    Content = model.Content,
                    PublishedDate = DateTime.UtcNow,
                    FilePath = filePath,
                    AuthorId = userId
                };

                await _newsRepository.AddAsync(news);
                await _newsRepository.SaveChangesAsync();

                return Ok(news);
            }
        

        private async Task<string> SaveFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{uniqueFileName}";
        }
    }
}
