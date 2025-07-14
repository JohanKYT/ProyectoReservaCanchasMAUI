using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;
using ProyectoReservaCanchasMAUI.Data;
using System.Net.Http.Json;
using System.Diagnostics;

public class AdministradorService
{
    private readonly HttpClient _httpClient;
    private readonly AppDatabase _db;

    public AdministradorService(HttpClient httpClient, AppDatabase db)
    {
        _httpClient = httpClient;
        _db = db;
    }

    public async Task SincronizarDesdeApiAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Administrador");
            if (!response.IsSuccessStatusCode)
                return;

            var json = await response.Content.ReadAsStringAsync();
            var administradoresApi = System.Text.Json.JsonSerializer.Deserialize<List<Administrador>>(json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (administradoresApi == null)
                return;

            var adminLocal = await _db.ObtenerAdministradoresAsync();

            foreach (var admin in administradoresApi)
            {
                admin.Sincronizado = true;

                var localExistente = adminLocal.FirstOrDefault(a => a.BannerId == admin.BannerId);
                if (localExistente == null)
                {
                    await _db.GuardarAdministradorAsync(admin);
                }
                else
                {
                    await _db.GuardarAdministradorAsync(admin);
                }
            }

            var apiIds = administradoresApi.Select(a => a.BannerId).ToHashSet();

            foreach (var localAdmin in adminLocal)
            {
                if (localAdmin.Sincronizado && !apiIds.Contains(localAdmin.BannerId))
                {
                    await _db.EliminarAdministradorAsync(localAdmin);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error sincronizando administradores desde API: {ex.Message}");
        }
    }

    public async Task SincronizarLocalesConApiAsync()
    {
        var localesNoSincronizados = await _db.ObtenerAdministradoresNoSincronizadosAsync();

        foreach (var adminLocal in localesNoSincronizados)
        {
            try
            {
                await GuardarAdministradorTotalAsync(adminLocal);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sincronizando administrador BannerId {adminLocal.BannerId}: {ex.Message}");
            }
        }
    }

    public async Task<List<Administrador>> ObtenerAdministradoresLocalAsync()
    {
        var lista = await _db.ObtenerAdministradoresAsync();
        Debug.WriteLine($"Administradores en base local: {lista.Count}");
        return lista;
    }

    public async Task GuardarAdministradorTotalAsync(Administrador admin)
    {
        if (admin == null) throw new ArgumentNullException(nameof(admin));

        if (admin.BannerId == 0) // Nuevo local sin ID válido
        {
            var dto = new AdministradorDTO
            {
                Nombre = admin.Nombre,
                Correo = admin.Correo,
                Password = admin.Password,
                Telefono = admin.Telefono,
                Direccion = admin.Direccion,
                FechaNacimiento = admin.FechaNacimiento,
                FacultadId = admin.FacultadId
            };

            var response = await _httpClient.PostAsJsonAsync("api/Administrador", dto);

            if (response.IsSuccessStatusCode)
            {
                var nuevoDto = await response.Content.ReadFromJsonAsync<AdministradorDTO>();

                admin.BannerId = nuevoDto.BannerId;
                admin.Sincronizado = true;

                await _db.GuardarAdministradorAsync(admin);
            }
            else
            {
                admin.Sincronizado = false;
                await _db.GuardarAdministradorAsync(admin);
            }
        }
        else
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

            var response = await _httpClient.PutAsJsonAsync($"api/Administrador/{admin.BannerId}", dto);

            if (response.IsSuccessStatusCode)
            {
                admin.Sincronizado = true;
                await _db.GuardarAdministradorAsync(admin);
            }
            else
            {
                admin.Sincronizado = false;
                await _db.GuardarAdministradorAsync(admin);
            }
        }
    }

    public async Task EliminarTotalAsync(Administrador admin)
    {
        if (admin == null) throw new ArgumentNullException(nameof(admin));

        // Si no tiene ID válido, eliminar solo localmente
        if (admin.BannerId == 0)
        {
            Debug.WriteLine("Eliminando administrador local sin ID válido.");
            await _db.EliminarAdministradorAsync(admin);
            return;
        }

        var url = $"api/Administrador/{admin.BannerId}";
        Debug.WriteLine("URL DELETE => " + url);

        var response = await _httpClient.DeleteAsync(url);
        if (response.IsSuccessStatusCode)
        {
            // Eliminado en API, ahora localmente
            await _db.EliminarAdministradorAsync(admin);
            Debug.WriteLine("Administrador eliminado exitosamente en API y local.");
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // No encontrado en API, igual eliminar localmente para evitar inconsistencias
            Debug.WriteLine("Administrador no encontrado en API (404), eliminando localmente.");
            await _db.EliminarAdministradorAsync(admin);
        }
        else
        {
            var contenido = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("ERROR DELETE => " + contenido);
            throw new Exception($"Error al eliminar administrador en API: {contenido}");
        }
    }

}
