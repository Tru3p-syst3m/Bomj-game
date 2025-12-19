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

        public async Task<Valera> GetValeraByIdAsync(int id)
        {
            return await _context.Valeras.FindAsync(id) ?? throw new KeyNotFoundException();
        }

        public async Task<Valera> CreateValeraAsync(Valera valera)
        {
            _context.Valeras.Add(valera);
            await _context.SaveChangesAsync();
            return valera;
        }

        public async Task<Valera?> UpdateValeraAsync(int id, Valera valera)
        {
            var existingValera = await _context.Valeras.FindAsync(id);
            if (existingValera == null)
                return null;
            existingValera.Health = valera.Health;
            existingValera.Mana = valera.Mana;
            existingValera.Cheerfulness = valera.Cheerfulness;
            existingValera.Fatigue = valera.Fatigue;
            existingValera.Money = valera.Money;

            await _context.SaveChangesAsync();
            return existingValera;
        }

        public async Task<bool> DeleteValeraAsync(int id)
        {
            var valera = await _context.Valeras.FindAsync(id);
            if (valera == null)
                return false;

            _context.Valeras.Remove(valera);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExecuteActionAsync(int id, string actionName)
        {
            var valera = await _context.Valeras.FindAsync(id);
            if (valera == null)
                return false;

            bool result = false;
            if (valera.IsAlive){
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

        public async Task<Valera?> ResetValeraAsync(int id)
        {
            var valera = await _context.Valeras.FindAsync(id);
            if (valera == null)
                return null;
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