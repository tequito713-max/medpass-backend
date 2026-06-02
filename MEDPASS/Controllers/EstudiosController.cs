using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MEDPASS.Models;
using MEDPASS.Services;

namespace MEDPASS.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Medico,Paciente")]
public class EstudiosController : ControllerBase
    {
        private readonly IEstudioService _servicioEstudio;

        public EstudiosController(IEstudioService servicioEstudio)
        {
            _servicioEstudio = servicioEstudio;
        }

        [HttpGet]
        public async Task<IActionResult> GetEstudios()
        {
            var estudios = await _servicioEstudio.DameTodosLosEstudios();
            return Ok(estudios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEstudio(int id)
        {
            var estudio = await _servicioEstudio.DameEstudioPorId(id);
            if (estudio == null)
                return NotFound("No se encontró el estudio con el ID proporcionado.");
            return Ok(estudio);
        }

        [Authorize(Roles = "Medico")]
        [HttpPost]
        public async Task<IActionResult> CrearEstudio([FromBody] Estudio nuevoEstudio)
        {
            if (nuevoEstudio == null || string.IsNullOrWhiteSpace(nuevoEstudio.Tipo))
                return BadRequest("Datos inválidos");
            var creado = await _servicioEstudio.CrearEstudio(nuevoEstudio);
            return CreatedAtAction(nameof(GetEstudio), new { id = creado.Id }, creado);
        }

        [Authorize(Roles = "Medico")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarEstudio(int id, [FromBody] Estudio estudioActualizar)
        {
            var actualizado = await _servicioEstudio.ActualizarEstudio(id, estudioActualizar);
            if (actualizado == null)
                return NotFound("No se encontró el estudio para actualizar.");
            return NoContent();
        }

        [Authorize(Roles = "Medico")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarEstudio(int id)
        {
            var eliminado = await _servicioEstudio.EliminarEstudio(id);
            if (eliminado == null)
                return NotFound("No se encontró el estudio con el ID proporcionado.");
            return NoContent();
        }
    }
}
