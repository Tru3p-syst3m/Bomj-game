using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using valera3.Models;
using valera3.Services;

namespace valera3.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ValeraDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(ValeraDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Проверяем, существует ли уже пользователь с таким email
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                return BadRequest("Пользователь с таким email уже существует");
            }

            var existingUserName = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (existingUser != null)
            {
                return BadRequest("Пользователь с таким Username уже существует");
            }

            // Создаем нового пользователя
            var user = new User
            {
                Email = model.Email,
                Username = model.Username,
                PasswordHash = PasswordService.HashPassword(model.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            //ADMIN'а можно допилить здесь, типа если id = 1 то выдаем роль админа

            // Генерируем JWT токен
            var token = _jwtService.GenerateToken(user);
            return Ok(new { Token = token, User = model.Username, Message = "Пользователь успешно зарегистрирован" });
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginModel model)
        {
            // Находим пользователя по email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                return Unauthorized("Неверный email или пароль");
            }

            // Проверяем пароль
            if (!PasswordService.VerifyPassword(model.Password, user.PasswordHash))
            {
                return Unauthorized("Неверный email или пароль");
            }

            // Генерируем JWT токен
            var token = _jwtService.GenerateToken(user);
            return Ok(new { Token = token, Message = "Вход выполнен успешно" });
        }
    }

    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
