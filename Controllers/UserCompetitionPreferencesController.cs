using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication_School.Data;
using WebApplication_School.Models;

namespace WebApplication_School.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserCompetitionPreferencesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserCompetitionPreferencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPreferences(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (userId != currentUserId && !User.IsInRole("Admin") && !User.IsInRole("Teacher"))
                return Forbid();

            var preferences = await _context.UserCompetitionPreferences
                .AsNoTracking()
                .Include(p => p.Competition)
                .Where(p => p.UserId == userId)
                .Select(p => new {
                    p.PreferenceId,
                    p.PreferenceName,
                    p.CompetitionType,
                    p.CreatedDate,
                    p.CompetitionId,
                    CompetitionTitle = p.Competition != null ? p.Competition.Title : "اهتمام عام"
                })
                .ToListAsync();

            return Ok(preferences);
        }


        [HttpPost]
        public async Task<IActionResult> AddPreference([FromBody] UserCompetitionPreferences preference)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            preference.UserId = currentUserId;

            var exists = await _context.UserCompetitionPreferences
                .AnyAsync(p => p.UserId == currentUserId && p.CompetitionId == preference.CompetitionId);

            if (exists) return BadRequest("هذه المسابقة موجودة بالفعل في مفضلتك.");

            preference.CreatedDate = DateTime.UtcNow;
            preference.User = null!;
            preference.Competition = null!;

            _context.UserCompetitionPreferences.Add(preference);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تمت الإضافة بنجاح", data = preference });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePreference(int id, [FromBody] UserCompetitionPreferences updated)
        {
           
            var preference = await _context.UserCompetitionPreferences.FindAsync(id);
            if (preference == null) return NotFound("التفضيلات غير موجودة");

           
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (preference.UserId != currentUserId) return Forbid();

     
            preference.PreferenceName = updated.PreferenceName;
            preference.CompetitionType = updated.CompetitionType;

           
            var user = await _context.Users.FindAsync(currentUserId);
            if (user == null) return NotFound("المستخدم غير موجود");
                  

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم التحديث بنجاح", data = preference });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePreference(int id)
        {
            var preference = await _context.UserCompetitionPreferences.FindAsync(id);
            if (preference == null) return NotFound();

            
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (preference.UserId != currentUserId) return Forbid();

            _context.UserCompetitionPreferences.Remove(preference);
            await _context.SaveChangesAsync();
            return Ok(new { message = "تم الحذف" });
        }


        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetUsersByInterest(string type)
        {
            var users = await _context.UserCompetitionPreferences
                .AsNoTracking()
                .Where(p => p.CompetitionType.ToLower() == type.ToLower())
                .Select(p => new {
                    p.UserId,
                    FullName = p.User != null ? p.User.FullName : "مستخدم غير معروف",
                    Email = p.User != null ? p.User.Email : null
                })
                .Distinct()
                .ToListAsync();

            return Ok(users);
        }
    }
}