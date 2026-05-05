using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication_School.Data;
using WebApplication_School.Models;

namespace WebApplication_School.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    [ApiController]
    [Route("api/[controller]")]
    public class CompetitionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompetitionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
           
            bool isStaff = User.Identity?.IsAuthenticated == true && (User.IsInRole("Admin") || User.IsInRole("Teacher"));

            var query = _context.Competitions.AsNoTracking().AsQueryable();

            if (!isStaff)
            {
                query = query.Where(c => c.IsActive == true);
            }

            var data = await query
                .Include(c => c.Department)
                .OrderByDescending(c => c.StartDate)
                .Select(c => new
                {
                    c.CompetitionId,
                    c.Title,
                    c.Description,
                    c.StartDate,
                    c.EndDate,
                    c.SubmissionDeadline,
                    c.IsActive,
                    c.Category,
                    c.ImageUrl,
                    c.PrizeDescription,
                    c.DepartmentId,
                    DepartmentName = c.Department != null ? c.Department.Name : null,
                    SubmissionsCount = _context.CompetitionSubmissions.Count(s => s.CompetitionId == c.CompetitionId)
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Competitions competition)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

           
            if (competition.StartDate.Date < DateTime.UtcNow.Date)
                return BadRequest(new { message = "تاريخ البدء لا يمكن أن يكون في الماضي" });

            if (competition.EndDate <= competition.StartDate)
                return BadRequest(new { message = "تاريخ الانتهاء يجب أن يكون بعد تاريخ البدء" });

     
            var exists = await _context.Competitions.AnyAsync(c =>
                c.Title == competition.Title && c.StartDate.Year == competition.StartDate.Year);

            if (exists) return BadRequest(new { message = "يوجد مسابقة بنفس العنوان في هذا العام" });

            _context.Competitions.Add(competition);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = competition.CompetitionId }, competition);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) // تأكد أن الاسم هنا هو GetById
        {
            var competition = await _context.Competitions
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CompetitionId == id);

            if (competition == null) return NotFound();
            return Ok(competition);
        }

        [HttpPut("{id}")] 
        public async Task<IActionResult> Update(int id, [FromBody] Competitions updated)
        {
            var competition = await _context.Competitions.FindAsync(id);
            if (competition == null) return NotFound(new { message = "المسابقة غير موجودة" });

            
            var hasSubmissions = await _context.CompetitionSubmissions.AnyAsync(s => s.CompetitionId == id);
            if (hasSubmissions && competition.StartDate < DateTime.UtcNow && updated.StartDate != competition.StartDate)
            {
                return BadRequest(new { message = "لا يمكن تعديل تاريخ بدء مسابقة انطلقت ولها مشاركون" });
            }

        
            competition.Title = updated.Title;
            competition.Description = updated.Description;
            competition.StartDate = updated.StartDate;
            competition.EndDate = updated.EndDate;
            competition.SubmissionDeadline = updated.SubmissionDeadline;
            competition.IsActive = updated.IsActive;
            competition.DepartmentId = updated.DepartmentId;
            competition.Category = updated.Category;
            competition.ImageUrl = updated.ImageUrl;
            competition.PrizeDescription = updated.PrizeDescription;
            competition.JudgingCriteria = updated.JudgingCriteria;

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم التحديث بنجاح", data = competition });
        }
        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
           
            var activeCompetitions = await _context.Competitions
                .AsNoTracking()
                .Where(c => c.IsActive && c.EndDate >= DateTime.UtcNow)
                .Include(c => c.Department)
                .Select(c => new {
                    c.CompetitionId,
                    c.Title,
                    c.StartDate,
                    c.EndDate,
                    c.SubmissionDeadline,
                    c.ImageUrl,
                    c.Category,
                    DepartmentName = c.Department != null ? c.Department.Name : null,
                    SubmissionsCount = _context.CompetitionSubmissions.Count(s => s.CompetitionId == c.CompetitionId)
                })
                .OrderBy(c => c.SubmissionDeadline)
                .ToListAsync();

            return Ok(activeCompetitions);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var competition = await _context.Competitions.FindAsync(id);
            if (competition == null) return NotFound(new { message = "المسابقة غير موجودة" });

           
            var hasSubmissions = await _context.CompetitionSubmissions.AnyAsync(s => s.CompetitionId == id);
            if (hasSubmissions)
            {
                return BadRequest(new { message = "لا يمكن حذف المسابقة لوجود مشاركات فيها. يمكنك تعطيلها (Deactivate) بدلاً من الحذف." });
            }

            _context.Competitions.Remove(competition);
            await _context.SaveChangesAsync();
            return Ok(new { message = "تم حذف المسابقة بنجاح" });
        }
    }
}