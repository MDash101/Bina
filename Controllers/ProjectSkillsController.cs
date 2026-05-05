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
    public class ProjectSkillsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectSkillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetByProject(int projectId)
        {
            if (projectId <= 0)
                return BadRequest("Invalid project id");

            var skills = await _context.ProjectSkills
                .AsNoTracking()
                .Include(s => s.Skill)
                .Include(s => s.FilledByUser)
                .Where(s => s.ProjectId == projectId)
                .Select(s => new
                {
                    s.ProjectSkillId,
                    s.ProjectId,
                    s.SkillId,
                    SkillName = s.Skill != null ? s.Skill.Name : null,
                    s.SkillLevelRequired,
                    s.IsFilled,
                    s.FilledByUserId,
                    FilledByUserName = s.FilledByUser != null ? s.FilledByUser.FullName : null,
                    s.Notes
                })
                .ToListAsync();

            return Ok(skills);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectSkills skill)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectsId == skill.ProjectId);
            if (project == null) return BadRequest("المشروع غير موجود");

        
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int currentUserId = int.Parse(userIdClaim);

         
            if (project.OwnerId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            
            var skillExists = await _context.Skills.AnyAsync(s => s.SkillId == skill.SkillId);
            if (!skillExists) return BadRequest("المهارة غير موجودة");

            var duplicate = await _context.ProjectSkills.AnyAsync(s => s.ProjectId == skill.ProjectId && s.SkillId == skill.SkillId);
            if (duplicate) return BadRequest("هذه المهارة مضافة بالفعل لهذا المشروع");

            skill.IsFilled = skill.FilledByUserId != null;
            _context.ProjectSkills.Add(skill);
            await _context.SaveChangesAsync();

            return Ok(skill);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectSkills updated)
        {
            var skill = await _context.ProjectSkills
                .Include(ps => ps.Project)
                .FirstOrDefaultAsync(ps => ps.ProjectSkillId == id);

            if (skill == null) return NotFound("السجل غير موجود");

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int currentUserId = int.Parse(userIdClaim);

           
            if (skill.Project.OwnerId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            
            skill.SkillLevelRequired = updated.SkillLevelRequired;
            skill.FilledByUserId = updated.FilledByUserId;
            skill.IsFilled = updated.FilledByUserId != null;
            skill.Notes = updated.Notes;

            await _context.SaveChangesAsync();
            return Ok(skill);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id");

            var skill = await _context.ProjectSkills.FindAsync(id);

            if (skill == null)
                return NotFound("المهارة غير موجودة");

            _context.ProjectSkills.Remove(skill);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف المهارة من المشروع" });
        }
    }
}