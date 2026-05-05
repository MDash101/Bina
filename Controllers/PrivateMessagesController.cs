using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApplication_School.Data;
using WebApplication_School.Models;


namespace WebApplication_School.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PrivateMessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PrivateMessagesController(ApplicationDbContext context)
        {
            _context = context;
        }


        private int CurrentUserId => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        [HttpGet("conversation")]
        public async Task<IActionResult> GetConversation(int otherUserId)
        {
            var currentId = CurrentUserId;


            var messages = await _context.PrivateMessages
                .AsNoTracking()
                .Where(m =>
                    (m.SenderId == currentId && m.ReceiverId == otherUserId) ||
                    (m.SenderId == otherUserId && m.ReceiverId == currentId))
                .OrderBy(m => m.SentDate)
                .Select(m => new
                {
                    m.MessageId,
                    m.SenderId,
                    m.ReceiverId,
                    m.Content,
                    m.SentDate,
                    m.IsRead,
                    SenderName = m.Sender.FullName,
                    ReceiverName = m.Receiver.FullName
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var currentId = CurrentUserId;

            if (currentId == dto.ReceiverId)
                return BadRequest("لا يمكنك مراسلة نفسك.");

            var receiverExists = await _context.Users.AnyAsync(u => u.UsersId == dto.ReceiverId);
            if (!receiverExists) return BadRequest("المستقبل غير موجود.");

            var message = new PrivateMessages
            {
                SenderId = currentId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content,
                SentDate = DateTime.UtcNow,
                IsRead = false
            };

            _context.PrivateMessages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _context.PrivateMessages.FindAsync(id);

            if (message == null) return NotFound();


            if (message.SenderId != CurrentUserId)
                return Forbid("لا تملك صلاحية حذف هذه الرسالة.");

            _context.PrivateMessages.Remove(message);
            await _context.SaveChangesAsync();
            return Ok(new { message = "تم الحذف" });
        }
    }


    public class SendMessageDto
    {
        public int ReceiverId { get; set; }

        [Required] 
        public required string Content { get; set; } = string.Empty;
    }
}