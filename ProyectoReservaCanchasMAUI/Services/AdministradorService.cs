using ProyectoReservaCanchasMAUI.Models;
using System.Net.Http.Json;

namespace ProyectoReservaCanchasMAUI.Services
{
    public class AdministradorService
    {
        private readonly HttpClient _httpClient;

        public AdministradorService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7004") // <-- cambia esto por tu URL base
            };
        }

        public async Task<List<Administrador>> ObtenerAdministradoresAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Administrador>>("api/Administrador");
                return response ?? new List<Administrador>();
            }
            catch (Exception ex)
            {
                // Puedes manejar el error aquí o lanzar
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Administrador>();
            }
        }
        // Se busco este codigo de ChatGPT para poder conectar las APIs con el FrontEnd de la app.
    }
}
