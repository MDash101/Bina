using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication_School.Data;

namespace WebApplication_School.Controllers
{
    [Authorize(Roles = "Admin")] 
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogsController : ControllerBase 
    {
        private readonly ApplicationDbContext _context;

        public AuditLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _context.AuditLogs
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Take(100)
                .ToListAsync();

            return Ok(logs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var log = await _context.AuditLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (log == null) return NotFound();

            return Ok(log);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var logs = await _context.AuditLogs
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(logs);
        }

        [HttpGet("table/{tableName}")]
        public async Task<IActionResult> GetByTable(string tableName)
        {
            var logs = await _context.AuditLogs
                .AsNoTracking()
                .Where(x => x.TableName == tableName)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(logs);
        }

        [HttpGet("action/{actionName}")]
        public async Task<IActionResult> GetByAction(string actionName)
        {
            var logs = await _context.AuditLogs
                .AsNoTracking()
                .Where(x => x.Action == actionName)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(logs);
        }
    }
}