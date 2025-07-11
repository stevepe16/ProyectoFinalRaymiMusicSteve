using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RaymiMusic.Api.Data;
using RaymiMusic.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace RaymiMusic.MVC.Pages.VistaArtista
{
    public class PerfilModel : PageModel
    {
        private readonly AppDbContext _context;

        public PerfilModel(AppDbContext context)
        {
            _context = context;
        }

        public Artista? Artista { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Obtener el correo desde la sesión
            var correo = HttpContext.Session.GetString("Correo");
            if (string.IsNullOrEmpty(correo))
                return RedirectToPage("/Cuenta/Login");

            // Buscar al artista por el correo del usuario
            Artista = await _context.Artistas
                .Include(a => a.Canciones)
                .Include(a => a.Albumes)
                .FirstOrDefaultAsync(a => a.Correo == correo);

            if (Artista == null)
                return NotFound();

            return Page();
        }
    }
}
