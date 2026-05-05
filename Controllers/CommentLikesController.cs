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
    public class CommentLikesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentLikesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("toggle/{commentId}")]
        public async Task<IActionResult> ToggleLike(int commentId)
        {
            var userIdClaim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);
            if (comment == null) return NotFound(new { message = "التعليق غير موجود" });

            var existingLike = await _context.CommentLikes
                .FirstOrDefaultAsync(l => l.CommentId == commentId && l.UserId == userId);

            bool isLikedNow;

            if (existingLike != null)
            {
               
                _context.CommentLikes.Remove(existingLike);
                comment.LikeCount = Math.Max(0, comment.LikeCount - 1);
                isLikedNow = false;
            }
            else
            {
            
                var newLike = new CommentLikes
                {
                    CommentId = commentId,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow
                };
                _context.CommentLikes.Add(newLike);
                comment.LikeCount += 1;
                isLikedNow = true;
            }

           
            await _context.SaveChangesAsync();

            return Ok(new { isLiked = isLikedNow, totalLikes = comment.LikeCount });
        }

        [AllowAnonymous]
        [HttpGet("count/{commentId}")]
        public async Task<IActionResult> GetLikesCount(int commentId)
        {
            if (commentId <= 0)
                return BadRequest();

            var count = await _context.CommentLikes
                .CountAsync(l => l.CommentId == commentId);

            return Ok(new { commentId, likesCount = count });
        }

        [AllowAnonymous]
        [HttpGet("users/{commentId}")]
        public async Task<IActionResult> GetUsersWhoLiked(int commentId)
        {
            if (commentId <= 0)
                return BadRequest();

            var users = await _context.CommentLikes
                .Where(l => l.CommentId == commentId)
                .Include(l => l.User)
                .Select(l => new
                {
                    l.UserId,
                    UserFullName = l.User.FullName,
                    l.CreatedDate
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("check/{commentId}")]
        public async Task<IActionResult> CheckIfLiked(int commentId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            var isLiked = await _context.CommentLikes
                .AnyAsync(l => l.CommentId == commentId && l.UserId == userId);

            return Ok(new { isLiked });
        }
    }
}