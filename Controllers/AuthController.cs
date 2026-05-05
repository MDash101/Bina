using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication_School.Data;
using WebApplication_School.DTOs;
using WebApplication_School.Models;

namespace WebApplication_School.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
        
            if (await _context.Users.AsNoTracking().AnyAsync(u => u.Email == model.Email))
                return BadRequest("هذا البريد الإلكتروني مسجل بالفعل.");

            if (model.DepartmentId > 0)
            {
                var deptExists = await _context.Departments.AnyAsync(d => d.DepartmentId == model.DepartmentId);
                if (!deptExists) return BadRequest("القسم المختار غير موجود.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

              
                var user = new Users
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = passwordHash,
                    Role = UserRole.Student,
                    DepartmentId = model.DepartmentId > 0 ? model.DepartmentId : null,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    LastLoginDate = DateTime.UtcNow,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    DateOfBirth = model.DateOfBirth
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var names = model.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var fName = names.Length > 0 ? names[0] : "New";
                var lName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "User";

                var profile = new UserProfiles
                {
                    UserId = user.UsersId,
                    FirstName = fName,
                    LastName = lName,
                    AcademicYear = "First Year", 
                    SkillsSummary = "No skills added yet", 
                    IsPublic = true,
                    Status = UserStatus.Active 
                };

                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { message = "تم التسجيل بنجاح وإنشاء الملف الشخصي" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Registration Error: {ex.Message}");
                return StatusCode(500, "حدث خطأ داخلي أثناء التسجيل. تأكد من إعدادات قاعدة البيانات.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                return Unauthorized("بيانات الدخول غير صحيحة.");

            if (!user.IsActive)
                return BadRequest("الحساب غير مفعل.");

            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                role = user.Role.ToString(),
                message = "تم تسجيل الدخول بنجاح"
            });
        }

        private string GenerateJwtToken(Users user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyString = jwtSettings["Key"];

            if (string.IsNullOrEmpty(keyString))
                throw new Exception("JWT Key is missing in appsettings.json");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UsersId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("FullName", user.FullName)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}