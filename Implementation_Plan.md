# План реализации аутентификации и авторизации

## Текущая структура проекта

### Backend (.NET 6)
- `valera3.csproj` - файл проекта
- `Program.cs` - основной файл приложения
- `Models/valera.cs` - модель Valera
- `Controllers/ValeraController.cs` - контроллер Valera
- `Data/ValeraDbContext.cs` - контекст базы данных
- `Services/ValeraService.cs` - сервис Valera
- `appsettings.json` - конфигурационный файл

### Frontend (React)
- `src/services/api.js` - API сервис

## Требования к реализации

1. Реализовать регистрацию пользователя (POST /api/auth/register)
2. Реализовать вход пользователя (POST /api/auth/login)
3. Подключить JWT-авторизацию
4. Реализовать роли (User и Admin)
5. Ограничить доступ к API Валеры
6. Реализовать проверки на фронтенде

## Подробный план реализации

### Шаг 1: Создание модели User

Файл: `valera3/Models/User.cs`
Строки: 1-30

Создаем модель пользователя с полями:
- Id (int)
- Email (string)
- PasswordHash (string)
- Username (string)
- Role (string) - "User" или "Admin"

### Шаг 2: Добавление JWT аутентификации

Файл: `valera3/valera3.csproj`
Строки: 8-15

Добавляем необходимые пакеты:
- Microsoft.AspNetCore.Authentication.JwtBearer
- System.IdentityModel.Tokens.Jwt
- Microsoft.AspNetCore.Authorization

### Шаг 3: Настройка конфигурации JWT

Файл: `valera3/appsettings.json`
Строки: 6-10

Добавляем секцию Jwt с настройками:
- Key
- Issuer
- Audience
- Expire

### Шаг 4: Создание AuthController

Файл: `valera3/Controllers/AuthController.cs`
Строки: 1-100

Создаем контроллер с методами:
- Register (POST /api/auth/register)
- Login (POST /api/auth/login)

### Шаг 5: Обновление модели Valera

Файл: `valera3/Models/valera.cs`
Строки: 1-150

Добавляем поле UserId и связь с пользователем

### Шаг 6: Обновление ValeraDbContext

Файл: `valera3/Data/ValeraDbContext.cs`
Строки: 1-30

Добавляем DbSet<User> и связи между Valera и User

### Шаг 7: Обновление ValeraService

Файл: `valera3/Services/ValeraService.cs`
Строки: 1-200

Добавляем методы с проверками авторизации

### Шаг 8: Обновление ValeraController

Файл: `valera3/Controllers/ValeraController.cs`
Строки: 1-120

Добавляем атрибуты авторизации и новые методы для доступа к "своим" Валерам

### Шаг 9: Обновление frontend

Файл: `valera-frontend/src/services/api.js`
Строки: 1-80

Добавляем обработку токенов и аутентификацию

## Технические детали

### Модель User
```csharp
public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Username { get; set; }
    public string Role { get; set; } = "User"; // По умолчанию "User"
    public List<Valera> Valeras { get; set; }
}
```

### Новые эндпоинты
- POST /api/auth/register - регистрация пользователя
- POST /api/auth/login - вход пользователя
- GET /api/valera/my - получить "своих" Валер (требует авторизацию)
- GET /api/valera (остается как есть, но требует авторизацию админа)
- POST /api/valera - создать Валеру (требует авторизацию)
- PUT /api/valera/{id} - обновить Валеру (только для владельца)
- DELETE /api/valera/{id} - удалить Валеру (только для владельца)

### Атрибуты авторизации
- [Authorize] - для всех методов, кроме регистрации и входа
- [Authorize(Roles = "Admin")] - для администраторских методов
