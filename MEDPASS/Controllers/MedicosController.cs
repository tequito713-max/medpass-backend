using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MEDPASS.Models;
using MEDPASS.Services;

namespace MEDPASS.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Medico,Paciente")]
public class MedicosController : ControllerBase
    {
        private readonly IMedicoService _servicioMedico;

        public MedicosController(IMedicoService servicioMedico)
        {
            _servicioMedico = servicioMedico;
        }

        [HttpGet]
        public async Task<IActionResult> GetMedicos()
        {
            var medicos = await _servicioMedico.DameTodosLosMedicos();
            return Ok(medicos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedico(int id)
        {
            var medico = await _servicioMedico.DameMedicoPorId(id);
            if (medico == null)
                return NotFound("No se encontró el médico con el ID proporcionado.");
            return Ok(medico);
        }

        [Authorize(Roles = "Medico")]
        [HttpPost]
        public async Task<IActionResult> CrearMedico([FromBody] Medico nuevoMedico)
        {
            if (nuevoMedico == null || string.IsNullOrWhiteSpace(nuevoMedico.Nombre))
                return BadRequest("Datos inválidos");
            var creado = await _servicioMedico.CrearMedico(nuevoMedico);
            return CreatedAtAction(nameof(GetMedico), new { id = creado.Id }, creado);
        }

        [Authorize(Roles = "Medico")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarMedico(int id, [FromBody] Medico medicoActualizar)
        {
            var actualizado = await _servicioMedico.ActualizarMedico(id, medicoActualizar);
            if (actualizado == null)
                return NotFound("No se encontró el médico para actualizar.");
            return NoContent();
        }

        [Authorize(Roles = "Medico")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarMedico(int id)
        {
            var eliminado = await _servicioMedico.EliminarMedico(id);
            if (eliminado == null)
                return NotFound("No se encontró el médico con el ID proporcionado.");
            return NoContent();
        }
    }
}
