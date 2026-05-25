using Microsoft.AspNetCore.Mvc;
using MEDPASS.Models;
using MEDPASS.Services;

namespace MEDPASS.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class ConsultasController : ControllerBase
    {
        private readonly IConsultaService _servicioConsulta;

        public ConsultasController(IConsultaService servicioConsulta)
        {
            _servicioConsulta = servicioConsulta;
        }

        [HttpGet]
        public async Task<IActionResult> GetConsultas()
        {
            var consultas = await _servicioConsulta.DameTodasLasConsultas();
            return Ok(consultas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConsulta(int id)
        {
            var consulta = await _servicioConsulta.DameConsultaPorId(id);
            if (consulta == null)
                return NotFound("No se encontró la consulta con el ID proporcionado.");
            return Ok(consulta);
        }

        [HttpPost]
        public async Task<IActionResult> CrearConsulta([FromBody] Consulta nuevaConsulta)
        {
            if (nuevaConsulta == null || string.IsNullOrWhiteSpace(nuevaConsulta.Motivo))
                return BadRequest("Datos inválidos");
            var creada = await _servicioConsulta.CrearConsulta(nuevaConsulta);
            return CreatedAtAction(nameof(GetConsulta), new { id = creada.Id }, creada);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarConsulta(int id, [FromBody] Consulta consultaActualizar)
        {
            var actualizado = await _servicioConsulta.ActualizarConsulta(id, consultaActualizar);
            if (actualizado == null)
                return NotFound("No se encontró la consulta para actualizar.");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarConsulta(int id)
        {
            var eliminado = await _servicioConsulta.EliminarConsulta(id);
            if (eliminado == null)
                return NotFound("No se encontró la consulta con el ID proporcionado.");
            return NoContent();
        }
    }
}