using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.DTOs;
using ProyectoReservaCanchasMAUI.Models;
using System.Diagnostics;
using System.Net.Http.Json;

namespace ProyectoReservaCanchasMAUI.Services
{
    public class CanchaService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDatabase _database;

        public CanchaService(HttpClient httpClient, AppDatabase database)
        {
            _httpClient = httpClient;
            _database = database;
        }

        public async Task<List<Cancha>> ObtenerCanchasLocalAsync()
        {
            return await _database.ObtenerCanchasAsync();
        }

        public async Task SincronizarLocalesConApiAsync()
        {
            var localesNoSincronizados = await _database.ObtenerCanchasNoSincronizadasAsync();

            foreach (var cancha in localesNoSincronizados)
            {
                if (cancha.CanchaId == 0)
                {
                    var dto = new CanchaDTO
                    {
                        Nombre = cancha.Nombre,
                        Tipo = cancha.Tipo,
                        Disponible = cancha.Disponible,
                        CampusId = cancha.CampusId
                    };

                    var response = await _httpClient.PostAsJsonAsync("api/Canchas", dto);

                    if (response.IsSuccessStatusCode)
                    {
                        var nuevoDto = await response.Content.ReadFromJsonAsync<CanchaDTO>();
                        cancha.CanchaId = nuevoDto.CanchaId;
                        cancha.Sincronizado = true;
                    }
                    else
                    {
                        cancha.Sincronizado = false;
                    }

                    await _database.GuardarCanchaAsync(cancha);
                }
                else
                {
                    var dto = new CanchaDTO
                    {
                        CanchaId = cancha.CanchaId,
                        Nombre = cancha.Nombre,
                        Tipo = cancha.Tipo,
                        Disponible = cancha.Disponible,
                        CampusId = cancha.CampusId
                    };

                    var response = await _httpClient.PutAsJsonAsync($"api/Canchas/{cancha.CanchaId}", dto);

                    cancha.Sincronizado = response.IsSuccessStatusCode;
                    await _database.GuardarCanchaAsync(cancha);
                }
            }
        }

        public async Task SincronizarDesdeApiAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Canchas");
                if (!response.IsSuccessStatusCode)
                    return;

                var listaApi = await response.Content.ReadFromJsonAsync<List<CanchaDTO>>();
                if (listaApi == null)
                    return;

                var listaLocal = await _database.ObtenerCanchasAsync();

                foreach (var dto in listaApi)
                {
                    var cancha = listaLocal.FirstOrDefault(c => c.CanchaId == dto.CanchaId);

                    if (cancha == null)
                    {
                        cancha = new Cancha
                        {
                            CanchaId = dto.CanchaId,
                            Nombre = dto.Nombre,
                            Tipo = dto.Tipo,
                            Disponible = dto.Disponible,
                            CampusId = dto.CampusId,
                            Sincronizado = true
                        };
                        await _database.GuardarCanchaAsync(cancha);
                    }
                    else
                    {
                        cancha.Nombre = dto.Nombre;
                        cancha.Tipo = dto.Tipo;
                        cancha.Disponible = dto.Disponible;
                        cancha.CampusId = dto.CampusId;
                        cancha.Sincronizado = true;
                        await _database.GuardarCanchaAsync(cancha);
                    }
                }

                // Eliminar locales sincronizados que ya no existen en API
                var apiIds = listaApi.Select(d => d.CanchaId).ToHashSet();
                foreach (var canchaLocal in listaLocal)
                {
                    if (canchaLocal.Sincronizado && !apiIds.Contains(canchaLocal.CanchaId))
                    {
                        await _database.EliminarCanchaAsync(canchaLocal);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CanchaService] Error al sincronizar desde API: {ex.Message}");
            }
        }


        public async Task GuardarCanchaTotalAsync(Cancha cancha)
        {
            if (cancha == null)
                throw new ArgumentNullException(nameof(cancha));

            if (cancha.CanchaId == 0)
            {
                var dto = new CanchaDTO
                {
                    Nombre = cancha.Nombre,
                    Tipo = cancha.Tipo,
                    Disponible = cancha.Disponible,
                    CampusId = cancha.CampusId
                };

                var response = await _httpClient.PostAsJsonAsync("api/Canchas", dto);

                if (response.IsSuccessStatusCode)
                {
                    var nuevoDto = await response.Content.ReadFromJsonAsync<CanchaDTO>();
                    cancha.CanchaId = nuevoDto.CanchaId;
                    cancha.Sincronizado = true;
                }
                else
                {
                    cancha.Sincronizado = false;
                }

                await _database.GuardarCanchaAsync(cancha);
            }
            else
            {
                var dto = new CanchaDTO
                {
                    CanchaId = cancha.CanchaId,
                    Nombre = cancha.Nombre,
                    Tipo = cancha.Tipo,
                    Disponible = cancha.Disponible,
                    CampusId = cancha.CampusId
                };

                var response = await _httpClient.PutAsJsonAsync($"api/Canchas/{cancha.CanchaId}", dto);

                cancha.Sincronizado = response.IsSuccessStatusCode;
                await _database.GuardarCanchaAsync(cancha);
            }
        }

        public async Task EliminarTotalAsync(Cancha cancha)
        {
            if (cancha == null) throw new ArgumentNullException(nameof(cancha));

            if (cancha.CanchaId == 0)
            {
                await _database.EliminarCanchaAsync(cancha);
                return;
            }

            var url = $"api/Canchas/{cancha.CanchaId}";

            var response = await _httpClient.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
            {
                await _database.EliminarCanchaAsync(cancha);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await _database.EliminarCanchaAsync(cancha);
            }
            else
            {
                var contenido = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al eliminar cancha en API: {contenido}");
            }
        }
    }
}
