using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;
using System.Net.Http.Json;
using ProyectoReservaCanchasMAUI.Data;


namespace ProyectoReservaCanchasMAUI.Services;

public class CampusService 
{
    private readonly HttpClient _httpClient;
    private readonly AppDatabase _db;

    public CampusService(HttpClient httpClient, AppDatabase db)
    {
        _httpClient = httpClient;
        _db = db;
    }
    public async Task SincronizarDesdeApiAsync()
    {
        try
        {
            var listaDTO = await _httpClient.GetFromJsonAsync<List<CampusDTO>>("api/Campus");
            if (listaDTO == null) return;

            foreach (var dto in listaDTO)
            {
                var campus = new Campus
                {
                    CampusId = dto.CampusId,
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
    public async Task GuardarCampusTotalAsync(Campus campus)
    {
        try
        {
            var dto = new CampusDTO
            {
                CampusId = campus.CampusId,
                Nombre = campus.Nombre,
                Direccion = campus.Direccion
            };

            HttpResponseMessage response;

            if (campus.CampusId == 0)
                response = await _httpClient.PostAsJsonAsync("api/Campus", dto);
            else
                response = await _httpClient.PutAsJsonAsync($"api/Campus/{campus.CampusId}", dto);

            if (response.IsSuccessStatusCode)
            {
                campus.Sincronizado = true;
                Console.WriteLine("Guardado y sincronizado con API.");
            }
            else
            {
                campus.Sincronizado = false;
                Console.WriteLine("Guardado local, fallo al sincronizar con API.");
            }
        }
        catch (Exception ex)
        {
            campus.Sincronizado = false;
            Console.WriteLine($"Error al conectar con API, guardado local: {ex.Message}");
        }

        await _db.GuardarCampusAsync(campus);
    }

    public async Task SincronizarLocalesConApiAsync()
    {
        var localesNoSincronizados = await _db.ObtenerCampusNoSincronizadosAsync();

        foreach (var campus in localesNoSincronizados)
        {
            try
            {
                var dto = new CampusDTO
                {
                    CampusId = campus.CampusId,
                    Nombre = campus.Nombre,
                    Direccion = campus.Direccion
                };

                HttpResponseMessage response;

                if (campus.CampusId == 0)
                    response = await _httpClient.PostAsJsonAsync("api/Campus", dto);
                else
                    response = await _httpClient.PutAsJsonAsync($"api/Campus/{campus.CampusId}", dto);

                if (response.IsSuccessStatusCode)
                {
                    campus.Sincronizado = true;
                    await _db.GuardarCampusAsync(campus);
                    Console.WriteLine($"Campus sincronizado: {campus.Nombre}");
                }
                else
                {
                    Console.WriteLine($"Error al sincronizar campus: {campus.Nombre}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al sincronizar campus: {ex.Message}");
            }
        }
    }
    public Task<List<Campus>> ObtenerCampusLocalAsync() => _db.ObtenerCampusAsync();

    public Task<int> GuardarCampusAsync(Campus campus) => _db.GuardarCampusAsync(campus);

    public Task<int> EliminarCampusAsync(Campus campus) => _db.EliminarCampusAsync(campus);

}

