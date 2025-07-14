using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;
using ProyectoReservaCanchasMAUI.Data;
using System.Net.Http.Json;
using System.Diagnostics;

namespace ProyectoReservaCanchasMAUI.Services
{
    public class CampusService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDatabase _db;

        public CampusService(HttpClient httpClient, AppDatabase db)
        {
            _httpClient = httpClient;
            _db = db;
        }

        // Descargar datos desde API a SQLite local
        public async Task SincronizarDesdeApiAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Campus");
                if (!response.IsSuccessStatusCode)
                    return;

                var json = await response.Content.ReadAsStringAsync();
                var campusApi = System.Text.Json.JsonSerializer.Deserialize<List<Campus>>(json,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (campusApi == null)
                    return;

                var campusLocal = await _db.ObtenerCampusAsync();

                // Insertar o actualizar campus desde API
                foreach (var campus in campusApi)
                {
                    campus.Sincronizado = true;

                    var localExistente = campusLocal.FirstOrDefault(c => c.CampusId == campus.CampusId);

                    if (localExistente == null)
                    {
                        await _db.GuardarCampusAsync(campus);
                    }
                    else
                    {
                        await _db.GuardarCampusAsync(campus);
                    }
                }

                // Eliminar locales sincronizados que ya no existen en API
                var campusApiIds = campusApi.Select(c => c.CampusId).ToHashSet();

                foreach (var localCampus in campusLocal)
                {
                    if (localCampus.Sincronizado && !campusApiIds.Contains(localCampus.CampusId))
                    {
                        await _db.EliminarCampusAsync(localCampus);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sincronizando desde API: {ex.Message}");
            }
        }

        public async Task SincronizarLocalesConApiAsync()
        {
            var localesNoSincronizados = await _db.ObtenerCampusNoSincronizadosAsync();

            foreach (var campusLocal in localesNoSincronizados)
            {
                try
                {
                    await GuardarCampusTotalAsync(campusLocal);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error sincronizando campus ID {campusLocal.CampusId}: {ex.Message}");
                }
            }
        }

        public async Task<List<Campus>> ObtenerCampusLocalAsync()
        {
            var lista = await _db.ObtenerCampusAsync();
            Console.WriteLine($"Campus en base local: {lista.Count}");
            return lista;
        }



        // Guardar local y marcar para sincronizar luego
        public async Task GuardarCampusTotalAsync(Campus campus)
        {
            if (campus == null) throw new ArgumentNullException(nameof(campus));

            if (campus.CampusId == 0) // Nuevo campus local sin ID válido
            {
                var dto = new CampusDTO
                {
                    Nombre = campus.Nombre,
                    Direccion = campus.Direccion
                };

                var response = await _httpClient.PostAsJsonAsync("api/Campus", dto);

                if (response.IsSuccessStatusCode)
                {
                    var nuevoDto = await response.Content.ReadFromJsonAsync<CampusDTO>();

                    // Actualizar el ID local con el que retornó la API
                    campus.CampusId = nuevoDto.CampusId;
                    campus.Sincronizado = true;

                    await _db.GuardarCampusAsync(campus);
                }
                else
                {
                    campus.Sincronizado = false;
                    await _db.GuardarCampusAsync(campus);
                }
            }
            else
            {
                var dto = new CampusDTO
                {
                    CampusId = campus.CampusId,
                    Nombre = campus.Nombre,
                    Direccion = campus.Direccion
                };

                var response = await _httpClient.PutAsJsonAsync($"api/Campus/{campus.CampusId}", dto);

                if (response.IsSuccessStatusCode)
                {
                    campus.Sincronizado = true;
                    await _db.GuardarCampusAsync(campus);
                }
                else
                {
                    campus.Sincronizado = false;
                    await _db.GuardarCampusAsync(campus);
                }
            }
        }

        // Eliminar local y API
        public async Task EliminarTotalAsync(Campus campus)
        {
            var url = $"api/Campus/{campus.CampusId}";
            Debug.WriteLine("URL DELETE => " + url);

            var response = await _httpClient.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var eliminadas = await _db.EliminarCampusAsync(campus);
                Debug.WriteLine($"Campus eliminados localmente: {eliminadas}");
            }
            else
            {
                var contenido = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("ERROR DELETE => " + contenido);
                throw new Exception($"Error al eliminar campus en API: {contenido}");
            }
        }

    }
}
