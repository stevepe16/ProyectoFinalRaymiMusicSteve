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

        [BindProperty]
        public Artista? Artista { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioId))
                return RedirectToPage("/Cuenta/Login");

            var id = Guid.Parse(usuarioId);  // Convertimos el id a Guid
            // Buscamos el artista por el ID del usuario
            Artista = await _context.Artistas
                .Include(a => a.Canciones)
                .Include(a => a.Albumes)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (Artista == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile? fotoPerfil)
        {
            if (!ModelState.IsValid) return Page();

            // Si se sube una nueva foto
            if (fotoPerfil != null)
            {
                var filePath = Path.Combine("wwwroot", "media", "perfil", fotoPerfil.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fotoPerfil.CopyToAsync(stream);
                }

                Artista.UrlFotoPerfil = "/media/perfil/" + fotoPerfil.FileName;
            }

            // Guardamos los cambios
            _context.Artistas.Update(Artista);
            await _context.SaveChangesAsync();

            return RedirectToPage("Perfil");
        }
    }
}
