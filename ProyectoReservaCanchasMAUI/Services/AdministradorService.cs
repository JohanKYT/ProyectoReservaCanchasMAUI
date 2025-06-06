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
                BaseAddress = new Uri("https://localhost:7004/") // URL base de tu API
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
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Administrador>();
            }
        }

        public async Task<bool> AgregarAdministradorAsync(Administrador nuevo)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Administrador", nuevo);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarAdministradorAsync(Administrador actualizado)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Administrador/{actualizado.BannerId}", actualizado);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EliminarAdministradorAsync(int bannerId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Administrador/{bannerId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
