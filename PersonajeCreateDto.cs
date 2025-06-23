using System.ComponentModel.DataAnnotations;

namespace PersonajeWebAPI.DTOs
{
    public class PersonajeCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "La clase no puede exceder 50 caracteres")]
        public string? Clase { get; set; }

        [Range(1, 100, ErrorMessage = "El nivel debe estar entre 1 y 100")]
        public int Nivel { get; set; }

        [Range(1, 1000, ErrorMessage = "La vida debe estar entre 1 y 1000")]
        public int Vida { get; set; }
    }
}