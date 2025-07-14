using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.DTOs;
using ProyectoReservaCanchasMAUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Services
{
    public class PersonalMantenimientoService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDatabase _db;

        public PersonalMantenimientoService(HttpClient httpClient, AppDatabase db)
        {
            _httpClient = httpClient;
            _db = db;
        }

        public async Task SincronizarDesdeApiAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/PersonalMantenimiento");
                if (!response.IsSuccessStatusCode)
                    return;

                var json = await response.Content.ReadAsStringAsync();
                var personalApi = System.Text.Json.JsonSerializer.Deserialize<List<PersonalMantenimiento>>(json,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (personalApi == null)
                    return;

                var personalLocales = await _db.ObtenerPersonalMantenimientoAsync();

                foreach (var personal in personalApi)
                {
                    personal.Sincronizado = true;

                    var local = personalLocales.FirstOrDefault(p => p.BannerId == personal.BannerId);
                    await _db.GuardarPersonalMantenimientoAsync(personal);
                }

                var idsApi = personalApi.Select(p => p.BannerId).ToHashSet();
                foreach (var local in personalLocales)
                {
                    if (local.Sincronizado && !idsApi.Contains(local.BannerId))
                    {
                        await _db.EliminarPersonalMantenimientoAsync(local);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sincronizando personal de mantenimiento desde API: {ex.Message}");
            }
        }

        public async Task SincronizarLocalesConApiAsync()
        {
            var noSincronizados = await _db.ObtenerPersonalMantenimientoNoSincronizadosAsync();

            foreach (var personal in noSincronizados)
            {
                try
                {
                    await GuardarPersonalMantenimientoTotalAsync(personal);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error sincronizando personal BannerId {personal.BannerId}: {ex.Message}");
                }
            }
        }

        public async Task<List<PersonalMantenimiento>> ObtenerPersonalMantenimientoLocalAsync()
        {
            var lista = await _db.ObtenerPersonalMantenimientoAsync();
            Debug.WriteLine($"Personal de mantenimiento en base local: {lista.Count}");
            return lista;
        }

        public async Task GuardarPersonalMantenimientoTotalAsync(PersonalMantenimiento personal)
        {
            if (personal == null) throw new ArgumentNullException(nameof(personal));

            var dto = new PersonalMantenimientoDTO
            {
                BannerId = personal.BannerId,
                Nombre = personal.Nombre,
                Correo = personal.Correo,
                Password = personal.Password,
                Telefono = personal.Telefono,
                Direccion = personal.Direccion,
                FechaNacimiento = personal.FechaNacimiento
            };

            HttpResponseMessage response;

            if (personal.BannerId == 0)
            {
                response = await _httpClient.PostAsJsonAsync("api/PersonalMantenimiento", dto);
            }
            else
            {
                response = await _httpClient.PutAsJsonAsync($"api/PersonalMantenimiento/{personal.BannerId}", dto);
            }

            if (response.IsSuccessStatusCode)
            {
                if (personal.BannerId == 0)
                {
                    var dtoRespuesta = await response.Content.ReadFromJsonAsync<PersonalMantenimientoDTO>();
                    personal.BannerId = dtoRespuesta.BannerId;
                }

                personal.Sincronizado = true;
            }
            else
            {
                personal.Sincronizado = false;
            }

            await _db.GuardarPersonalMantenimientoAsync(personal);
        }

        public async Task EliminarTotalAsync(PersonalMantenimiento personal)
        {
            if (personal == null) throw new ArgumentNullException(nameof(personal));

            if (personal.BannerId == 0)
            {
                await _db.EliminarPersonalMantenimientoAsync(personal);
                return;
            }

            var response = await _httpClient.DeleteAsync($"api/PersonalMantenimiento/{personal.BannerId}");

            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await _db.EliminarPersonalMantenimientoAsync(personal);
            }
            else
            {
                var contenido = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al eliminar personal en API: {contenido}");
            }
        }
    }
}

