using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication_School.Data;
using WebApplication_School.Models;

namespace WebApplication_School.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departments = await _context.Departments
                .AsNoTracking()
                .Include(d => d.Head)
                .Select(d => new
                {
                    d.DepartmentId,
                    d.Name,
                    d.Description,
                    d.ContactEmail,
                    d.IsActive,
                    d.HeadId,
                    HeadName = d.Head != null ? d.Head.FullName : "لا يوجد رئيس"
                })
                .ToListAsync();

            return Ok(departments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department = await _context.Departments
                .AsNoTracking()
                .Include(d => d.Head)
                .Where(d => d.DepartmentId == id)
                .Select(d => new
                {
                    d.DepartmentId,
                    d.Name,
                    d.Description,
                    d.ContactEmail,
                    d.IsActive,
                    d.HeadId,
                    HeadName = d.Head != null ? d.Head.FullName : null
                })
                .FirstOrDefaultAsync();

            if (department == null)
                return NotFound(new { message = "القسم غير موجود" });

            return Ok(department);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Department department)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

         
            var nameExists = await _context.Departments.AnyAsync(d => d.Name == department.Name);
            if (nameExists)
                return BadRequest(new { message = "هذا القسم موجود بالفعل" });

           
            if (department.HeadId.HasValue && department.HeadId > 0)
            {
                var headExists = await _context.Users.AnyAsync(u => u.UsersId == department.HeadId);
                if (!headExists)
                    return BadRequest(new { message = "المستخدم المحدد كرئيس غير موجود" });
            }

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = department.DepartmentId }, department);
        }

       [HttpPut("{id}")]
public async Task<IActionResult> Update(int id, [FromBody] Department updated)
{
    
    var department = await _context.Departments.FindAsync(id);
    if (department == null)
    {
        return NotFound(new { message = "القسم غير موجود" });
    }

   
    var nameExists = await _context.Departments.AnyAsync(d => d.Name == updated.Name && d.DepartmentId != id);
    if (nameExists)
    {
        return BadRequest(new { message = "يوجد قسم آخر بنفس هذا الاسم" });
    }

   
    if (updated.HeadId.HasValue && updated.HeadId > 0)
    {
        var headExists = await _context.Users.AnyAsync(u => u.UsersId == updated.HeadId);
        if (!headExists)
        {
            return BadRequest(new { message = "رئيس القسم المختار غير موجود في نظام المستخدمين" });
        }
    }

    
    department.Name = updated.Name;
    department.Description = updated.Description;
    department.ContactEmail = updated.ContactEmail;
    department.IsActive = updated.IsActive;
    department.HeadId = updated.HeadId;

    try
    {
        
        await _context.SaveChangesAsync();
        
        
        return Ok(new 
        { 
            message = "تم تحديث بيانات القسم بنجاح", 
            data = department 
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "حدث خطأ أثناء تحديث البيانات", error = ex.Message });
    }
}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return NotFound(new { message = "القسم غير موجود" });

            try
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
                return Ok(new { message = "تم حذف القسم بنجاح" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "لا يمكن حذف القسم لأنه مرتبط ببيانات أخرى (طلاب أو معلمين). يفضل تعطيله بدلاً من حذفه." });
            }
        }
    }
}