using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Api.Data;
using RaymiMusic.Modelos;
using Microsoft.AspNetCore.Http;

namespace RaymiMusic.MVC.Pages.VistaArtista
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _dbContext; 

        public IndexModel(AppDbContext dbContext) 
        {
            _dbContext = dbContext; 
        }

        public ICollection<Cancion> Canciones { get; set; }  // Aquí se almacenan las canciones del artista

        public async Task<IActionResult> OnGetAsync()
        {
            var correo = HttpContext.Session.GetString("Correo");
            if (string.IsNullOrEmpty(correo))
                return RedirectToPage("/Cuenta/Login");

            // Obtener las canciones del artista por su correo
            var artista = await _dbContext.Artistas
                .Include(a => a.Canciones)  // Incluimos las canciones del artista
                .FirstOrDefaultAsync(a => a.Correo == correo);

            if (artista == null)
                return NotFound();

            // Asignar las canciones del artista a la propiedad Canciones
            Canciones = artista.Canciones.ToList();

            return Page();
        }
    }
}
