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
    public class FileStorageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FileStorageController(ApplicationDbContext context)
        {
            _context = context;
        }

   
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return 0;
            return int.Parse(userIdClaim);
        }

      
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetCurrentUserId();

            var files = await _context.FileStorages
                .AsNoTracking()
                .Include(f => f.Uploader)
                .Where(f => f.IsPublic || f.UploaderId == userId)
                .OrderByDescending(f => f.UploadDate)
                .Select(f => new
                {
                    f.FileId,
                    f.FileName,
                    f.FilePath,
                    f.FileSize,
                    f.FileType,
                    f.UploadDate,
                    f.IsPublic,
                    f.DownloadCount,
                    f.Description,
                    f.Category,
                    f.UploaderId,
                    UploaderName = f.Uploader != null ? f.Uploader.FullName : "Unknown"
                })
                .ToListAsync();

            return Ok(files);
        }

    
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetCurrentUserId();

            var file = await _context.FileStorages
                .AsNoTracking()
                .Include(f => f.Uploader)
                .FirstOrDefaultAsync(f => f.FileId == id);

            if (file == null)
                return NotFound(new { message = "الملف غير موجود" });

         
            if (!file.IsPublic && file.UploaderId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return Ok(file);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FileStorage file)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();

            file.UploaderId = userId;
            file.UploadDate = DateTime.UtcNow;
            file.DownloadCount = 0;

            _context.FileStorages.Add(file);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = file.FileId }, file);
        }

     
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FileStorage updated)
        {
            var file = await _context.FileStorages.FindAsync(id);

            if (file == null)
                return NotFound(new { message = "الملف غير موجود" });

            var userId = GetCurrentUserId();

           
            if (file.UploaderId != userId && !User.IsInRole("Admin"))
                return Forbid();

            if (string.IsNullOrWhiteSpace(updated.FileName))
                return BadRequest(new { message = "اسم الملف مطلوب" });

           
            file.FileName = updated.FileName;
            file.FilePath = updated.FilePath;
            file.FileSize = updated.FileSize;
            file.FileType = updated.FileType;
            file.Description = updated.Description;
            file.Category = updated.Category;
            file.IsPublic = updated.IsPublic;

            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تحديث بيانات الملف", data = file });
        }

    
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var file = await _context.FileStorages.FindAsync(id);

            if (file == null)
                return NotFound(new { message = "الملف غير موجود" });

            var userId = GetCurrentUserId();

            if (file.UploaderId != userId && !User.IsInRole("Admin"))
                return Forbid();

            _context.FileStorages.Remove(file);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف السجل من قاعدة البيانات بنجاح" });
        }

       
        [HttpPatch("download/{id}")]
        public async Task<IActionResult> IncreaseDownloadCount(int id)
        {
            var file = await _context.FileStorages.FindAsync(id);

            if (file == null)
                return NotFound(new { message = "الملف غير موجود" });

            
            var userId = GetCurrentUserId();
            if (!file.IsPublic && file.UploaderId != userId && !User.IsInRole("Admin"))
                return Forbid();

            file.DownloadCount++;
            await _context.SaveChangesAsync();

            return Ok(new { currentDownloads = file.DownloadCount });
        }
    }
}