# Описание всех изменений в проекте

## Backend (.NET 6)

### 1. Создание модели User
**Файл:** `valera3/Models/User.cs`
**Строки:** 1-22
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace valera3.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
        
        [Required]
        public string Username { get; set; }
        
        public string Role { get; set; } = "User"; // По умолчанию "User"
        
        public List<Valera> Valeras { get; set; } = new List<Valera>();
    }
}
```

### 2. Обновление модели Valera
**Файл:** `valera3/Models/valera.cs`
**Строки:** 10-15
```csharp
        // Связь с пользователем
        public int UserId { get; set; }
        public User { get; set; }
```

### 3. Обновление ValeraDbContext
**Файл:** `valera3/Data/ValeraDbContext.cs`
**Строки:** 8, 22-38
```csharp
        public DbSet<User> Users { get; set; }
```

```csharp
            modelBuilder.Entity<Valera>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Health).IsRequired();
                entity.Property(e => e.Mana).IsRequired();
                entity.Property(e => e.Cheerfulness).IsRequired();
                entity.Property(e => e.Fatigue).IsRequired();
                entity.Property(e => e.Money).IsRequired();
                
                // Настройка связи Valera с User
                entity.HasOne(v => v.User)
                      .WithMany(u => u.Valeras)
                      .HasForeignKey(v => v.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Email).IsUnique();
            });
```

### 4. Создание JwtService
**Файл:** `valera3/Services/JwtService.cs`
**Строки:** 1-36
```csharp
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using valera3.Models;

namespace valera3.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration["Jwt:Expire"])),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
```

### 5. Создание PasswordService
**Файл:** `valera3/Services/PasswordService.cs`
**Строки:** 1-31
```csharp
using System.Security.Cryptography;
using System.Text;

