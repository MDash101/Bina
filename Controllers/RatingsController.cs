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
    public class RatingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RatingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRatings(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user id");

            var ratings = await _context.Ratings
                .AsNoTracking()
                .Include(r => r.RaterUser)
                .Include(r => r.Project)
                .Where(r => r.RatedUserId == userId)
                .Select(r => new
                {
                    r.RatingId,
                    r.RatedUserId,
                    r.RaterUserId,
                    RaterName = r.RaterUser != null ? r.RaterUser.FullName : null,
                    r.ProjectId,
                    ProjectTitle = r.Project != null ? r.Project.Title : null,
                    r.RatingValue,
                    r.Comment,
                    r.RatingCategory,
                    r.CreatedDate
                })
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();

            return Ok(ratings);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Rating rating)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var currentUserIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserIdClaim)) return Unauthorized();
            int currentUserId = int.Parse(currentUserIdClaim);

            rating.RaterUserId = currentUserId;

            if (rating.RaterUserId == rating.RatedUserId)
                return BadRequest("لا يمكنك تقييم نفسك");

            if (rating.ProjectId > 0)
            {
                var alreadyRated = await _context.Ratings.AnyAsync(r =>
                    r.RaterUserId == currentUserId &&
                    r.RatedUserId == rating.RatedUserId &&
                    r.ProjectId == rating.ProjectId);

                if (alreadyRated) return BadRequest("لقد قمت بتقييم هذا المستخدم في هذا المشروع مسبقاً");
            }

            var ratedUserExists = await _context.Users.AnyAsync(u => u.UsersId == rating.RatedUserId);
            if (!ratedUserExists) return BadRequest("المستخدم المقيَّم غير موجود");

            rating.CreatedDate = DateTime.UtcNow;
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return Ok(rating);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Rating updated)
        {
            if (id <= 0)
                return BadRequest();

            var rating = await _context.Ratings.FindAsync(id);

            if (rating == null)
                return NotFound("التقييم غير موجود");

            rating.RatingValue = updated.RatingValue;
            rating.Comment = updated.Comment;
            rating.RatingCategory = updated.RatingCategory;

            await _context.SaveChangesAsync();

            return Ok(rating);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null) return NotFound("التقييم غير موجود");

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("لم يتم العثور على تعريف للمستخدم في التوكن.");
            }

            int currentUserId = int.Parse(userIdClaim);

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف التقييم" });
        }
    }
}