using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication_School.Data;
using WebApplication_School.Models;

namespace WebApplication_School.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompetitionSubmissionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompetitionSubmissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        [AllowAnonymous]
        [HttpGet("competition/{competitionId}")]
        public async Task<IActionResult> GetByCompetition(int competitionId)
        {
            if (competitionId <= 0) return BadRequest(new { message = "معرف المسابقة غير صحيح" });

            bool isStaff = User.IsInRole("Admin") || User.IsInRole("Teacher");
            int currentUserId = GetCurrentUserId();

            var query = _context.CompetitionSubmissions.AsNoTracking().AsQueryable();

            
            if (!isStaff)
            {
            
                query = query.Where(s => s.Project.OwnerId == currentUserId || s.Project.IsPublic == true);
            }

            var submissions = await query
                .Include(s => s.Project)
                .Where(s => s.CompetitionId == competitionId)
                .OrderByDescending(s => s.SubmittedDate)
                .Select(s => new
                {
                    s.SubmissionId,
                    s.CompetitionId,
                    s.ProjectId,
                    s.SubmittedDate,
                    s.Status,
                 
                    Score = (isStaff || s.Project.OwnerId == currentUserId) ? s.Score : null,
                    JudgeComments = (isStaff || s.Project.OwnerId == currentUserId) ? s.JudgeComments : null,
                    s.AwardType,
                    ProjectTitle = s.Project != null ? s.Project.Title : "مشروع محذوف"
                })
                .ToListAsync();

            return Ok(submissions);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CompetitionSubmissions submission)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

           
            var competition = await _context.Competitions
                .FirstOrDefaultAsync(c => c.CompetitionId == submission.CompetitionId);

            if (competition == null) return NotFound(new { message = "المسابقة غير موجودة" });

           
            if (competition.EndDate < DateTime.UtcNow) return BadRequest(new { message = "انتهى وقت التقديم لهذه المسابقة" });

            int currentUserId = GetCurrentUserId();
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.ProjectsId == submission.ProjectId && p.OwnerId == currentUserId);

            if (project == null) return BadRequest(new { message = "لا يمكنك المشاركة بمشروع لا تملكه" });

         
            var exists = await _context.CompetitionSubmissions.AnyAsync(s =>
                s.CompetitionId == submission.CompetitionId && s.ProjectId == submission.ProjectId);

            if (exists) return BadRequest(new { message = "تم إرسال هذا المشروع لهذه المسابقة مسبقاً" });

            submission.SubmittedDate = DateTime.UtcNow;
            submission.Status = "Pending"; 

            _context.CompetitionSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByCompetition), new { competitionId = submission.CompetitionId }, submission);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CompetitionSubmissions updated)
        {
            var submission = await _context.CompetitionSubmissions.FindAsync(id);
            if (submission == null) return NotFound(new { message = "المشاركة غير موجودة" });

            
            submission.Status = updated.Status;
            submission.Score = updated.Score;
            submission.JudgeComments = updated.JudgeComments;
            submission.AwardType = updated.AwardType;

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم تحديث التقييم بنجاح", data = submission });
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var submission = await _context.CompetitionSubmissions.FindAsync(id);
            if (submission == null) return NotFound(new { message = "المشاركة غير موجودة" });

            _context.CompetitionSubmissions.Remove(submission);
            await _context.SaveChangesAsync();
            return Ok(new { message = "تم حذف المشاركة بنجاح" });
        }
    }
}