using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;
using System.Net.Http.Json;

namespace ProyectoReservaCanchasMAUI.Services
{
    public class CampusService
    {
        private readonly HttpClient _httpClient;

        public CampusService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7004/")
            };
        }

        public async Task<List<CampusDTO>> ObtenerCampusAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<CampusDTO>>("api/Campus");
                return response ?? new List<CampusDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ObtenerCampusAsync: {ex.Message}");
                return new List<CampusDTO>();
            }
        }

        public async Task<bool> AgregarCampusAsync(CampusDTO campus)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Campus", campus);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error AgregarCampusAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarCampusAsync(CampusDTO campus)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Campus/{campus.Id}", campus);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ActualizarCampusAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EliminarCampusAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Campus/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error EliminarCampusAsync: {ex.Message}");
                return false;
            }
        }
    }
}

