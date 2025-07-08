using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RaymiMusic.Api.Data;
using RaymiMusic.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace RaymiMusic.MVC.Pages.VistaArtista
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public Artista? Artista { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Obtener el UsuarioId desde la sesión
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
                return RedirectToPage("/Cuenta/Login");

            var id = Guid.Parse(usuarioId);  // Convertir el id a Guid

            // Buscar al artista por el Id del usuario
            Artista = await _context.Artistas
                .Include(a => a.Canciones)
                .Include(a => a.Albumes)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (Artista == null)
                return NotFound();

            return Page();
        }
    }
}
