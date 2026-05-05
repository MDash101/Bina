using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication_School.Data;
using WebApplication_School.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication_School.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Teacher")]
    public class ProjectMembersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectMembersController(ApplicationDbContext context)
        {
            _context = context;
        }

     
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetByProject(int projectId)
        {
            var members = await _context.ProjectMembers
                .Include(m => m.User)
                .Where(m => m.ProjectId == projectId)
                .Select(m => new
                {
                    m.ProjectMemberId,
                    m.ProjectId,
                    m.UserId,
                    m.Role,
                    m.JoinedDate,
                    m.IsActive,
                    m.Status,
                    m.Permissions,
                    UserName = m.User.FullName,
                    UserEmail = m.User.Email
                })
                .ToListAsync();

            return Ok(members);
        }

       
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectMembers member)
        {
           
            var projectExists = await _context.Projects
                .AnyAsync(p => p.ProjectsId == member.ProjectId);

            if (!projectExists)
                return BadRequest(new { message = "المشروع غير موجود" });

           
            var userExists = await _context.Users
                .AnyAsync(u => u.UsersId == member.UserId);

            if (!userExists)
                return BadRequest(new { message = "المستخدم غير موجود" });

           
            var isAlreadyMember = await _context.ProjectMembers
                .AnyAsync(m => m.ProjectId == member.ProjectId && m.UserId == member.UserId);

            if (isAlreadyMember)
                return BadRequest(new { message = "هذا المستخدم عضو بالفعل في هذا المشروع" });

          
            member.JoinedDate = DateTime.UtcNow;

            _context.ProjectMembers.Add(member);
            await _context.SaveChangesAsync();

        
            return CreatedAtAction(nameof(GetByProject), new { projectId = member.ProjectId }, member);
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectMembers updated)
        {
            var member = await _context.ProjectMembers.FindAsync(id);

            if (member == null)
                return NotFound(new { message = "عضو المشروع غير موجود" });

            
            if (!string.IsNullOrWhiteSpace(updated.Role))
                member.Role = updated.Role;

            member.Status = updated.Status;
            member.IsActive = updated.IsActive;
            member.Permissions = updated.Permissions;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "حدث خطأ أثناء التحديث، قد تكون البيانات تغيرت من مصدر آخر" });
            }

            return Ok(new { message = "تم التحديث بنجاح", data = member });
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _context.ProjectMembers.FindAsync(id);

            if (member == null)
                return NotFound(new { message = "العضو غير موجود بالفعل" });

            _context.ProjectMembers.Remove(member);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف العضو من المشروع بنجاح" });
        }
    }
}