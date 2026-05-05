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
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // دالة مساعدة للحصول على ID المستخدم الحالي
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        [HttpGet("my-notifications")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = GetCurrentUserId();

            var notifications = await _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => new
                {
                    n.NotificationId,
                    n.Title,
                    Message = n.Content,
                    n.CreatedDate,
                    n.IsRead,
                    n.Type
                })
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Notifications notification)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userExists = await _context.Users
                .AnyAsync(u => u.UsersId == notification.UserId);

            if (!userExists)
                return BadRequest(new { message = "المستخدم غير موجود" });

            notification.CreatedDate = DateTime.UtcNow;
            notification.IsRead = false;

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم إرسال الإشعار بنجاح", id = notification.NotificationId });
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);

            if (notification == null)
                return NotFound(new { message = "الإشعار غير موجود" });

            // حماية: لا يمكن للمستخدم تحديث إشعار لا يخصه
            if (notification.UserId != GetCurrentUserId() && !User.IsInRole("Admin"))
                return Forbid();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تحديد الإشعار كمقروء" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);

            if (notification == null)
                return NotFound(new { message = "الإشعار غير موجود" });

            // حماية: لا يمكن للمستخدم حذف إشعار لا يخصه
            if (notification.UserId != GetCurrentUserId() && !User.IsInRole("Admin"))
                return Forbid();

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف الإشعار بنجاح" });
        }
    }
}