using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Api.Data;
using RaymiMusic.Data;
using RaymiMusic.Modelos;
using System;

namespace RaymiMusic.MVC.Pages.Cuenta
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Correo { get; set; } = null!;

        [BindProperty]
        public string Contrasena { get; set; } = null!;

        public string? ErrorMensaje { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == Correo);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(Contrasena, usuario.HashContrasena))
            {
                ErrorMensaje = "Correo o contraseña incorrectos.";
                return Page();
            }

            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("Correo", usuario.Correo);
            HttpContext.Session.SetString("Rol", usuario.Rol);

            // Redirige según el rol del usuario
            switch (usuario.Rol.ToLower())
            {
                case "admin":
                    return RedirectToPage("/Index");

                case "artista":
                    return RedirectToPage("/VistaArtista/Index");

                case "free":
                case "premium":
                    return RedirectToPage("/Cliente/Index");

                default:
                    ErrorMensaje = "Rol de usuario no reconocido.";
                    return Page();
            }
        }
    }
}
