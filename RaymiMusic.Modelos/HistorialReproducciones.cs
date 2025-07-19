using System;
using System.ComponentModel.DataAnnotations;

namespace RaymiMusic.Modelos
{
    public class HistorialReproducciones
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CancionId { get; set; }
        public Cancion Cancion { get; set; }

        [Required]
        public DateTime FechaReproduccion { get; set; } 
    }
}
