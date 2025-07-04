using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RaymiMusic.Api.Data;
using RaymiMusic.Data;
using RaymiMusic.Modelos;
using System;
using System.Security.Claims;

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
            // Verifica si el formulario fue enviado
            Console.WriteLine("Formulario enviado");

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == Correo);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(Contrasena, usuario.HashContrasena))
            {
                ErrorMensaje = "Correo o contraseña incorrectos.";
                return Page(); 
            }

            if (usuario.Correo == "admin@gmail.com")
            {

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Correo),
            new Claim(ClaimTypes.Role, "Admin")
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                Console.WriteLine("Redirigiendo a /Index");
                return RedirectToPage("/Index");
            }

            if (usuario.Rol == "Cliente")
            {

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Correo),
            new Claim(ClaimTypes.Role, "Cliente")  
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                Console.WriteLine("Redirigiendo a Clientes/Index");
                return RedirectToPage("/Clientes/Index");
            }
            ErrorMensaje = "No tienes acceso como cliente o administrador.";
            return Page(); 
        }

    }
}
