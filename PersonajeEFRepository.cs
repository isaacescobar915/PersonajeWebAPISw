using Microsoft.EntityFrameworkCore;
using PersonajeWebAPI.Models;

namespace PersonajeWebAPI.DataAccess
{
    public class PersonajeEFRepository
    {
        private readonly PersonajeContext _context;
        private readonly ILogger<PersonajeEFRepository> _logger;

        public PersonajeEFRepository(PersonajeContext context, ILogger<PersonajeEFRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> InsertarPersonajeAsync(Personaje personaje)
        {
            try
            {
                _context.Personajes.Add(personaje);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Personaje insertado con ID: {Id}", personaje.Id);
                return personaje.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar personaje");
                throw;
            }
        }

        public async Task<List<Personaje>> ObtenerTodosLosPersonajesAsync()
        {
            try
            {
                var personajes = await _context.Personajes
                    .OrderBy(p => p.Id)
                    .ToListAsync();
                _logger.LogInformation("Se obtuvieron {Count} personajes", personajes.Count);
                return personajes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener personajes");
                throw;
            }
        }

        public async Task<Personaje?> ObtenerPersonajePorIdAsync(int id)
        {
            try
            {
                return await _context.Personajes.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar personaje con ID: {Id}", id);
                throw;
            }
        }

        public async Task<List<Personaje>> ObtenerPersonajesPorClaseAsync(string clase)
        {
            try
            {
                return await _context.Personajes
                    .Where(p => p.Clase == clase)
                    .OrderBy(p => p.Nombre)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar por clase: {Clase}", clase);
                throw;
            }
        }
    }
}