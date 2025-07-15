using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NAudio.Wave;
using RaymiMusic.Api.Data;
using RaymiMusic.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RaymiMusic.MVC.Pages.VistaArtista
{
    public class SubirCancionModel : PageModel
    {
        private readonly AppDbContext _context;

        public SubirCancionModel(AppDbContext context)
        {
            _context = context;
            Errores = new List<string>(); // Inicializa la lista de errores
        }

        [BindProperty]
        public Cancion Cancion { get; set; }

        [BindProperty]
        public IFormFile Archivo { get; set; }

        public SelectList Generos { get; set; }

        public List<string> Errores { get; set; }

        // Método para cargar los géneros en el OnGet
        public async Task OnGetAsync()
        {
            var generos = await _context.Generos.ToListAsync();
            if (generos == null || !generos.Any())
            {
                Errores.Add("No hay géneros disponibles.");
            }
            else
            {
                Generos = new SelectList(generos, "Id", "Nombre");
            }
        }

        // Método para procesar el formulario y subir la canción
        public async Task<IActionResult> OnPostAsync()
        {
            Errores = new List<string>();

            if (Archivo == null || Archivo.Length == 0)
            {
                Errores.Add("Debes seleccionar un archivo.");
                return Page();
            }

            if (Cancion.GeneroId == Guid.Empty)
            {
                Errores.Add("Debes seleccionar un género.");
                return Page();
            }

            if (string.IsNullOrEmpty(Cancion.Titulo))
            {
                Errores.Add("El título de la canción es obligatorio.");
                return Page();
            }

            // Verificar que el archivo sea de tipo audio
            if (!Archivo.ContentType.StartsWith("audio"))
            {
                Errores.Add("El archivo debe ser un audio.");
                return Page();
            }

            // Guarda el archivo en wwwroot/media
            var rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "media", Archivo.FileName);
            try
            {
                using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                {
                    await Archivo.CopyToAsync(stream);
                }

                // Extraer la duración del archivo MP3 usando NAudio
                TimeSpan duracion = GetAudioDuration(rutaArchivo);
                Cancion.Duracion = duracion;
            }
            catch (Exception ex)
            {
                Errores.Add($"Error al guardar el archivo: {ex.Message}");
                return Page();
            }

            // Guardar la canción en la base de datos
            Cancion.RutaArchivo = Archivo.FileName;
            Cancion.Id = Guid.NewGuid();
            _context.Canciones.Add(Cancion);
            await _context.SaveChangesAsync();

            return RedirectToPage("/VistaArtista/Index"); 
        }

        private TimeSpan GetAudioDuration(string filePath)
        {
            using (var reader = new AudioFileReader(filePath))
            {
                return reader.TotalTime;
            }
        }
    }
}
