using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.DTOs;
using ProyectoReservaCanchasMAUI.Models;
using System.Diagnostics;
using System.Net.Http.Json;


namespace ProyectoReservaCanchasMAUI.Services
{
    public class FacultadService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDatabase _database;

        public FacultadService(HttpClient httpClient, AppDatabase database)
        {
            _httpClient = httpClient;
            _database = database;
        }

        public async Task<List<Facultad>> ObtenerFacultadesLocalAsync()
        {
            return await _database.ObtenerFacultadesAsync();
        }

        public async Task SincronizarLocalesConApiAsync()
        {
            var localesNoSincronizados = await _database.ObtenerFacultadesNoSincronizadasAsync();

            foreach (var facultadLocal in localesNoSincronizados)
            {
                if (facultadLocal.FacultadId == 0)
                {
                    var dto = new FacultadDTO
                    {
                        Nombre = facultadLocal.Nombre,
                        CampusId = facultadLocal.CampusId
                    };

                    var response = await _httpClient.PostAsJsonAsync("api/Facultades", dto);

                    if (response.IsSuccessStatusCode)
                    {
                        var nuevoDto = await response.Content.ReadFromJsonAsync<FacultadDTO>();
                        facultadLocal.FacultadId = nuevoDto.FacultadId;
                        facultadLocal.Sincronizado = true;
                        await _database.GuardarFacultadAsync(facultadLocal);
                    }
                    else
                    {
                        facultadLocal.Sincronizado = false;
                        await _database.GuardarFacultadAsync(facultadLocal);
                    }
                }
                else
                {
                    var dto = new FacultadDTO
                    {
                        FacultadId = facultadLocal.FacultadId,
                        Nombre = facultadLocal.Nombre,
                        CampusId = facultadLocal.CampusId
                    };

                    var response = await _httpClient.PutAsJsonAsync($"api/Facultades/{facultadLocal.FacultadId}", dto);

                    if (response.IsSuccessStatusCode)
                    {
                        facultadLocal.Sincronizado = true;
                        await _database.GuardarFacultadAsync(facultadLocal);
                    }
                    else
                    {
                        facultadLocal.Sincronizado = false;
                        await _database.GuardarFacultadAsync(facultadLocal);
                    }
                }
            }
        }

        public async Task SincronizarDesdeApiAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Facultades");
                if (!response.IsSuccessStatusCode)
                    return;

                var listaApi = await response.Content.ReadFromJsonAsync<List<FacultadDTO>>();
                if (listaApi == null)
                    return;

                var listaLocal = await _database.ObtenerFacultadesAsync();

                foreach (var dto in listaApi)
                {
                    var facultad = listaLocal.FirstOrDefault(f => f.FacultadId == dto.FacultadId);

                    if (facultad == null)
                    {
                        facultad = new Facultad
                        {
                            FacultadId = dto.FacultadId,
                            Nombre = dto.Nombre,
                            CampusId = dto.CampusId,
                            Sincronizado = true
                        };
                        await _database.GuardarFacultadAsync(facultad);
                    }
                    else
                    {
                        facultad.Nombre = dto.Nombre;
                        facultad.CampusId = dto.CampusId;
                        facultad.Sincronizado = true;
                        await _database.GuardarFacultadAsync(facultad);
                    }
                }

                // Eliminar locales sincronizados que ya no existen en API
                var apiIds = listaApi.Select(d => d.FacultadId).ToHashSet();
                foreach (var localFacultad in listaLocal)
                {
                    if (localFacultad.Sincronizado && !apiIds.Contains(localFacultad.FacultadId))
                    {
                        await _database.EliminarFacultadAsync(localFacultad);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FacultadService] Error al sincronizar desde API: {ex.Message}");
            }
        }


        public async Task GuardarFacultadTotalAsync(Facultad facultad)
        {
            if (facultad == null) throw new ArgumentNullException(nameof(facultad));

            if (facultad.FacultadId == 0) // Nuevo facultad local sin ID válido
            {
                var dto = new FacultadDTO
                {
                    Nombre = facultad.Nombre,
                    CampusId = facultad.CampusId
                };

                var response = await _httpClient.PostAsJsonAsync("api/Facultades", dto);

                if (response.IsSuccessStatusCode)
                {
                    var nuevoDto = await response.Content.ReadFromJsonAsync<FacultadDTO>();

                    // Actualizar el ID local con el que retornó la API
                    facultad.FacultadId = nuevoDto.FacultadId;
                    facultad.Sincronizado = true;

                    await _database.GuardarFacultadAsync(facultad);
                }
                else
                {
                    facultad.Sincronizado = false;
                    await _database.GuardarFacultadAsync(facultad);
                }
            }
            else
            {
                var dto = new FacultadDTO
                {
                    FacultadId = facultad.FacultadId,
                    Nombre = facultad.Nombre,
                    CampusId = facultad.CampusId
                };

                var response = await _httpClient.PutAsJsonAsync($"api/Facultades/{facultad.FacultadId}", dto);

                if (response.IsSuccessStatusCode)
                {
                    facultad.Sincronizado = true;
                    await _database.GuardarFacultadAsync(facultad);
                }
                else
                {
                    facultad.Sincronizado = false;
                    await _database.GuardarFacultadAsync(facultad);
                }
            }
        }

        public async Task EliminarTotalAsync(Facultad facultad)
        {
            if (facultad == null) throw new ArgumentNullException(nameof(facultad));

            // Verificar si tiene dependencias antes de proceder
            bool tieneDependencias = await TieneDependenciasFacultadAsync(facultad.FacultadId);
            if (tieneDependencias)
            {
                // Mostrar mensaje si tiene dependencias
                throw new Exception("No se puede eliminar la facultad porque tiene dependencias asociadas.");
            }

            if (facultad.FacultadId == 0)
            {
                // No tiene ID válido, eliminar solo localmente
                Debug.WriteLine("Eliminando facultad local sin ID válido.");
                await _database.EliminarFacultadAsync(facultad);
                return;
            }

            var url = $"api/Facultades/{facultad.FacultadId}";
            Debug.WriteLine("URL DELETE => " + url);

            var response = await _httpClient.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
            {
                await _database.EliminarFacultadAsync(facultad);
                Debug.WriteLine("Facultad eliminada exitosamente en API y local.");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Debug.WriteLine("Facultad no encontrada en API (404), eliminando localmente.");
                await _database.EliminarFacultadAsync(facultad);
            }
            else
            {
                var contenido = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("ERROR DELETE => " + contenido);
                throw new Exception($"Error al eliminar facultad en API: {contenido}");
            }
        }

        public async Task<bool> TieneDependenciasFacultadAsync(int facultadId)
        {
            return await _database.TieneDependenciasFacultadAsync(facultadId);
        }

    }
}



