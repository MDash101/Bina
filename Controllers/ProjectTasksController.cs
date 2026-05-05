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
    public class ProjectTasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetByProject(int projectId)
        {
            if (projectId <= 0)
                return BadRequest(new { message = "Invalid project id" });

            var tasks = await _context.ProjectTasks
                .AsNoTracking()
                .Include(t => t.AssignedToUser)
                .Include(t => t.CreatedByUser)
                .Where(t => t.ProjectId == projectId)
                .Select(t => new
                {
                    t.TaskId,
                    t.ProjectId,
                    t.Title,
                    t.Description,
                    t.AssignedTo,
                    AssignedToName = t.AssignedToUser != null ? t.AssignedToUser.FullName : null,
                    t.CreatedBy,
                    CreatedByName = t.CreatedByUser != null ? t.CreatedByUser.FullName : null,
                    t.Status,
                    t.DueDate,
                    t.Priority
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid id" });

            var task = await _context.ProjectTasks
                .AsNoTracking()
                .Include(t => t.AssignedToUser)
                .Include(t => t.CreatedByUser)
                .Select(t => new {
                    t.TaskId,
                    t.ProjectId,
                    t.Title,
                    t.Description,
                    t.AssignedTo,
                    AssignedToName = t.AssignedToUser != null ? t.AssignedToUser.FullName : null,
                    t.CreatedBy,
                    CreatedByName = t.CreatedByUser.FullName,
                    t.Status,
                    t.DueDate,
                    t.Priority
                })
                .FirstOrDefaultAsync(t => t.TaskId == id);

            if (task == null)
                return NotFound(new { message = "المهمة غير موجودة" });

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectTasks task)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var projectExists = await _context.Projects.AnyAsync(p => p.ProjectsId == task.ProjectId);
            if (!projectExists)
                return BadRequest(new { message = "المشروع غير موجود" });

           
            if (task.Status == 0) task.Status = ProjectTaskStatus.Todo;

            _context.ProjectTasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = task.TaskId }, new { message = "تمت الإضافة", id = task.TaskId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectTasks updated)
        {
            var task = await _context.ProjectTasks.FindAsync(id);

            if (task == null)
                return NotFound(new { message = "المهمة غير موجودة" });

            if (!string.IsNullOrWhiteSpace(updated.Title)) task.Title = updated.Title;
            if (!string.IsNullOrWhiteSpace(updated.Description)) task.Description = updated.Description;

            task.AssignedTo = updated.AssignedTo;
            task.Status = updated.Status;
            task.DueDate = updated.DueDate;
            task.Priority = updated.Priority;

            await _context.SaveChangesAsync();

            return Ok(new { message = "تم التحديث بنجاح" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null)
                return NotFound(new { message = "المهمة غير موجودة" });

            _context.ProjectTasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف المهمة بنجاح" });
        }
    }
}