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
    public class SkillsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SkillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var skills = await _context.Skills
                .AsNoTracking()
                .Select(s => new {
                    s.SkillId,
                    s.Name,
                    s.Category,
                    DepartmentName = s.Department != null ? s.Department.Name : "عام",
                    s.DifficultyLevel,
                    s.IsCoreSkills
                }).ToListAsync();

            return Ok(skills);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id");

            var skill = await _context.Skills
                .AsNoTracking()
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.SkillId == id);

            if (skill == null)
                return NotFound("المهارة غير موجودة");

            return Ok(skill);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Skills skill)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exists = await _context.Skills.AnyAsync(s => s.Name.ToLower() == skill.Name.ToLower());
            if (exists) return BadRequest("هذه المهارة مضافة مسبقاً");

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();
            return Ok(new { message = "تمت إضافة المهارة بواسطة المسؤول", data = skill });
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Skills updated)
        {
            if (id <= 0)
                return BadRequest();

            var skill = await _context.Skills.FindAsync(id);

            if (skill == null)
                return NotFound("المهارة غير موجودة");

            skill.Name = updated.Name;
            skill.Category = updated.Category;
            skill.Description = updated.Description;
            skill.DifficultyLevel = updated.DifficultyLevel;
            skill.IsCoreSkills = updated.IsCoreSkills;
            skill.DepartmentId = updated.DepartmentId;

            await _context.SaveChangesAsync();

            return Ok(skill);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound();

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            return Ok(new { message = "تم حذف المهارة نهائياً من النظام" });
        }
    }
}