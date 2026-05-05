using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication_School.Data;
using WebApplication_School.DTOs;
using WebApplication_School.Models;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserSkillsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserSkillsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var skills = await _context.UserSkills
            .Include(us => us.Skill)
            .Include(us => us.LastEndorsedByUser)
            .Where(us => us.UserId == userId)
            .Select(us => new
            {
                us.UserSkillId,
                us.UserId,
                us.SkillId,
                SkillName = us.Skill.Name,
                us.Level,
                us.IsVerified,
                us.ExperienceDescription,
                us.Notes,
                us.EndorseCount,
                LastEndorsedByName = us.LastEndorsedByUser != null ? us.LastEndorsedByUser.FullName : null,
                us.LastEndorsedDate,
                us.CreatedAt
            })
            .ToListAsync();

        return Ok(skills);
    }

    

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var skill = await _context.UserSkills
            .Include(us => us.User)
            .Include(us => us.Skill)
            .FirstOrDefaultAsync(us => us.UserSkillId == id);

        if (skill == null)
            return NotFound("المهارة غير موجودة");

        return Ok(new
        {
            skill.UserSkillId,
            skill.UserId,
            skill.SkillId,
            SkillName = skill.Skill.Name,
            skill.Level,
            skill.IsVerified,
            skill.ExperienceDescription,
            skill.Notes,
            skill.EndorseCount,
            skill.CreatedAt
        });
    }

    

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserSkillDto dto)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UsersId == dto.UserId);
        var skillExists = await _context.Skills.AnyAsync(s => s.SkillId == dto.SkillId);

        if (!userExists || !skillExists)
            return BadRequest("المستخدم أو المهارة غير موجودة");

      
        var exists = await _context.UserSkills
            .AnyAsync(x => x.UserId == dto.UserId && x.SkillId == dto.SkillId);

        if (exists)
            return BadRequest("المهارة مضافة بالفعل");

        var entity = new UserSkills
        {
            UserId = dto.UserId,
            SkillId = dto.SkillId,
            Level = dto.Level,
            ExperienceDescription = dto.ExperienceDescription,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserSkills.Add(entity);
        await _context.SaveChangesAsync();

        return Ok(entity);
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserSkillDto dto)
    {
        var skill = await _context.UserSkills.FindAsync(id);

        if (skill == null)
            return NotFound("المهارة غير موجودة");

        skill.Level = dto.Level;
        skill.IsVerified = dto.IsVerified;
        skill.VerifiedBy = dto.VerifiedBy;
        skill.VerificationDate = dto.VerificationDate;
        skill.ExperienceDescription = dto.ExperienceDescription;
        skill.Notes = dto.Notes;

        await _context.SaveChangesAsync();

        return Ok(skill);
    }



    [HttpPut("{id}/endorse")]
    public async Task<IActionResult> Endorse(int id)
    {
        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        var skill = await _context.UserSkills.FindAsync(id);

        if (skill == null)
            return NotFound("المهارة غير موجودة");

        skill.EndorseCount++;
        skill.LastEndorsedBy = userId;
        skill.LastEndorsedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "تم دعم المهارة" });
    }

   

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var skill = await _context.UserSkills.FindAsync(id);

        if (skill == null)
            return NotFound("المهارة غير موجودة");

        _context.UserSkills.Remove(skill);
        await _context.SaveChangesAsync();

        return Ok(new { message = "تم حذف المهارة بنجاح" });
    }
}