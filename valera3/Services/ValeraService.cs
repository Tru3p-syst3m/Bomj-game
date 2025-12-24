using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using valera3.Models;

namespace valera3.Services
{
    public class ValeraService
    {
        private readonly ValeraDbContext _context;

        public ValeraService(ValeraDbContext context)
        {
            _context = context;
        }

        public async Task<List<Valera>> GetAllValerasAsync()
        {
            return await _context.Valeras.ToListAsync();
        }

        // Новый метод для получения Валер конкретного пользователя
        public async Task<List<Valera>> GetUserValerasAsync(int userId)
        {
            return await _context.Valeras.Where(v => v.UserId == userId).ToListAsync();
        }

        public async Task<Valera> GetValeraByIdAsync(int id)
        {
            return await _context.Valeras.FindAsync(id) ?? throw new KeyNotFoundException();
        }

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

        public async Task<Valera> CreateValeraAsync(Valera valera, int userId)
        {
            valera.UserId = userId; // Устанавливаем владельца
            _context.Valeras.Add(valera);
            await _context.SaveChangesAsync();
            return valera;
        }

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
    }
}