namespace valera3.Services
{
    public class PasswordService
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }
    }
}
```

### 6. Создание AuthController
**Файл:** `valera3/Controllers/AuthController.cs`
**Строки:** 1-85
```csharp
using Microsoft.AspNetCore.Mvc;
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

            // Создаем нового пользователя
            var user = new User
            {
                Email = model.Email,
                Username = model.Username,
                PasswordHash = PasswordService.HashPassword(model.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Генерируем JWT токен
            var token = _jwtService.GenerateToken(user);
            return Ok(new { Token = token, Message = "Пользователь успешно зарегистрирован" });
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
```

### 7. Обновление ValeraService
**Файл:** `valera3/Services/ValeraService.cs`
**Строки:** 25-31, 33-38, 40-48, 50-71, 73-93, 95-117, 119-144, 146-171
```csharp
        // Новый метод для получения Валер конкретного пользователя
        public async Task<List<Valera>> GetUserValerasAsync(int userId)
        {
            return await _context.Valeras.Where(v => v.UserId == userId).ToListAsync();
        }
```

```csharp
        // Новый метод для получения Валеры с проверкой принадлежности пользователю
        public async Task<Valera> GetValeraByIdForUserAsync(int id, int userId)
        {
            var valera = await _context.Valeras.FindAsync(id);
            if (valera == null || valera.UserId != userId)
            {
                throw new KeyNotFoundException();
            }
            return valera;
        }
```

```csharp
        public async Task<Valera> CreateValeraAsync(Valera valera, int userId)
        {
            valera.UserId = userId; // Устанавливаем владельца
            _context.Valeras.Add(valera);
            await _context.SaveChangesAsync();
            return valera;
        }
```

```csharp
        public async Task<Valera?> UpdateValeraAsync(int id, Valera valera, int userId, string userRole = "User")
        {
            var existingValera = await _context.Valeras.FindAsync(id);
            if (existingValera == null)
                return null;
                
            // Проверяем, является ли пользователь владельцем Валеры или администратором
            if (existingValera.UserId != userId && userRole != "Admin")
            {
                return null; // Пользователь не может обновлять чужую Валеру
            }
            
            existingValera.Health = valera.Health;
            existingValera.Mana = valera.Mana;
            existingValera.Cheerfulness = valera.Cheerfulness;
            existingValera.Fatigue = valera.Fatigue;
            existingValera.Money = valera.Money;

            await _context.SaveChangesAsync();
            return existingValera;
        }
```

```csharp
        public async Task<bool> DeleteValeraAsync(int id, int userId, string userRole = "User")
        {
            var valera = await _context.Valeras.FindAsync(id);
            if (valera == null)
                return false;

            // Проверяем, является ли пользователь владельцем Валеры или администратором
            if (valera.UserId != userId && userRole != "Admin")
            {
                return false; // Пользователь не может удалять чужую Валеру
            }

            _context.Valeras.Remove(valera);
            await _context.SaveChangesAsync();
            return true;
        }
```

```csharp
        public async Task<bool> ExecuteActionAsync(int id, string actionName, int userId, string userRole = "User")
        {
            var valera = await _context.Valeras.FindAsync(id);
            if (valera == null)
                return false;

            // Проверяем, является ли пользователь владельцем Валеры или администратором
            if (valera.UserId != userId && userRole != "Admin")
            {
                return false; // Пользователь не может выполнять действия с чужой Валерой
            }

            bool result = false;
            if (valera.IsAlive)
            {
                result = actionName.ToLower() switch
                {
                    "work" => valera.GoToWork(),
                    "nature" => valera.ContemplateNature(),
                    "wine" => valera.DrinkWineAndWatchSeries(),
                    "bar" => valera.GoToBar(),
                    "marginals" => valera.DrinkWithMarginals(),
                    "sing" => valera.SingInMetro(),
                    "sleep" => valera.Sleep(),
                    _ => false,
                };
            }

            if (result)
            {
                await _context.SaveChangesAsync();
            }

            return result;
        }
```

```csharp
        public async Task<Valera?> ResetValeraAsync(int id, int userId, string userRole = "User")
        {
            var valera = await _context.Valeras.FindAsync(id);
            if (valera == null)
                return null;

            // Проверяем, является ли пользователь владельцем Валеры или администратором
            if (valera.UserId != userId && userRole != "Admin")
            {
                return null; // Пользователь не может сбрасывать чужую Валеру
            }

            valera.Health = 100;
            valera.Mana = 0;
            valera.Cheerfulness = 0;
            valera.Fatigue = 0;
            valera.Money = 100;

            await _context.SaveChangesAsync();
            return valera;
        }
```

### 8. Обновление ValeraController
**Файл:** `valera3/Controllers/ValeraController.cs`
**Строки:** 1, 2, 16-21, 23-30, 32-48, 50-58, 60-72, 74-86, 8-100, 102-114, 116-134
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
```

```csharp
        // Получить всех Валер (только для администраторов)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Valera>>> GetAll()
        {
            var valeras = await _valeraService.GetAllValerasAsync();
            return Ok(valeras);
        }
```

```csharp
        // Получить "своих" Валер (доступно авторизованным пользователям)
        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<List<Valera>>> GetMyValeras()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var valeras = await _valeraService.GetUserValerasAsync(userId);
            return Ok(valeras);
        }
```

```csharp
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Valera>> GetById(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            try
            {
                var valera = await _valeraService.GetValeraByIdForUserAsync(id, userId);
                return Ok(valera);
            }
            catch (KeyNotFoundException)
            {
                // Проверяем, является ли пользователь администратором
                if (userRole == "Admin")
                {
                    var valera = await _valeraService.GetValeraByIdAsync(id);
                    if (valera == null)
                        return NotFound();
                    return Ok(valera);
                }
                return NotFound();
            }
        }
```

```csharp
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Valera>> Create(Valera valera)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var created = await _valeraService.CreateValeraAsync(valera, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
```

```csharp
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Valera>> Update(int id, Valera valera)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            var updated = await _valeraService.UpdateValeraAsync(id, valera, userId, userRole);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }
```

```csharp
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            var result = await _valeraService.DeleteValeraAsync(id, userId, userRole);
            if (!result)
                return NotFound();
            return NoContent();
        }
```

```csharp
        [HttpPost("{id}/actions/{actionName}")]
        [Authorize]
        public async Task<ActionResult> ExecuteAction(int id, string actionName)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            var result = await _valeraService.ExecuteActionAsync(id, actionName, userId, userRole);
            if (!result)
                return Ok("Not enough stats");
            return Ok("Action executed successfully");
        }
```

```csharp
        [HttpPost("{id}/reset")]
        [Authorize]
        public async Task<ActionResult<Valera>> Reset(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            var valera = await _valeraService.ResetValeraAsync(id, userId, userRole);
            if (valera == null)
                return NotFound();
            return Ok(valera);
        }
```

### 9. Обновление Program.cs
**Файл:** `valera3/Program.cs`
**Строки:** 1, 2, 1-32, 44, 45, 51, 52
```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
```

```csharp
// Настройка JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"];
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
```

```csharp
builder.Services.AddScoped<JwtService>();
```

```csharp
// Аутентификация должна идти перед авторизацией
app.UseAuthentication();
```

### 10. Обновление appsettings.json
**Файл:** `valera3/appsettings.json`
**Строки:** 8-12
```json
  "Jwt": {
    "Key": "THIS IS USED TO SIGN AND VERIFY JWT TOKENS, REPLACE THIS WITH YOUR OWN SECRET",
    "Issuer": "ValeraApp",
    "Audience": "ValeraAppUser",
    "Expire": "00:30:00"
  }
```

### 11. Обновление проекта
**Файл:** `valera3/valera3.csproj`
**Строки:** 12-14
```xml
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="6.0.0" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="6.15.1" />
    <PackageReference Include="Microsoft.AspNetCore.Authorization" Version="6.0.0" />
```

## Frontend (React)

### 1. Обновление API сервиса
**Файл:** `valera-frontend/src/services/api.js`
**Строки:** 7-25, 28-37, 39-51, 53-61, 63-67, 69-81, 83-95, 97-109, 111-123, 125-145
```javascript
// Интерceptors для добавления токена к каждому запросу
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Интерceptors для обработки 401 ошибок
api.interceptors.response.use(
  (response) => response,
 (error) => {
    if (error.response?.status === 401) {
      // Удаляем токен и перенаправляем на страницу логина
      localStorage.removeItem('token');
      window.location.href = '/login'; // или другая страница логина
    }
    return Promise.reject(error);
  }
);
```

```javascript
// Аутентификация
export const register = async (userData) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/auth/register`, userData);
    if (response.data.Token) {
      localStorage.setItem('token', response.data.Token);
    }
    return response.data;
  } catch (error) {
    console.error('Ошибка при регистрации:', error);
    throw error;
 }
};
```

```javascript
export const login = async (userData) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/auth/login`, userData);
    if (response.data.Token) {
      localStorage.setItem('token', response.data.Token);
    }
    return response.data;
  } catch (error) {
    console.error('Ошибка при входе:', error);
    throw error;
  }
};
```

