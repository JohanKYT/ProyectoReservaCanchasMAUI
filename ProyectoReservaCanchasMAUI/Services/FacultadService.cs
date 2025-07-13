using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;
using System.Net.Http.Json;
using ProyectoReservaCanchasMAUI.Data;

public class FacultadService
{
    private readonly HttpClient _httpClient;
    private readonly AppDatabase _db;

    public FacultadService(HttpClient httpClient, AppDatabase db)
    {
        _httpClient = httpClient;
        _db = db;
    }

    // Obtener datos locales (rápido)
    public Task<List<Facultad>> ObtenerFacultadesLocalesAsync()
    {
        return _db.ObtenerFacultadesAsync();
    }

    // Sincronizar datos desde API a SQLite local (carga inicial o refresco)
    public async Task SincronizarDesdeApiAsync()
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

    // Sincronizar locales no sincronizados hacia API
    public async Task SincronizarLocalesConApiAsync()
    {
        var locales = await _db.ObtenerFacultadesNoSincronizadasAsync();

        foreach (var facultad in locales)
        {
            HttpResponseMessage response;
            var dto = new FacultadDTO
            {
                FacultadId = facultad.FacultadId,
                Nombre = facultad.Nombre,
                CampusId = facultad.CampusId
            };

            if (facultad.FacultadId == 0)
                response = await _httpClient.PostAsJsonAsync("api/Facultades", dto);
            else
                response = await _httpClient.PutAsJsonAsync($"api/Facultades/{facultad.FacultadId}", dto);

            if (response.IsSuccessStatusCode)
            {
                facultad.Sincronizado = true;
                await _db.GuardarFacultadAsync(facultad);
            }
        }
    }

    // Guardar local + API (llama a sincronizar locales con API o guarda directo)
    public async Task GuardarFacultadTotalAsync(Facultad facultad)
    {
        // Guardar local con Sincronizado=false para que se sincronice luego
        facultad.Sincronizado = false;
        await _db.GuardarFacultadAsync(facultad);
    }

    // Eliminar local + API
    public async Task EliminarTotalAsync(Facultad facultad)
    {
        var response = await _httpClient.DeleteAsync($"api/Facultades/{facultad.FacultadId}");
        if (response.IsSuccessStatusCode)
        {
            await _db.EliminarFacultadAsync(facultad);
        }
    }
}
