using LandingApi.Data;
using LandingApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LandingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")] // Только для админов
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string sortBy = "name_asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // Базовый запрос с включением роли
            var query = _context.Users
                .Include(u => u.Role)
                .AsQueryable();

            // Поиск по имени или роли
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    u.Name.Contains(search) ||
                    u.Role.Name.Contains(search));
            }

            // Сортировка
            query = sortBy switch
            {
                "name_desc" => query.OrderByDescending(u => u.Name),
                "role_asc" => query.OrderBy(u => u.Role.Name),
                "role_desc" => query.OrderByDescending(u => u.Role.Name),
                _ => query.OrderBy(u => u.Name) // По умолчанию: name_asc
            };

            // Пагинация
            var totalItems = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    RoleId = u.RoleId,
                    RoleName = u.Role.Name
                })
                .ToListAsync();

            return Ok(new
            {
                TotalItems = totalItems,
                Items = users,
                CurrentPage = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Id == id)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    RoleId = u.RoleId,
                    RoleName = u.Role.Name
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

