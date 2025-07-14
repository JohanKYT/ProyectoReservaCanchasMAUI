using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Dispatching;

namespace ProyectoReservaCanchasMAUI.Services
{
    public class CalendarioService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDatabase _db;

        public CalendarioService(HttpClient httpClient, AppDatabase db)
        {
            _httpClient = httpClient;
            _db = db;
        }

        /// <summary>
        /// Obtiene la lista local de calendarios (reservas).
        /// </summary>
        public async Task<List<Calendario>> ObtenerCalendariosLocalAsync()
        {
            return await _db.ObtenerCalendariosAsync();
        }

        /// <summary>
        /// Sincroniza la lista local con la API: envía los no sincronizados.
        /// </summary>
        public async Task SincronizarLocalesConApiAsync()
        {
            var localesNoSincronizados = await _db.ObtenerCalendariosNoSincronizadosAsync();

            foreach (var calendarioLocal in localesNoSincronizados)
            {
                try
                {
                    await GuardarCalendarioTotalAsync(calendarioLocal);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error sincronizando calendario ID {calendarioLocal.CalendarioId}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Descarga desde API y actualiza la base local (sincroniza API -> local).
        /// </summary>
        public async Task SincronizarDesdeApiAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Calendarios");
                if (!response.IsSuccessStatusCode)
                    return;

                var json = await response.Content.ReadAsStringAsync();
                var listaApi = System.Text.Json.JsonSerializer.Deserialize<List<CalendarioDTO>>(json,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (listaApi == null)
                    return;

                var locales = await _db.ObtenerCalendariosAsync();

                // Insertar o actualizar locales según datos API
                foreach (var dto in listaApi)
                {
                    var calendarioLocal = locales.FirstOrDefault(c => c.CalendarioId == dto.CalendarioId);

                    if (calendarioLocal == null)
                    {
                        calendarioLocal = new Calendario();
                    }

                    calendarioLocal.CalendarioId = dto.CalendarioId;
                    calendarioLocal.FechaHoraInicio = dto.FechaHoraInicio;
                    calendarioLocal.FechaHoraFin = dto.FechaHoraFin;
                    calendarioLocal.Estado = dto.Estado;
                    calendarioLocal.NotasDetallada = dto.NotasDetallada;
                    calendarioLocal.CanchaId = dto.CanchaId;
                    calendarioLocal.PersonaUdlaId = dto.PersonaUdlaId;
                    calendarioLocal.Sincronizado = true;

                    await _db.GuardarCalendarioAsync(calendarioLocal);
                }

                // Eliminar locales que ya no están en API (opcional)
                var apiIds = listaApi.Select(c => c.CalendarioId).ToHashSet();
                foreach (var local in locales)
                {
                    if (local.Sincronizado && !apiIds.Contains(local.CalendarioId))
                    {
                        await _db.EliminarCalendarioAsync(local);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sincronizando desde API (Calendario): {ex.Message}");

                // Mostrar alerta UI en hilo principal para informar modo offline
  
            }
        }

        /// <summary>
        /// Guarda (inserta o actualiza) calendario en local y API.
        /// </summary>

        public async Task GuardarCalendarioTotalAsync(Calendario calendario)
        {
            if (calendario == null)
                throw new ArgumentNullException(nameof(calendario));

            var dto = new CalendarioDTO
            {
                CalendarioId = calendario.CalendarioId,
                FechaHoraInicio = calendario.FechaHoraInicio,
                FechaHoraFin = calendario.FechaHoraFin,
                Estado = calendario.Estado,
                NotasDetallada = calendario.NotasDetallada,
                CanchaId = calendario.CanchaId,
                PersonaUdlaId = calendario.PersonaUdlaId
            };

            try
            {
                HttpResponseMessage response;

                if (calendario.CalendarioId == 0)
                {
                    // Nuevo registro - POST
                    response = await _httpClient.PostAsJsonAsync("api/Calendarios", dto);
                }
                else
                {
                    // Actualizar existente - PUT
                    response = await _httpClient.PutAsJsonAsync($"api/Calendarios/{calendario.CalendarioId}", dto);
                }

                if (response.IsSuccessStatusCode)
                {
                    var resultDto = await response.Content.ReadFromJsonAsync<CalendarioDTO>();

                    calendario.CalendarioId = resultDto.CalendarioId;
                    calendario.Sincronizado = true;
                }
                else
                {
                    calendario.Sincronizado = false;
                    Debug.WriteLine($"Error guardando calendario en API. StatusCode: {response.StatusCode}");
                }
            }
            catch (HttpRequestException hre)
            {
                calendario.Sincronizado = false;
                Debug.WriteLine($"Error HTTP guardando calendario: {hre.Message}");
            }
            catch (Exception ex)
            {
                calendario.Sincronizado = false;
                Debug.WriteLine($"Error inesperado guardando calendario: {ex.Message}");
            }

            // Guardar siempre localmente (incluyendo si no se sincronizó)
            await _db.GuardarCalendarioAsync(calendario);
        }

        /// <summary>
        /// Elimina un calendario en API y local.
        /// </summary>

        public async Task EliminarTotalAsync(Calendario calendario)
        {
            if (calendario == null)
                throw new ArgumentNullException(nameof(calendario));

            if (calendario.CalendarioId == 0)
            {
                // Solo eliminar localmente si no tiene id válido
                await _db.EliminarCalendarioAsync(calendario);
                return;
            }

            try
            {
                var response = await _httpClient.DeleteAsync($"api/Calendarios/{calendario.CalendarioId}");

                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _db.EliminarCalendarioAsync(calendario);
                    Debug.WriteLine($"Calendario eliminado exitosamente. ID: {calendario.CalendarioId}");
                }
                else
                {
                    var contenido = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("ERROR al eliminar calendario => " + contenido);
                    throw new Exception($"Error al eliminar calendario en API: {contenido}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error eliminando calendario: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene las reservas para una persona específica localmente.
        /// </summary>

        public async Task<List<Calendario>> ObtenerReservasPorPersonaAsync(int personaUdlaId)
        {
            try
            {
                return await _db.ObtenerReservasPorPersonaAsync(personaUdlaId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error obteniendo reservas por persona: {ex.Message}");
                return new List<Calendario>();
            }
        }
    }
}
