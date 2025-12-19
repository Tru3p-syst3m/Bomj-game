using Microsoft.AspNetCore.Mvc;
using valera3.Models;
using valera3.Services;

namespace valera3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValeraController : ControllerBase
    {
        private readonly ValeraService _valeraService;

        public ValeraController(ValeraService valeraService)
        {
            _valeraService = valeraService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Valera>>> GetAll()
        {
            var valeras = await _valeraService.GetAllValerasAsync();
            return Ok(valeras);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Valera>> GetById(int id)
        {
            var valera = await _valeraService.GetValeraByIdAsync(id);
            if (valera == null)
                return NotFound();
            return Ok(valera);
        }

        [HttpPost]
        public async Task<ActionResult<Valera>> Create(Valera valera)
        {
            var created = await _valeraService.CreateValeraAsync(valera);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Valera>> Update(int id, Valera valera)
        {
            var updated = await _valeraService.UpdateValeraAsync(id, valera);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _valeraService.DeleteValeraAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/actions/{actionName}")]
        public async Task<ActionResult> ExecuteAction(int id, string actionName)
        {
            var result = await _valeraService.ExecuteActionAsync(id, actionName);
            if (!result)
                return BadRequest("Action failed or Valera not found");
            return Ok("Action executed successfully");
        }

        [HttpPost("{id}/reset")]
        public async Task<ActionResult<Valera>> Reset(int id)
        {
            var valera = await _valeraService.ResetValeraAsync(id);
            if (valera == null)
                return NotFound();
            return Ok(valera);
        }
    }
}