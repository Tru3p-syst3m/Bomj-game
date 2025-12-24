using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using valera3.Models;
using valera3.Services;

namespace valera3.Controllers
{
    [ApiController]
    [Route("api")]
    public class ValeraController : ControllerBase
    {
        private readonly ValeraService _valeraService;

        public ValeraController(ValeraService valeraService)
        {
            _valeraService = valeraService;
        }

        // Получить Валер (всех для администраторов, своих для обычных пользователей)
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Valera>>> GetAll()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            if (userRole == "Admin")
            {
                // Администратор видит всех Валер
                var valeras = await _valeraService.GetAllValerasAsync();
                return Ok(valeras);
            }
            else
            {
                // Обычный пользователь видит только своих Валер
                var valeras = await _valeraService.GetUserValerasAsync(userId);
                return Ok(valeras);
            }
        }

        // Получить "своих" Валер (доступно авторизованным пользователям)
        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<List<Valera>>> GetMyValeras()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var valeras = await _valeraService.GetUserValerasAsync(userId);
            return Ok(valeras);
        }

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

        [HttpPost]
        //[Authorize]
        public async Task<ActionResult<Valera>> Create(Valera valera)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var created = await _valeraService.CreateValeraAsync(valera, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

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
    }
}
