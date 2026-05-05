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
    public class CompetitionDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompetitionDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("{competitionId}")]
        public async Task<IActionResult> GetByCompetitionId(int competitionId)
        {
            if (competitionId <= 0) return BadRequest(new { message = "معرف غير صحيح" });

            var details = await _context.CompetitionDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.CompetitionId == competitionId);

            if (details == null)
                return NotFound(new { message = "تفاصيل المسابقة غير موجودة" });

            return Ok(details);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CompetitionDetails details)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var competition = await _context.Competitions.AnyAsync(c => c.CompetitionId == details.CompetitionId);
            if (!competition) return BadRequest(new { message = "المسابقة الأصلية غير موجودة" });

            var alreadyExists = await _context.CompetitionDetails.AnyAsync(d => d.CompetitionId == details.CompetitionId);
            if (alreadyExists) return BadRequest(new { message = "هذه المسابقة تملك تفاصيل بالفعل" });

            if (!ValidateDates(details.StartDate, details.EndDate, details.SubmissionDeadline))
                return BadRequest(new { message = "تاريخ الانتهاء يجب أن يكون بعد البدء، وموعد التسليم لا يتجاوز الانتهاء" });

            _context.CompetitionDetails.Add(details);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByCompetitionId), new { competitionId = details.CompetitionId }, details);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CompetitionDetails updated)
        {
            var details = await _context.CompetitionDetails.FindAsync(id);
            if (details == null) return NotFound(new { message = "التفاصيل غير موجودة" });

            if (!ValidateDates(updated.StartDate, updated.EndDate, updated.SubmissionDeadline))
                return BadRequest(new { message = "خطأ في ترتيب التواريخ" });

            if (updated.MinParticipants > updated.MaxParticipants)
                return BadRequest(new { message = "خطأ في حدود المشاركين" });

            details.Title = updated.Title;
            details.Description = updated.Description;
            details.EvaluationCriteria = updated.EvaluationCriteria;
            details.StartDate = updated.StartDate;
            details.EndDate = updated.EndDate;
            details.SubmissionDeadline = updated.SubmissionDeadline;
            details.MinParticipants = updated.MinParticipants;
            details.MaxParticipants = updated.MaxParticipants;
            details.CompetitionType = updated.CompetitionType;
            details.PrizeDetails = updated.PrizeDetails;
            details.IsActive = updated.IsActive;
            details.RulesUrl = updated.RulesUrl;

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم التحديث بنجاح", data = details });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var details = await _context.CompetitionDetails.FindAsync(id);
            if (details == null) return NotFound(new { message = "السجل غير موجود" });

            _context.CompetitionDetails.Remove(details);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف التفاصيل بنجاح" });
        }

        private bool ValidateDates(DateTime start, DateTime end, DateTime? submission)
        {
            if (end <= start) return false;
            if (submission.HasValue && (submission > end || submission < start)) return false;
            return true;
        }
    }
}