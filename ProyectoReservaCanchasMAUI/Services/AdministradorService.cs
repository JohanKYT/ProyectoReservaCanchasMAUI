using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;
using ProyectoReservaCanchasMAUI.Data;
using System.Net.Http.Json;

namespace ProyectoReservaCanchasMAUI.Services
{
    public class AdministradorService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDatabase _db;

        public AdministradorService(HttpClient httpClient, AppDatabase db)
        {
            _httpClient = httpClient;
            _db = db;
        }

        // Descargar datos desde API y guardarlos localmente
        public async Task SincronizarDesdeApiAsync()
        {
            var listaDTO = await _httpClient.GetFromJsonAsync<List<AdministradorDTO>>("api/Administrador");
            if (listaDTO == null) return;

            foreach (var dto in listaDTO)
            {
                var admin = new Administrador
                {
                    BannerId = dto.BannerId,
                    Nombre = dto.Nombre,
                    Correo = dto.Correo,
                    Password = dto.Password,
                    Telefono = dto.Telefono,
                    Direccion = dto.Direccion,
                    FechaNacimiento = dto.FechaNacimiento,
                    FacultadId = dto.FacultadId,
                    Sincronizado = true
                };
                await _db.GuardarAdministradorAsync(admin);
            }
        }

        // Subir locales no sincronizados a API
        public async Task SincronizarLocalesConApiAsync()
        {
            var localesNoSincronizados = await _db.ObtenerAdministradoresNoSincronizadosAsync();

            foreach (var admin in localesNoSincronizados)
            {
                var dto = new AdministradorDTO
                {
                    BannerId = admin.BannerId,
                    Nombre = admin.Nombre,
                    Correo = admin.Correo,
                    Password = admin.Password,
                    Telefono = admin.Telefono,
                    Direccion = admin.Direccion,
                    FechaNacimiento = admin.FechaNacimiento,
                    FacultadId = admin.FacultadId
                };

                HttpResponseMessage response;
                if (admin.BannerId == 0)
                    response = await _httpClient.PostAsJsonAsync("api/Administrador", dto);
                else
                    response = await _httpClient.PutAsJsonAsync($"api/Administrador/{admin.BannerId}", dto);

                if (response.IsSuccessStatusCode)
                {
                    admin.Sincronizado = true;
                    await _db.GuardarAdministradorAsync(admin);
                }
            }
        }

        // Guardar local y marcar para sincronizar luego
        public async Task GuardarTotalAsync(Administrador admin)
        {
            admin.Sincronizado = false;
            await _db.GuardarAdministradorAsync(admin);
        }

        // Eliminar local y en API
        public async Task EliminarTotalAsync(Administrador admin)
        {
            var response = await _httpClient.DeleteAsync($"api/Administrador/{admin.BannerId}");
            if (response.IsSuccessStatusCode)
            {
                await _db.EliminarAdministradorAsync(admin);
            }
        }

        // Obtener datos locales
        public Task<List<Administrador>> ObtenerAdministradoresLocalesAsync() =>
            _db.ObtenerAdministradoresAsync();
    }
}
