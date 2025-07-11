using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;
using System.Net.Http.Json;
using ProyectoReservaCanchasMAUI.Data;

namespace ProyectoReservaCanchasMAUI.Services;

public class CampusService
{
    private readonly HttpClient _httpClient;
    private readonly AppDatabase _db;

    public CampusService(AppDatabase db)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7004/") // Cambia por la URL de tu API
        };
        _db = db;
    }
    public async Task SincronizarCampusDesdeApiAsync()
    {
        try
        {
            var listaDTO = await _httpClient.GetFromJsonAsync<List<CampusDTO>>("api/Campus");
            if (listaDTO == null) return;

            foreach (var dto in listaDTO)
            {
                var campus = new Campus
                {
                    Id = dto.CampusId,
                    Nombre = dto.Nombre,
                    Direccion = dto.Direccion
                };
                await _db.GuardarCampusAsync(campus);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al sincronizar: {ex.Message}");
        }
    }
    public Task<List<Campus>> ObtenerCampusLocalAsync() => _db.ObtenerCampusAsync();

    public Task<int> GuardarCampusAsync(Campus campus) => _db.GuardarCampusAsync(campus);

    public Task<int> EliminarCampusAsync(Campus campus) => _db.EliminarCampusAsync(campus);
}

