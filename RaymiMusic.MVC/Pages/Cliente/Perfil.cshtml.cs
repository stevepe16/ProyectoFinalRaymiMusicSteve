using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RaymiMusic.Modelos;
using System.Net.Http.Json;

namespace RaymiMusic.MVC.Pages.Oyente
{
    public class PerfilModel : PageModel
    {
        private readonly HttpClient _http;

        public PerfilModel(HttpClient http)
        {
            _http = http;
        }

        public Usuario? Usuario { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var correo = HttpContext.Session.GetString("Correo");
            if (string.IsNullOrEmpty(correo))
                return RedirectToPage("/Cuenta/Login");

            Usuario = await _http.GetFromJsonAsync<Usuario>(
                $"https://localhost:7153/api/usuarios/porcorreo?correo={correo}");

            if (Usuario == null)
                return NotFound("No se encontró el usuario.");

            return Page();
        }
    }
}
