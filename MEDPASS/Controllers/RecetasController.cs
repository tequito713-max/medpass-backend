using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MEDPASS.Models;
using MEDPASS.Services;

namespace MEDPASS.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Medico,Paciente")]
public class RecetasController : ControllerBase
    {
        private readonly IRecetaService _servicioReceta;

        public RecetasController(IRecetaService servicioReceta)
        {
            _servicioReceta = servicioReceta;
        }

        [HttpGet]
        public async Task<IActionResult> GetRecetas()
        {
            var recetas = await _servicioReceta.DameTodasLasRecetas();
            return Ok(recetas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReceta(int id)
        {
            var receta = await _servicioReceta.DameRecetaPorId(id);
            if (receta == null)
                return NotFound("No se encontró la receta con el ID proporcionado.");
            return Ok(receta);
        }

        [Authorize(Roles = "Medico")]
        [HttpPost]
        public async Task<IActionResult> CrearReceta([FromBody] Receta nuevaReceta)
        {
            if (nuevaReceta == null || string.IsNullOrWhiteSpace(nuevaReceta.Medicamento))
                return BadRequest("Datos inválidos");
            var creada = await _servicioReceta.CrearReceta(nuevaReceta);
            return CreatedAtAction(nameof(GetReceta), new { id = creada.Id }, creada);
        }

        [Authorize(Roles = "Medico")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarReceta(int id, [FromBody] Receta recetaActualizar)
        {
            var actualizado = await _servicioReceta.ActualizarReceta(id, recetaActualizar);
            if (actualizado == null)
                return NotFound("No se encontró la receta para actualizar.");
            return NoContent();
        }

        [Authorize(Roles = "Medico")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarReceta(int id)
        {
            var eliminado = await _servicioReceta.EliminarReceta(id);
            if (eliminado == null)
                return NotFound("No se encontró la receta con el ID proporcionado.");
            return NoContent();
        }
    }
}
