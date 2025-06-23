using System.Data.SqlClient;
using PersonajeWebAPI.Models;

namespace PersonajeWebAPI.DataAccess
{
    public class PersonajeADORepository
    {
        private readonly string _connectionString;
        private readonly ILogger<PersonajeADORepository> _logger;

        public PersonajeADORepository(IConfiguration configuration, ILogger<PersonajeADORepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        public async Task InicializarTablaAsync()
        {
            string sql = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PersonajesADO' AND xtype='U')
                CREATE TABLE PersonajesADO (
                    Id int IDENTITY(1,1) PRIMARY KEY,
                    Nombre nvarchar(100) NOT NULL,
                    Clase nvarchar(50) NULL,
                    Nivel int NOT NULL,
                    Vida int NOT NULL,
                    FechaCreacion datetime NOT NULL
                )";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("Tabla PersonajesADO verificada/creada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear tabla PersonajesADO");
                throw;
            }
        }

        public async Task<int> InsertarPersonajeAsync(Personaje personaje)
        {
            string sql = @"
                INSERT INTO PersonajesADO (Nombre, Clase, Nivel, Vida, FechaCreacion) 
                VALUES (@nombre, @clase, @nivel, @vida, @fechaCreacion);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@nombre", personaje.Nombre);
                command.Parameters.AddWithValue("@clase", personaje.Clase ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@nivel", personaje.Nivel);
                command.Parameters.AddWithValue("@vida", personaje.Vida);
                command.Parameters.AddWithValue("@fechaCreacion", personaje.FechaCreacion);

                var result = await command.ExecuteScalarAsync();
                int nuevoId = Convert.ToInt32(result);
                _logger.LogInformation("Personaje insertado con ID: {Id}", nuevoId);
                return nuevoId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar personaje");
                throw;
            }
        }

        public async Task<List<Personaje>> ObtenerTodosLosPersonajesAsync()
        {
            var personajes = new List<Personaje>();
            string sql = "SELECT Id, Nombre, Clase, Nivel, Vida, FechaCreacion FROM PersonajesADO ORDER BY Id";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var personaje = new Personaje
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader.GetString("Nombre"),
                        Clase = reader.IsDBNull("Clase") ? null : reader.GetString("Clase"),
                        Nivel = reader.GetInt32("Nivel"),
                        Vida = reader.GetInt32("Vida"),
                        FechaCreacion = reader.GetDateTime("FechaCreacion")
                    };
                    personajes.Add(personaje);
                }

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
            string sql = "SELECT Id, Nombre, Clase, Nivel, Vida, FechaCreacion FROM PersonajesADO WHERE Id = @id";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Personaje
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader.GetString("Nombre"),
                        Clase = reader.IsDBNull("Clase") ? null : reader.GetString("Clase"),
                        Nivel = reader.GetInt32("Nivel"),
                        Vida = reader.GetInt32("Vida"),
                        FechaCreacion = reader.GetDateTime("FechaCreacion")
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar personaje con ID: {Id}", id);
                throw;
            }
        }
    }
}