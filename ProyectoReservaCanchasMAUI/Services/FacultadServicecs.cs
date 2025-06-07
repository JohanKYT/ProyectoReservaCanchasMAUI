using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;
using System.Net.Http.Json;

namespace ProyectoReservaCanchasMAUI.Services;

public class FacultadService
{
    private readonly HttpClient _httpClient;

    public FacultadService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7004/") // URL de tu API
        };
    }

    public async Task<List<FacultadDTO>> ObtenerFacultadesAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<FacultadDTO>>("api/Facultades");
            return response ?? new List<FacultadDTO>();
        }
        catch
        {
            return new List<FacultadDTO>();
        }
    }

    public async Task<bool> AgregarFacultadAsync(FacultadDTO facultad)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Facultades", facultad);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ActualizarFacultadAsync(FacultadDTO facultad)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Facultades/{facultad.FacultadId}", facultad);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarFacultadAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/Facultades/{id}");
        return response.IsSuccessStatusCode;
    }
}

