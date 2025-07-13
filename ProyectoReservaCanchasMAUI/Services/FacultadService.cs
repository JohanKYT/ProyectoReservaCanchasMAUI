using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;
using System.Net.Http.Json;
using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.Interfaces;

namespace ProyectoReservaCanchasMAUI.Services;

public class FacultadService : ISincronizable
{
    private readonly HttpClient _httpClient;
    private readonly AppDatabase _db;

    public FacultadService(HttpClient httpClient, AppDatabase db)
{
    _httpClient = httpClient;
    _db = db;
}

    public async Task SincronizarDesdeApiAsync()
    {
        try
        {
            var listaDTO = await _httpClient.GetFromJsonAsync<List<FacultadDTO>>("api/Facultades");
            if (listaDTO == null) return;

            foreach (var dto in listaDTO)
            {
                var facultad = new Facultad
                {
                    FacultadId = dto.FacultadId,
                    Nombre = dto.Nombre,
                    CampusId = dto.CampusId,
                    Sincronizado = true
                };
                await _db.GuardarFacultadAsync(facultad);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al sincronizar facultades: {ex.Message}");
        }
    }

    public async Task SincronizarLocalesConApiAsync()
    {
        var locales = await _db.ObtenerFacultadesNoSincronizadasAsync();

        foreach (var facultad in locales)
        {
            try
            {
                var dto = new FacultadDTO
                {
                    FacultadId = facultad.FacultadId,
                    Nombre = facultad.Nombre,
                    CampusId = facultad.CampusId
                };

                HttpResponseMessage response;

                if (facultad.FacultadId == 0)
                    response = await _httpClient.PostAsJsonAsync("api/Facultades", dto);
                else
                    response = await _httpClient.PutAsJsonAsync($"api/Facultades/{facultad.FacultadId}", dto);

                if (response.IsSuccessStatusCode)
                {
                    facultad.Sincronizado = true;
                    await _db.GuardarFacultadAsync(facultad);
                    Console.WriteLine($"Facultad sincronizada: {facultad.Nombre}");
                }
                else
                {
                    Console.WriteLine($"Error al sincronizar facultad: {facultad.Nombre}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al sincronizar facultad: {ex.Message}");
            }
        }
    }

    public Task<List<Facultad>> ObtenerFacultadesLocalesAsync() => _db.ObtenerFacultadesAsync();
    public Task<int> GuardarLocalAsync(Facultad f) => _db.GuardarFacultadAsync(f);
    public Task<int> EliminarFacutadLocalAsync(Facultad f) => _db.EliminarFacultadAsync(f);

    public async Task GuardarFacultadTotalAsync(Facultad facultad)
    {
        await _db.GuardarFacultadAsync(facultad);

        try
        {
            var dto = new FacultadDTO
            {
                FacultadId = facultad.FacultadId,
                Nombre = facultad.Nombre,
                CampusId = facultad.CampusId
            };

            HttpResponseMessage response;

            if (facultad.FacultadId == 0)
                response = await _httpClient.PostAsJsonAsync("api/Facultades", dto);
            else
                response = await _httpClient.PutAsJsonAsync($"api/Facultades/{facultad.FacultadId}", dto);

            if (response.IsSuccessStatusCode)
            {
                facultad.Sincronizado = true;
                await _db.GuardarFacultadAsync(facultad);
                Console.WriteLine("Facultad enviada a la API exitosamente");
            }
            else
            {
                facultad.Sincronizado = false;
                await _db.GuardarFacultadAsync(facultad);
                Console.WriteLine("Fallo al subir facultad a la API");
            }
        }
        catch (Exception ex)
        {
            facultad.Sincronizado = false;
            await _db.GuardarFacultadAsync(facultad);
            Console.WriteLine($"Error al subir facultad a la API: {ex.Message}");
        }
    }

    public async Task EliminarTotalAsync(Facultad f)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/Facultades/{f.FacultadId}");
            if (response.IsSuccessStatusCode)
                await _db.EliminarFacultadAsync(f);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error eliminando: {ex.Message}");
        }
    }
}

