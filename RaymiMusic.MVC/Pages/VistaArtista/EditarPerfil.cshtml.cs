using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RaymiMusic.Api.Data;
using RaymiMusic.Modelos;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace RaymiMusic.MVC.Pages.Cliente
{
    public class EditarPerfilModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditarPerfilModel(AppDbContext context)
        {
            _context = context;
        }

        public Artista Artista { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Obtener el correo desde la sesión
            var correo = HttpContext.Session.GetString("Correo");
            if (string.IsNullOrEmpty(correo))
                return RedirectToPage("/Cuenta/Login");

            // Buscar al artista por el correo
            Artista = await _context.Artistas
                .FirstOrDefaultAsync(a => a.Correo == correo);

            if (Artista == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var correo = HttpContext.Session.GetString("Correo");
            if (string.IsNullOrEmpty(correo))
                return RedirectToPage("/Cuenta/Login");

            var artista = await _context.Artistas
                .FirstOrDefaultAsync(a => a.Correo == correo);

            if (artista == null)
                return NotFound();

            artista.NombreArtistico = Request.Form["NombreArtistico"];
            artista.Biografia = Request.Form["Biografia"];

            var archivoFoto = Request.Form.Files.FirstOrDefault();
            if (archivoFoto != null)
            {

                var fotoPath = "/media/perfil/" + archivoFoto.FileName; 
                artista.UrlFotoPerfil = fotoPath;
            }

            _context.Artistas.Update(artista);
            await _context.SaveChangesAsync();

            return RedirectToPage("/VistaArtista/Perfil"); 
        }
    }
}