```javascript
export const logout = () => {
  localStorage.removeItem('token');
};
```

```javascript
export const getCurrentUser = () => {
  const token = localStorage.getItem('token');
  if (!token) return null;

  // Декодируем JWT токен, чтобы получить информацию о пользователе
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
      return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
  } catch (error) {
    console.error('Ошибка при декодировании токена:', error);
    return null;
 }
};
```

```javascript
// Получить "своих" Валер
export const getMyValeras = async () => {
  try {
    const response = await api.get('/my');
    return response.data;
 } catch (error) {
    console.error('Ошибка при получении списка моих Валер:', error);
    throw error;
  }
};
```

### 2. Создание компонента Login
**Файл:** `valera-frontend/src/components/Login.jsx`
**Строки:** 1-56

Создан компонент для входа пользователя с полями email и пароль, а также обработкой ошибок. Добавлена ссылка на страницу регистрации и перенаправление на главную страницу после успешного входа.

**Файл:** `valera-frontend/src/components/Login.css`
**Строки:** 1-54

Созданы стили для компонента входа, включая стили для ссылки на регистрацию.

### 3. Создание компонента Register
**Файл:** `valera-frontend/src/components/Register.jsx`
**Строки:** 1-60

Создан компонент для регистрации пользователя с полями email, пароль и имя пользователя, а также обработкой ошибок. Добавлена ссылка на страницу входа и перенаправление на страницу входа после успешной регистрации.

**Файл:** `valera-frontend/src/components/Register.css`
**Строки:** 1-54

Созданы стили для компонента регистрации, включая стили для ссылки на вход.

### 4. Создание компонентов маршрутов
**Файл:** `valera-frontend/src/components/ProtectedRoute.jsx`
**Строки:** 1-15

Создан компонент для защищенных маршрутов, который проверяет наличие токена и перенаправляет на страницу входа при его отсутствии.

**Файл:** `valera-frontend/src/components/PublicRoute.jsx`
**Строки:** 1-15

Создан компонент для публичных маршрутов, который перенаправляет авторизованных пользователей на главную страницу.

### 5. Обновление главного компонента
**Файл:** `valera-frontend/src/App.jsx`
**Строки:** 1-58

Обновлен главный компонент приложения с добавлением маршрутов для авторизации и защиты маршрутов с использованием новых компонентов ProtectedRoute и PublicRoute.

### 6. Обновление компонента ValeraList
**Файл:** `valera-frontend/src/components/ValeraList/ValeraList.jsx`
**Строки:** 1-158

Обновлен компонент списка Валер с добавлением:
- Кнопки выхода из системы
- Проверки роли пользователя для отображения Валер (администратор видит всех, обычный пользователь - только своих)
- Использования метода getMyValeras для получения "своих" Валер

**Файл:** `valera-frontend/src/components/ValeraList.css`
**Строки:** 20-32

Добавлены стили для заголовка с кнопкой выхода.

### 7. Обновление компонента ValeraStats
**Файл:** `valera-frontend/src/components/ValeraStats/ValeraStats.jsx`
**Строки:** 1-158

Обновлен компонент статистики Валеры с добавлением:
- Кнопки выхода из системы
- Проверки роли пользователя
- Передачи функции onLogout из родительского компонента

**Файл:** `valera-frontend/src/components/ValeraStats.css`
**Строки:** 10-22

Добавлены стили для заголовка с кнопкой выхода.
