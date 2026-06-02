using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MEDPASS.Models;
using MEDPASS.Services;

namespace MEDPASS.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Medico,Paciente")]
public class ClinicasController : ControllerBase
    {
        private readonly IClinicaService _servicioClinica;

        public ClinicasController(IClinicaService servicioClinica)
        {
            _servicioClinica = servicioClinica;
        }

        [HttpGet]
        public async Task<IActionResult> GetClinicas()
        {
            var clinicas = await _servicioClinica.DameTodasLasClinicas();
            return Ok(clinicas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClinica(int id)
        {
            var clinica = await _servicioClinica.DameClinicaPorId(id);
            if (clinica == null)
                return NotFound("No se encontró la clínica con el ID proporcionado.");
            return Ok(clinica);
        }

        [Authorize(Roles = "Medico")]
        [HttpPost]
        public async Task<IActionResult> CrearClinica([FromBody] Clinica nuevaClinica)
        {
            if (nuevaClinica == null || string.IsNullOrWhiteSpace(nuevaClinica.Nombre))
                return BadRequest("Datos inválidos");
            var creada = await _servicioClinica.CrearClinica(nuevaClinica);
            return CreatedAtAction(nameof(GetClinica), new { id = creada.Id }, creada);
        }

        [Authorize(Roles = "Medico")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarClinica(int id, [FromBody] Clinica clinicaActualizar)
        {
            var actualizado = await _servicioClinica.ActualizarClinica(id, clinicaActualizar);
            if (actualizado == null)
                return NotFound("No se encontró la clínica para actualizar.");
            return NoContent();
        }

        [Authorize(Roles = "Medico")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarClinica(int id)
        {
            var eliminado = await _servicioClinica.EliminarClinica(id);
            if (eliminado == null)
                return NotFound("No se encontró la clínica con el ID proporcionado.");
            return NoContent();
        }
    }
}
