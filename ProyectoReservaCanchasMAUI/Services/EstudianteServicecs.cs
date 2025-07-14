using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.DTOs;
using ProyectoReservaCanchasMAUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Services
{
    public class EstudianteService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDatabase _db;

        public EstudianteService(HttpClient httpClient, AppDatabase db)
        {
            _httpClient = httpClient;
            _db = db;
        }

        public async Task SincronizarDesdeApiAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Estudiante");
                if (!response.IsSuccessStatusCode)
                    return;

                var json = await response.Content.ReadAsStringAsync();
                var estudiantesApi = System.Text.Json.JsonSerializer.Deserialize<List<Estudiante>>(json,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (estudiantesApi == null)
                    return;

                var estudiantesLocales = await _db.ObtenerEstudiantesAsync();

                foreach (var estudiante in estudiantesApi)
                {
                    estudiante.Sincronizado = true;

                    var local = estudiantesLocales.FirstOrDefault(e => e.BannerId == estudiante.BannerId);
                    await _db.GuardarEstudianteAsync(estudiante);
                }

                var idsApi = estudiantesApi.Select(e => e.BannerId).ToHashSet();
                foreach (var local in estudiantesLocales)
                {
                    if (local.Sincronizado && !idsApi.Contains(local.BannerId))
                    {
                        await _db.EliminarEstudianteAsync(local);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sincronizando estudiantes desde API: {ex.Message}");
            }
        }

        public async Task SincronizarLocalesConApiAsync()
        {
            var noSincronizados = await _db.ObtenerEstudiantesNoSincronizadosAsync();

            foreach (var estudiante in noSincronizados)
            {
                try
                {
                    await GuardarEstudianteTotalAsync(estudiante);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error sincronizando estudiante BannerId {estudiante.BannerId}: {ex.Message}");
                }
            }
        }

        public async Task<List<Estudiante>> ObtenerEstudiantesLocalAsync()
        {
            var lista = await _db.ObtenerEstudiantesAsync();
            Debug.WriteLine($"Estudiantes en base local: {lista.Count}");
            return lista;
        }

        public async Task GuardarEstudianteTotalAsync(Estudiante estudiante)
        {
            if (estudiante == null) throw new ArgumentNullException(nameof(estudiante));

            var dto = new EstudianteDTO
            {
                BannerId = estudiante.BannerId,
                Nombre = estudiante.Nombre,
                Correo = estudiante.Correo,
                Password = estudiante.Password,
                Telefono = estudiante.Telefono,
                Direccion = estudiante.Direccion,
                FechaNacimiento = estudiante.FechaNacimiento,
                CarreraId = estudiante.CarreraId
            };

            HttpResponseMessage response;

            if (estudiante.BannerId == 0)
            {
                response = await _httpClient.PostAsJsonAsync("api/Estudiante", dto);
            }
            else
            {
                response = await _httpClient.PutAsJsonAsync($"api/Estudiante/{estudiante.BannerId}", dto);
            }

            if (response.IsSuccessStatusCode)
            {
                if (estudiante.BannerId == 0)
                {
                    var dtoRespuesta = await response.Content.ReadFromJsonAsync<EstudianteDTO>();
                    estudiante.BannerId = dtoRespuesta.BannerId;
                }

                estudiante.Sincronizado = true;
            }
            else
            {
                estudiante.Sincronizado = false;
            }

            await _db.GuardarEstudianteAsync(estudiante);
        }

        public async Task EliminarTotalAsync(Estudiante estudiante)
        {
            if (estudiante == null) throw new ArgumentNullException(nameof(estudiante));

            if (estudiante.BannerId == 0)
            {
                await _db.EliminarEstudianteAsync(estudiante);
                return;
            }

            var response = await _httpClient.DeleteAsync($"api/Estudiante/{estudiante.BannerId}");

            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await _db.EliminarEstudianteAsync(estudiante);
            }
            else
            {
                var contenido = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al eliminar estudiante en API: {contenido}");
            }
        }
    }
}
