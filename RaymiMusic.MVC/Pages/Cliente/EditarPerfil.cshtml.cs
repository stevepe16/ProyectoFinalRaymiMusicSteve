using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RaymiMusic.Modelos;
using System.Net.Http.Json;

namespace RaymiMusic.MVC.Pages.Oyente
{
    public class EditarPerfilModel : PageModel
    {
        private readonly HttpClient _http;

        public EditarPerfilModel(HttpClient http)
        {
            _http = http;
        }

        [BindProperty]
        public Usuario Usuario { get; set; } = new Usuario();

        public async Task<IActionResult> OnGetAsync()
        {
            var correo = HttpContext.Session.GetString("Correo");
            if (string.IsNullOrEmpty(correo))
                return RedirectToPage("/Cuenta/Login");

            var usuario = await _http.GetFromJsonAsync<Usuario>(
                $"https://localhost:7153/api/usuarios/porcorreo?correo={correo}");

            if (usuario == null)
                return NotFound();

            Usuario = usuario;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // PUT a la API
            var result = await _http.PutAsJsonAsync(
                $"https://localhost:7153/api/usuarios/{Usuario.Id}",
                Usuario);

            if (!result.IsSuccessStatusCode)
                return BadRequest("Error al actualizar el perfil.");
            TempData["Mensaje"] = "Perfil actualizado correctamente.";
            return RedirectToPage("Perfil");
        }
    }
}
