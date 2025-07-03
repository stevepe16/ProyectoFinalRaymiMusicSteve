    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.EntityFrameworkCore;
    using RaymiMusic.Api.Data;
    using RaymiMusic.Modelos;
    using RaymiMusic.MVC.Services;

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddRazorPages();

    // Configura HttpClient para consumir la API
    builder.Services.AddHttpClient("RaymiMusicApi", client =>
    {
        client.BaseAddress = new Uri("https://localhost:7153/");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    });

    // Configurar base de datos para usuarios
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("RaymiMusicDb")));

    // Configura sesiones
    builder.Services.AddSession();

    // Configura la autenticación con cookies
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Cuenta/Login";  
        });

    // Registra tus servicios de consumo
    builder.Services.AddScoped<IUsuarioApiService, UsuarioApiService>();
    builder.Services.AddScoped<IPlanesApiService, PlanesApiService>();
    builder.Services.AddScoped<IArtistasApiService, ArtistasApiService>();
    builder.Services.AddScoped<ICancionesApiService, CancionesApiService>();
    builder.Services.AddScoped<IGenerosApiService, GenerosApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// **Asegúrate de que UseSession() esté antes de UseAuthentication() y UseAuthorization()**
app.UseSession();   // Esta línea debe estar antes de UseAuthentication
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

