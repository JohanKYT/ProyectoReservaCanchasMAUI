using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.DTOs;
using ProyectoReservaCanchasMAUI.Models;
using System.Net.Http.Json;
using System.Diagnostics;

namespace ProyectoReservaCanchasMAUI.Services
{
    public class CarreraService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDatabase _database;

        public CarreraService(HttpClient httpClient, AppDatabase database)
        {
            _httpClient = httpClient;
            _database = database;
        }

        public async Task<List<Carrera>> ObtenerCarrerasLocalAsync()
        {
            return await _database.ObtenerCarrerasAsync();
        }

        public async Task SincronizarLocalesConApiAsync()
        {
            var localesNoSincronizados = await _database.ObtenerCarrerasNoSincronizadasAsync();

            foreach (var carrera in localesNoSincronizados)
            {
                if (carrera.CarreraId == 0)
                {
                    var dto = new CarreraDTO
                    {
                        Nombre = carrera.Nombre,
                        FacultadId = carrera.FacultadId
                    };

                    var response = await _httpClient.PostAsJsonAsync("api/Carreras", dto);

                    if (response.IsSuccessStatusCode)
                    {
                        var nuevoDto = await response.Content.ReadFromJsonAsync<CarreraDTO>();
                        carrera.CarreraId = nuevoDto.CarreraId;
                        carrera.Sincronizado = true;
                    }
                    else
                    {
                        carrera.Sincronizado = false;
                    }

                    await _database.GuardarCarreraAsync(carrera);
                }
                else
                {
                    var dto = new CarreraDTO
                    {
                        CarreraId = carrera.CarreraId,
                        Nombre = carrera.Nombre,
                        FacultadId = carrera.FacultadId
                    };

                    var response = await _httpClient.PutAsJsonAsync($"api/Carreras/{carrera.CarreraId}", dto);

                    carrera.Sincronizado = response.IsSuccessStatusCode;
                    await _database.GuardarCarreraAsync(carrera);
                }
            }
        }

        public async Task SincronizarDesdeApiAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Carreras");
                if (!response.IsSuccessStatusCode) return;

                var listaApi = await response.Content.ReadFromJsonAsync<List<CarreraDTO>>();
                if (listaApi == null) return;

                var listaLocal = await _database.ObtenerCarrerasAsync();

                foreach (var dto in listaApi)
                {
                    var carrera = listaLocal.FirstOrDefault(c => c.CarreraId == dto.CarreraId);
                    if (carrera == null)
                    {
                        carrera = new Carrera
                        {
                            CarreraId = dto.CarreraId,
                            Nombre = dto.Nombre,
                            FacultadId = dto.FacultadId,
                            Sincronizado = true
                        };
                        await _database.GuardarCarreraAsync(carrera);
                    }
                    else
                    {
                        carrera.Nombre = dto.Nombre;
                        carrera.FacultadId = dto.FacultadId;
                        carrera.Sincronizado = true;
                        await _database.GuardarCarreraAsync(carrera);
                    }
                }

                // Eliminar locales sincronizados que no están en API
                var apiIds = listaApi.Select(d => d.CarreraId).ToHashSet();
                foreach (var localCarrera in listaLocal)
                {
                    if (localCarrera.Sincronizado && !apiIds.Contains(localCarrera.CarreraId))
                        await _database.EliminarCarreraAsync(localCarrera);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CarreraService] Error al sincronizar desde API: {ex.Message}");
                // Opcional: puedes mostrar un mensaje o guardar en log interno si usas uno
            }
        }

        public async Task GuardarCarreraTotalAsync(Carrera carrera)
        {
            if (carrera == null)
                throw new ArgumentNullException(nameof(carrera));

            if (carrera.CarreraId == 0)
            {
                var dto = new CarreraDTO
                {
                    Nombre = carrera.Nombre,
                    FacultadId = carrera.FacultadId
                };

                var response = await _httpClient.PostAsJsonAsync("api/Carreras", dto);

                if (response.IsSuccessStatusCode)
                {
                    var nuevoDto = await response.Content.ReadFromJsonAsync<CarreraDTO>();
                    carrera.CarreraId = nuevoDto.CarreraId;
                    carrera.Sincronizado = true;
                    await _database.GuardarCarreraAsync(carrera);
                }
                else
                {
                    carrera.Sincronizado = false;
                    await _database.GuardarCarreraAsync(carrera);
                }
            }
            else
            {
                var dto = new CarreraDTO
                {
                    CarreraId = carrera.CarreraId,
                    Nombre = carrera.Nombre,
                    FacultadId = carrera.FacultadId
                };

                var response = await _httpClient.PutAsJsonAsync($"api/Carreras/{carrera.CarreraId}", dto);

                carrera.Sincronizado = response.IsSuccessStatusCode;
                await _database.GuardarCarreraAsync(carrera);
            }
        }

        public async Task EliminarTotalAsync(Carrera carrera)
        {
            if (carrera == null) throw new ArgumentNullException(nameof(carrera));

            if (carrera.CarreraId == 0)
            {
                await _database.EliminarCarreraAsync(carrera);
                return;
            }

            var url = $"api/Carreras/{carrera.CarreraId}";
            var response = await _httpClient.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
            {
                await _database.EliminarCarreraAsync(carrera);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await _database.EliminarCarreraAsync(carrera);
            }
            else
            {
                var contenido = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al eliminar carrera en API: {contenido}");
            }
        }
    }
}

