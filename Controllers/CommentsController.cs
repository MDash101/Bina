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
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        [AllowAnonymous] 
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetProjectComments(int projectId)
        {
            var comments = await _context.Comments
                .AsNoTracking()
                .Where(c => c.ProjectId == projectId && c.ParentCommentId == null && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedDate)
                .Select(c => new
                {
                    c.Id,
                    c.Content,
                    c.CreatedDate,
                    c.IsEdited,
                    c.LikeCount,
                    UserFullName = c.User.FullName,
                
                    Replies = c.Replies
                        .Where(r => !r.IsDeleted)
                        .OrderBy(r => r.CreatedDate)
                        .Select(r => new
                        {
                            r.Id,
                            r.Content,
                            r.CreatedDate,
                            UserFullName = r.User.FullName
                        })
                })
                .ToListAsync();

            return Ok(comments);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Comments comment)
        {
            if (string.IsNullOrWhiteSpace(comment.Content))
                return BadRequest(new { message = "التعليق فارغ" });

            
            comment.UserId = GetCurrentUserId();
            comment.CreatedDate = DateTime.UtcNow;
            comment.IsDeleted = false;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تمت إضافة التعليق", id = comment.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Comments updatedComment)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null || comment.IsDeleted)
                return NotFound();

           
            if (comment.UserId != GetCurrentUserId())
                return Forbid();

            comment.Content = updatedComment.Content;
            comment.UpdatedDate = DateTime.UtcNow;
            comment.IsEdited = true;

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم التعديل" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null) return NotFound();

            if (comment.UserId != GetCurrentUserId() && !User.IsInRole("Admin"))
                return Forbid();

            comment.IsDeleted = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم الحذف" });
        }
    }
}