using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;
using System.Net.Http.Json;

namespace ProyectoReservaCanchasMAUI.Services;

public class CampusService
{
    private readonly HttpClient _httpClient;

    public CampusService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7004/") // Cambia por la URL de tu API
        };
    }

    public async Task<List<CampusDTO>> ObtenerCampusAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<CampusDTO>>("api/Campus");
            return response ?? new List<CampusDTO>();
        }
        catch
        {
            return new List<CampusDTO>();
        }
    }

    public async Task<bool> AgregarCampusAsync(CampusDTO campus)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Campus", campus);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ActualizarCampusAsync(CampusDTO campus)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Campus/{campus.CampusId}", campus);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarCampusAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/Campus/{id}");
        return response.IsSuccessStatusCode;
    }
}

