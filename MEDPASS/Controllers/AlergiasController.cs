using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MEDPASS.Models;
using MEDPASS.Services;

namespace MEDPASS.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Medico,Paciente")]
public class AlergiasController : ControllerBase
    {
        private readonly IAlergiaService _servicioAlergia;

        public AlergiasController(IAlergiaService servicioAlergia)
        {
            _servicioAlergia = servicioAlergia;
        }

        [HttpGet]
        public async Task<IActionResult> GetAlergias()
        {
            var alergias = await _servicioAlergia.DameTodasLasAlergias();
            return Ok(alergias);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlergia(int id)
        {
            var alergia = await _servicioAlergia.DameAlergiaPorId(id);
            if (alergia == null)
                return NotFound("No se encontró la alergia con el ID proporcionado.");
            return Ok(alergia);
        }

        [Authorize(Roles = "Medico")]
        [HttpPost]
        public async Task<IActionResult> CrearAlergia([FromBody] Alergia nuevaAlergia)
        {
            if (nuevaAlergia == null || string.IsNullOrWhiteSpace(nuevaAlergia.Sustancia))
                return BadRequest("Datos inválidos");
            var creada = await _servicioAlergia.CrearAlergia(nuevaAlergia);
            return CreatedAtAction(nameof(GetAlergia), new { id = creada.Id }, creada);
        }

        [Authorize(Roles = "Medico")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarAlergia(int id, [FromBody] Alergia alergiaActualizar)
        {
            var actualizado = await _servicioAlergia.ActualizarAlergia(id, alergiaActualizar);
            if (actualizado == null)
                return NotFound("No se encontró la alergia para actualizar.");
            return NoContent();
        }

        [Authorize(Roles = "Medico")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarAlergia(int id)
        {
            var eliminado = await _servicioAlergia.EliminarAlergia(id);
            if (eliminado == null)
                return NotFound("No se encontró la alergia con el ID proporcionado.");
            return NoContent();
        }
    }
}
