using System.Net.Http.Json;
using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;

namespace ProyectoReservaCanchasMAUI.Services
{
    public class FacultadService
    {
        private readonly HttpClient _httpClient;

        public FacultadService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7004/") // Reemplaza con tu URL si es diferente
            };
        }

        public async Task<List<FacultadDTO>> ObtenerFacultadesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<FacultadDTO>>("api/Facultades");
                return response ?? new List<FacultadDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ObtenerFacultadesAsync: {ex.Message}");
                return new List<FacultadDTO>();
            }
        }

        public async Task<bool> AgregarFacultadAsync(FacultadDTO facultad)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Facultades", facultad);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error AgregarFacultadAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarFacultadAsync(FacultadDTO facultad)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Facultades/{facultad.Id}", facultad);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ActualizarFacultadAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EliminarFacultadAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Facultades/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error EliminarFacultadAsync: {ex.Message}");
                return false;
            }
        }
    }
}
