using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication_School.Data;
using WebApplication_School.Models;

namespace WebApplication_School.Controllers
{
 

    public class UserProfileDto
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? AcademicYear { get; set; }
        public string? SkillsSummary { get; set; }
        public string? GitHubUrl { get; set; }
        public string? LinkedInUrl { get; set; }

        public string? Country { get; set; }
        public DateTime? BirthDate { get; set; }
        public int Age { get; set; }
        public bool IsPublic { get; set; }
        public int CompetitionsCount { get; set; }
        public int ProjectsCount { get; set; }
        public int AwardsCount { get; set; }
        public required string UserEmail { get; set; }
    }

    public class CreateUserProfileDto
    {
        public int UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public required string AcademicYear { get; set; }
        public required string SkillsSummary { get; set; }
        public string? GitHubUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? Country { get; set; }
        public DateTime? BirthDate { get; set; }
    }

    public class UpdateStatsDto
    {
        public int Projects { get; set; }
        public int Competitions { get; set; }
        public int Awards { get; set; }
    }

  

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var profile = await _context.UserProfiles
                .AsNoTracking()
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .Select(p => new UserProfileDto
                {
                    ProfileId = p.ProfileId,
                    UserId = p.UserId,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Bio = p.Bio,
                    AvatarUrl = p.AvatarUrl,
                    AcademicYear = p.AcademicYear,
                    SkillsSummary = p.SkillsSummary,
                    GitHubUrl = p.GitHubUrl,
                    LinkedInUrl = p.LinkedInUrl,
                    Country = p.Country,
                    BirthDate = p.BirthDate,
                    Age = p.Age,
                    IsPublic = p.IsPublic,
                    CompetitionsCount = p.CompetitionsCount,
                    ProjectsCount = p.ProjectsCount,
                    AwardsCount = p.AwardsCount,
                    UserEmail = p.User.Email 
                })
                .FirstOrDefaultAsync();

            if (profile == null) return NotFound("البروفايل غير موجود");

           
            if (!profile.IsPublic && profile.UserId != GetCurrentUserId() && !User.IsInRole("Admin"))
                return Forbid();

            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserProfileDto dto)
        {
            var currentUserId = GetCurrentUserId();

            
            if (dto.UserId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            var alreadyHasProfile = await _context.UserProfiles.AnyAsync(p => p.UserId == dto.UserId);
            if (alreadyHasProfile) return BadRequest("لديك بروفايل بالفعل، استخدم Update للتعديل.");

            var profile = new UserProfiles
            {
                UserId = dto.UserId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                AcademicYear = dto.AcademicYear,
                SkillsSummary = dto.SkillsSummary,
                Bio = dto.Bio,
                AvatarUrl = dto.AvatarUrl,
                GitHubUrl = dto.GitHubUrl,
                LinkedInUrl = dto.LinkedInUrl,
                Country = dto.Country,
                BirthDate = dto.BirthDate,
                IsPublic = true 
            };

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
            return Ok(profile);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUserProfileDto dto)
        {
            var profile = await _context.UserProfiles.FindAsync(id);
            if (profile == null) return NotFound("البروفايل غير موجود");

            if (profile.UserId != GetCurrentUserId() && !User.IsInRole("Admin"))
                return Forbid();

            profile.FirstName = dto.FirstName;
            profile.LastName = dto.LastName;
            profile.Bio = dto.Bio;
            profile.AcademicYear = dto.AcademicYear;
            profile.SkillsSummary = dto.SkillsSummary;
            profile.GitHubUrl = dto.GitHubUrl;
            profile.LinkedInUrl = dto.LinkedInUrl;
            profile.Country = dto.Country;
            profile.BirthDate = dto.BirthDate;

            await _context.SaveChangesAsync();
            return Ok(profile);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/stats")]
        public async Task<IActionResult> UpdateStats(int id, [FromBody] UpdateStatsDto dto)
        {
            var profile = await _context.UserProfiles.FindAsync(id);
            if (profile == null) return NotFound("البروفايل غير موجود");

            profile.ProjectsCount = dto.Projects;
            profile.CompetitionsCount = dto.Competitions;
            profile.AwardsCount = dto.Awards;

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم تحديث الإحصائيات بواسطة المسؤول" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var profile = await _context.UserProfiles.FindAsync(id);
            if (profile == null) return NotFound("البروفايل غير موجود");

            _context.UserProfiles.Remove(profile);
            await _context.SaveChangesAsync();
            return Ok(new { message = "تم حذف الحساب بنجاح" });
        }
    }
}