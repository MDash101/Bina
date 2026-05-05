using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication_School.Data;
using WebApplication_School.Models;

namespace WebApplication_School.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            bool isAdminOrTeacher = (User.Identity?.IsAuthenticated ?? false) && (User.IsInRole("Admin") || User.IsInRole("Teacher")); if (isAdminOrTeacher)
            {
                var projects = await _context.Projects
                .AsNoTracking()
                .Include(p => p.Owner)
                .Include(p => p.Department)
                .Select(p => new
                {
                    p.ProjectsId,
                    p.Title,
                    p.Description,
                    p.OwnerId,
                    OwnerName = p.Owner != null ? p.Owner.FullName : null,
                    p.DepartmentId,
                    DepartmentName = p.Department != null ? p.Department.Name : null,
                    p.CompetitionId,
                    p.Status,
                    p.Complexity,
                    p.CreatedDate,
                    p.StartDate,
                    p.ExpectedEndDate,
                    p.IsCompetitionEntry,
                    p.IsPublic,
                    p.ViewCount,
                    p.MilestoneCompletionPercentage,
                    p.MilestoneIsCompleted
                })
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

                return Ok(projects);
            }
            var publicData = await _context.Projects
    .AsNoTracking()
    .Where(p => p.IsPublic == true) 
    .Include(p => p.Owner)
    .Include(p => p.Department)
    .Select(p => new
    {
        p.ProjectsId,
        p.Title,
        p.Description,
        p.OwnerId,
        OwnerName = p.Owner != null ? p.Owner.FullName : null,
        p.DepartmentId,
        DepartmentName = p.Department != null ? p.Department.Name : null,
        p.Status,
        p.CreatedDate,
        p.ViewCount,
        p.MilestoneCompletionPercentage
    })
    .OrderByDescending(p => p.CreatedDate)
    .ToListAsync();

            return Ok(publicData);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid project id");

            var project = await _context.Projects
                .AsNoTracking()
                .Include(p => p.Owner)
                .Include(p => p.Department)
                .Include(p => p.Competition)
                .FirstOrDefaultAsync(p => p.ProjectsId == id);

            if (project == null)
                return NotFound("المشروع غير موجود");

            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Projects project)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

          
            var ownerExists = await _context.Users
                .AnyAsync(u => u.UsersId == project.OwnerId);

            if (!ownerExists)
                return BadRequest("المالك غير موجود");

            
            var duplicate = await _context.Projects
                .AnyAsync(p => p.Title == project.Title);

            if (duplicate)
                return BadRequest("يوجد مشروع بنفس الاسم");

            project.CreatedDate = DateTime.UtcNow;

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Projects updated)
        {
            if (id <= 0)
                return BadRequest("Invalid id");

            var project = await _context.Projects.FindAsync(id);

            if (project == null)
                return NotFound("المشروع غير موجود");

            
            if (!string.IsNullOrWhiteSpace(updated.Title))
                project.Title = updated.Title;

            if (!string.IsNullOrWhiteSpace(updated.Description))
                project.Description = updated.Description;

            project.Status = updated.Status;
            project.Complexity = updated.Complexity;
            project.StartDate = updated.StartDate;
            project.ExpectedEndDate = updated.ExpectedEndDate;
            project.IsCompetitionEntry = updated.IsCompetitionEntry;
            project.IsPublic = updated.IsPublic;
            project.MilestoneCompletionPercentage = updated.MilestoneCompletionPercentage;
            project.MilestoneIsCompleted = updated.MilestoneIsCompleted;
            project.DepartmentId = updated.DepartmentId;
            project.CompetitionId = updated.CompetitionId;

            await _context.SaveChangesAsync();

            return Ok(project);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id");

            var project = await _context.Projects.FindAsync(id);

            if (project == null)
                return NotFound("المشروع غير موجود");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف المشروع بنجاح" });
        }

        [HttpPost("view/{id}")]
        public async Task<IActionResult> IncreaseView(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid id");

            var affected = await _context.Database.ExecuteSqlRawAsync(
                "UPDATE Projects SET ViewCount = ViewCount + 1 WHERE ProjectsId = {0}", id);

            if (affected == 0)
                return NotFound("المشروع غير موجود");

            return Ok(new { message = "تم تحديث عدد المشاهدات" });
        }
    }
}