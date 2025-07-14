using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;
using ProyectoReservaCanchasMAUI.Data;
using System.Net.Http.Json;

namespace ProyectoReservaCanchasMAUI.Services
{
    public class FacultadService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDatabase _db;

        public FacultadService(HttpClient httpClient, AppDatabase db)
        {
            _httpClient = httpClient;
            _db = db;
        }

        // Descarga desde API a local
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

        // Sube datos locales no sincronizados a API
        public async Task SincronizarLocalesConApiAsync()
        {
            var localesNoSincronizados = await _db.ObtenerFacultadesNoSincronizadasAsync();

            foreach (var facultad in localesNoSincronizados)
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
                }
            }
        }

        // Guardar local y marcar para sincronizar luego
        public async Task GuardarFacultadTotalAsync(Facultad facultad)
        {
            facultad.Sincronizado = false;
            await _db.GuardarFacultadAsync(facultad);
        }

        // Eliminar local y API
        public async Task EliminarTotalAsync(Facultad facultad)
        {
            var response = await _httpClient.DeleteAsync($"api/Facultades/{facultad.FacultadId}");
            if (response.IsSuccessStatusCode)
            {
                await _db.EliminarFacultadAsync(facultad);
            }
        }

        // Obtener datos locales
        public Task<List<Facultad>> ObtenerFacultadesLocalesAsync() =>
            _db.ObtenerFacultadesAsync();
    }
}
