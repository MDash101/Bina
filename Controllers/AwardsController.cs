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
    public class AwardsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AwardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var awards = await _context.Awards
                .AsNoTracking()
                .Include(a => a.User)
                .OrderByDescending(a => a.AwardedDate)
                .Select(a => new
                {
                    a.AwardsId,
                    a.UserId,
                    UserFullName = a.User.FullName,
                    a.Title,
                    a.Type,
                    a.Description,
                    a.AwardedDate,
                    a.PointsAwarded,
                    a.BadgeUrl
                })
                .ToListAsync();

            return Ok(awards);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            if (userId <= 0) return BadRequest("معرف المستخدم غير صحيح");

            var userAwards = await _context.Awards
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AwardedDate)
                .ToListAsync();

            return Ok(userAwards);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Awards award)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userExists = await _context.Users.AnyAsync(u => u.UsersId == award.UserId);
            if (!userExists) return BadRequest("المستخدم غير موجود");

            // منع التكرار
            var exists = await _context.Awards.AnyAsync(a =>
                a.UserId == award.UserId && a.Title == award.Title);

            if (exists) return BadRequest("هذه الجائزة مضافة لهذا المستخدم مسبقاً");

            award.AwardedDate = award.AwardedDate == default ? DateTime.UtcNow : award.AwardedDate;

            _context.Awards.Add(award);

           
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == award.UserId);
            if (profile != null)
            {
                profile.AwardsCount++;
                profile.TotalPoints += award.PointsAwarded;
            }

            await _context.SaveChangesAsync();
            return Ok(award);
        }

        [Authorize(Roles = "Admin,Teacher")] 
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Awards updatedAward)
        {
            var award = await _context.Awards.FindAsync(id);
            if (award == null) return NotFound("الجائزة غير موجودة");

            award.Title = updatedAward.Title;
            award.Type = updatedAward.Type;
            award.Description = updatedAward.Description;
            award.PointsAwarded = updatedAward.PointsAwarded;
            award.BadgeUrl = updatedAward.BadgeUrl;

            await _context.SaveChangesAsync();
            return Ok(award);
        }

        [Authorize(Roles = "Admin,Teacher")] 
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var award = await _context.Awards.FindAsync(id);
            if (award == null) return NotFound("الجائزة غير موجودة");

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == award.UserId);
            if (profile != null)
            {
                if (profile.AwardsCount > 0) profile.AwardsCount--;
     
            }

            _context.Awards.Remove(award);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف الجائزة وتحديث ملف المستخدم" });
        }
    }
}