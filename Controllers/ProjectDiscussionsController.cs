using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication_School.Data;
using WebApplication_School.Models;

namespace WebApplication_School.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectDiscussionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectDiscussionsController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetByProject(int projectId)
        {
           
            var isMember = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == CurrentUserId);

            if (!isMember)
                return Forbid("عذراً، لا يمكنك رؤية مناقشات مشروع لست عضواً فيه.");

            var discussions = await _context.ProjectDiscussions
                .AsNoTracking() 
                .Where(d => d.ProjectId == projectId)
                .Select(d => new
                {
                    d.DiscussionId,
                    d.Title,
                    d.CreatedDate,
                    d.IsPinned,
                    d.LastActivityDate,
                    d.CreatedById,
                    CreatedByName = d.CreatedBy.FullName,
                    MessagesCount = d.Messages.Count()
                })
                .OrderByDescending(d => d.IsPinned)
                .ThenByDescending(d => d.LastActivityDate)
                .ToListAsync();

            return Ok(discussions);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDiscussionDto dto)
        {
      
            var isMember = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == dto.ProjectId && pm.UserId == CurrentUserId);

            if (!isMember)
                return Forbid("لا يمكنك إنشاء مناقشة في مشروع لست عضواً فيه.");

            var discussion = new ProjectDiscussions
            {
                Title = dto.Title,
                ProjectId = dto.ProjectId,
                CreatedById = CurrentUserId, 
                CreatedDate = DateTime.UtcNow,
                LastActivityDate = DateTime.UtcNow,
                IsPinned = false
            };

            _context.ProjectDiscussions.Add(discussion);
            await _context.SaveChangesAsync();

            return Ok(discussion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDiscussionDto dto)
        {
            var discussion = await _context.ProjectDiscussions.FindAsync(id);

            if (discussion == null) return NotFound();

          
            if (discussion.CreatedById != CurrentUserId)
                return Forbid("لا تملك صلاحية تعديل هذه المناقشة.");

            if (!string.IsNullOrWhiteSpace(dto.Title))
                discussion.Title = dto.Title;

            discussion.IsPinned = dto.IsPinned;
            discussion.LastActivityDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(discussion);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var discussion = await _context.ProjectDiscussions.FindAsync(id);

            if (discussion == null) return NotFound();

          
            if (discussion.CreatedById != CurrentUserId)
                return Forbid("لا تملك صلاحية حذف هذه المناقشة.");

            _context.ProjectDiscussions.Remove(discussion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف المناقشة بنجاح" });
        }
    }

   
    public class CreateDiscussionDto
    {
        public int ProjectId { get; set; }
        public required string Title { get; set; }
    }

    public class UpdateDiscussionDto
    {
        public string? Title { get; set; }
        public bool IsPinned { get; set; }
    }
}