using Microsoft.AspNetCore.Mvc;
using MEDPASS.Models;
using MEDPASS.Services;

namespace MEDPASS.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class PacientesController : ControllerBase
    {
        private readonly IPacienteService _servicioPaciente;

        public PacientesController(IPacienteService servicioPaciente)
        {
            _servicioPaciente = servicioPaciente;
        }

        [HttpGet]
        public async Task<IActionResult> GetPacientes()
        {
            var pacientes = await _servicioPaciente.DameTodosLosPacientes();
            return Ok(pacientes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaciente(int id)
        {
            var paciente = await _servicioPaciente.DamePacientePorId(id);
            if (paciente == null)
                return NotFound("No se encontró el paciente con el ID proporcionado.");
            return Ok(paciente);
        }

        [HttpPost]
        public async Task<IActionResult> CrearPaciente([FromBody] Paciente nuevoPaciente)
        {
            if (nuevoPaciente == null || string.IsNullOrWhiteSpace(nuevoPaciente.Nombre))
                return BadRequest("Datos inválidos");
            var creado = await _servicioPaciente.CrearPaciente(nuevoPaciente);
            return CreatedAtAction(nameof(GetPaciente), new { id = creado.Id }, creado);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarPaciente(int id, [FromBody] Paciente pacienteActualizar)
        {
            var actualizado = await _servicioPaciente.ActualizarPaciente(id, pacienteActualizar);
            if (actualizado == null)
                return NotFound("No se encontró el paciente para actualizar.");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPaciente(int id)
        {
            var eliminado = await _servicioPaciente.EliminarPaciente(id);
            if (eliminado == null)
                return NotFound("No se encontró el paciente con el ID proporcionado.");
            return NoContent();
        }
    }
}