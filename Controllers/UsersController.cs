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
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

     

        public class UserDto
        {
            public int UsersId { get; set; }
            public required string Email { get; set; }
            public required string Role { get; set; }

            public int? DepartmentId { get; set; }
            public string? DepartmentName { get; set; }

            public required string FullName { get; set; }
            public bool IsEmailVerified { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? LastLoginDate { get; set; }
            public bool IsActive { get; set; }

            public string? PhoneNumber { get; set; }
            public string? Address { get; set; }
            public DateTime? DateOfBirth { get; set; }
        }

        public class CreateUserDto
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
            public required string FullName { get; set; }

            public int? DepartmentId { get; set; }

            public string? PhoneNumber { get; set; }
            public string? Address { get; set; }
            public DateTime? DateOfBirth { get; set; }
        }

        public class UpdateUserDto
        {
            public required string Email { get; set; }
            public required string Role { get; set; }

            public int? DepartmentId { get; set; }

            public required string FullName { get; set; }

            public bool IsEmailVerified { get; set; }
            public bool IsActive { get; set; }

            public string? PhoneNumber { get; set; }
            public string? Address { get; set; }
            public DateTime? DateOfBirth { get; set; }
        }

        

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .Include(u => u.Department)
                .Select(u => new UserDto
                {
                    UsersId = u.UsersId,
                    Email = u.Email,
                    Role = u.Role.ToString(), 
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department != null ? u.Department.Name : null,
                    FullName = u.FullName,
                    IsEmailVerified = u.IsEmailVerified,
                    CreatedDate = u.CreatedDate,
                    LastLoginDate = u.LastLoginDate,
                    IsActive = u.IsActive,
                    PhoneNumber = u.PhoneNumber,
                    Address = u.Address,
                    DateOfBirth = u.DateOfBirth
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .Where(u => u.UsersId == id)
                .Select(u => new UserDto
                {
                    UsersId = u.UsersId,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department != null ? u.Department.Name : null,
                    FullName = u.FullName,
                    IsEmailVerified = u.IsEmailVerified,
                    CreatedDate = u.CreatedDate,
                    LastLoginDate = u.LastLoginDate,
                    IsActive = u.IsActive,
                    PhoneNumber = u.PhoneNumber,
                    Address = u.Address,
                    DateOfBirth = u.DateOfBirth
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("المستخدم غير موجود");

            return Ok(user);
        }



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
                return BadRequest("البريد الإلكتروني مستخدم بالفعل");

        
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

         
            var user = new Users
            {
                Email = dto.Email,
                PasswordHash = hashedPassword, 
                FullName = dto.FullName,
                DepartmentId = dto.DepartmentId,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,

                Role = UserRole.Student,
                IsActive = true,
                IsEmailVerified = false,
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User created successfully", user.UsersId });
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("المستخدم غير موجود");

            if (!Enum.TryParse<UserRole>(dto.Role, out var parsedRole))
            {
                return BadRequest("رتبة المستخدم المرسلة غير صحيحة. القيم المتاحة هي: Admin, Student, Lecturer");
            }

            
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Role = parsedRole; 
            user.DepartmentId = dto.DepartmentId;
            user.IsEmailVerified = dto.IsEmailVerified;
            user.IsActive = dto.IsActive;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;
            user.DateOfBirth = dto.DateOfBirth;

          
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تحديث المستخدم بنجاح" });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("المستخدم غير موجود");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف المستخدم بنجاح" });
        }

      

        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("المستخدم غير موجود");

            user.IsActive = false;

            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تعطيل الحساب" });
        }

       

        [HttpPut("{id}/login")]
        public async Task<IActionResult> UpdateLastLogin(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("المستخدم غير موجود");

            user.LastLoginDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تحديث آخر تسجيل دخول" });
        }
    }
}