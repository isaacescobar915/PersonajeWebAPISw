using Microsoft.AspNetCore.Mvc;
using PersonajeWebAPI.DataAccess;
using PersonajeWebAPI.DTOs;
using PersonajeWebAPI.Models;

namespace PersonajeWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Personajes Entity Framework")]
    public class PersonajesEFController : ControllerBase
    {
        private readonly PersonajeEFRepository _repository;
        private readonly ILogger<PersonajesEFController> _logger;

        public PersonajesEFController(PersonajeEFRepository repository, ILogger<PersonajesEFController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los personajes usando Entity Framework
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Personaje>>> ObtenerTodos()
        {
            try
            {
                var personajes = await _repository.ObtenerTodosLosPersonajesAsync();
                return Ok(new
                {
                    personajes,
                    total = personajes.Count,
                    tecnologia = "Entity Framework",
                    tabla = "PersonajesEF"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener personajes");
                return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un personaje por ID usando Entity Framework
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Personaje>> ObtenerPorId(int id)
        {
            try
            {
                var personaje = await _repository.ObtenerPersonajePorIdAsync(id);
                if (personaje == null)
                {
                    return NotFound(new { mensaje = $"Personaje con ID {id} no encontrado", tecnologia = "Entity Framework" });
                }

                return Ok(new { personaje, tecnologia = "Entity Framework" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener personaje");
                return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo personaje usando Entity Framework
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Personaje>> CrearPersonaje(PersonajeCreateDto personajeDto)
        {
            try
            {
                var personaje = new Personaje
                {
                    Nombre = personajeDto.Nombre,
                    Clase = personajeDto.Clase,
                    Nivel = personajeDto.Nivel,
                    Vida = personajeDto.Vida,
                    FechaCreacion = DateTime.Now
                };

                var nuevoId = await _repository.InsertarPersonajeAsync(personaje);
                personaje.Id = nuevoId;

                return CreatedAtAction(
                    nameof(ObtenerPorId),
                    new { id = nuevoId },
                    new
                    {
                        personaje,
                        mensaje = "Personaje creado exitosamente",
                        tecnologia = "Entity Framework"
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear personaje");
                return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene personajes por clase usando Entity Framework (funcionalidad adicional)
        /// </summary>
        [HttpGet("clase/{clase}")]
        public async Task<ActionResult<List<Personaje>>> ObtenerPorClase(string clase)
        {
            try
            {
                var personajes = await _repository.ObtenerPersonajesPorClaseAsync(clase);
                return Ok(new
                {
                    personajes,
                    total = personajes.Count,
                    clase,
                    tecnologia = "Entity Framework"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener personajes por clase");
                return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
            }
        }
    }

    internal class PersonajeEFRepository
    {
    }
}