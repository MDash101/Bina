using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication_School.Data;
using WebApplication_School.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication_School.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class ProjectMediaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectMediaController(ApplicationDbContext context)
        {
            _context = context;
        }

     
        [AllowAnonymous]
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetByProject(int projectId)
        {
            var media = await _context.ProjectMedia
                .Where(m => m.ProjectId == projectId)
                .Select(m => new
                {
                    m.MediaId,
                    m.ProjectId,
                    m.FileName,
                    m.FileUrl,
                    FileType = m.FileType.ToString(), 
                    m.UploadDate,
                    m.Description,
                    m.IsPrimary,
                    m.FileSize,
                    m.ThumbnailUrl
                })
                .OrderByDescending(m => m.IsPrimary) 
                .ThenByDescending(m => m.UploadDate)
                .ToListAsync();

            return Ok(media);
        }

      
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectMedia media)
        {
            if (string.IsNullOrWhiteSpace(media.FileUrl))
                return BadRequest(new { message = "رابط الملف (FileUrl) مطلوب" });

            var projectExists = await _context.Projects
                .AnyAsync(p => p.ProjectsId == media.ProjectId);

            if (!projectExists)
                return BadRequest(new { message = "المشروع المحدد غير موجود" });

       
            if (media.IsPrimary)
            {
                await ResetPrimaryMedia(media.ProjectId);
            }

            media.UploadDate = DateTime.UtcNow;

            _context.ProjectMedia.Add(media);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByProject), new { projectId = media.ProjectId }, media);
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectMedia updated)
        {
            var media = await _context.ProjectMedia.FindAsync(id);

            if (media == null)
                return NotFound(new { message = "الملف غير موجود" });

            
            if (updated.IsPrimary && !media.IsPrimary)
            {
                await ResetPrimaryMedia(media.ProjectId);
            }

           
            if (!string.IsNullOrWhiteSpace(updated.FileName))
                media.FileName = updated.FileName;

            if (!string.IsNullOrWhiteSpace(updated.FileUrl))
                media.FileUrl = updated.FileUrl;

            media.FileType = updated.FileType;
            media.Description = updated.Description;
            media.IsPrimary = updated.IsPrimary;
            media.FileSize = updated.FileSize;
            media.ThumbnailUrl = updated.ThumbnailUrl;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "حدث خطأ أثناء التحديث، قد تكون البيانات تغيرت" });
            }

            return Ok(new { message = "تم التحديث بنجاح", data = media });
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var media = await _context.ProjectMedia.FindAsync(id);

            if (media == null)
                return NotFound(new { message = "الملف غير موجود بالفعل" });

            _context.ProjectMedia.Remove(media);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف الملف بنجاح" });
        }

     
        private async Task ResetPrimaryMedia(int projectId)
        {
            var primaryItems = await _context.ProjectMedia
                .Where(m => m.ProjectId == projectId && m.IsPrimary)
                .ToListAsync();

            foreach (var item in primaryItems)
            {
                item.IsPrimary = false;
            }
        }
    }
}