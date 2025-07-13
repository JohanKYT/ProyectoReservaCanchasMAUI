using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.DTOs;
using ProyectoReservaCanchasMAUI.Interfaces;
using ProyectoReservaCanchasMAUI.Models;
using System.Net.Http.Json;

public class AdministradorService : ISincronizable
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
            var listaDTO = await _httpClient.GetFromJsonAsync<List<AdministradorDTO>>("api/Administradores");
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
                    TipoPersona = dto.TipoPersona,
                    FacultadId = dto.FacultadId,
                    Sincronizado = true
                };

                await _db.GuardarAdministradorAsync(admin);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al sincronizar administradores: {ex.Message}");
        }
    }

    public async Task SincronizarLocalesConApiAsync()
    {
        var locales = await _db.ObtenerAdministradoresNoSincronizadosAsync();

        foreach (var admin in locales)
        {
            try
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
                    TipoPersona = admin.TipoPersona,
                    FacultadId = admin.FacultadId
                };

                HttpResponseMessage response;

                if (admin.BannerId == 0)
                    response = await _httpClient.PostAsJsonAsync("api/Administradores", dto);
                else
                    response = await _httpClient.PutAsJsonAsync($"api/Administradores/{admin.BannerId}", dto);

                if (response.IsSuccessStatusCode)
                {
                    admin.Sincronizado = true;
                    await _db.GuardarAdministradorAsync(admin);
                    Console.WriteLine($"Administrador sincronizado: {admin.Nombre}");
                }
                else
                {
                    Console.WriteLine($"Error al sincronizar administrador: {admin.Nombre}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al sincronizar administrador: {ex.Message}");
            }
        }
    }

    public Task<List<Administrador>> ObtenerAdministradoresLocalesAsync() => _db.ObtenerAdministradoresAsync();
    public Task<int> GuardarLocalAsync(Administrador a) => _db.GuardarAdministradorAsync(a);
    public Task<int> EliminarAdministradorLocalAsync(Administrador a) => _db.EliminarAdministradorAsync(a);

    public async Task GuardarTotalAsync(Administrador admin)
    {
        await _db.GuardarAdministradorAsync(admin);

        try
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
                TipoPersona = admin.TipoPersona,
                FacultadId = admin.FacultadId
            };

            HttpResponseMessage response;

            if (admin.BannerId == 0)
                response = await _httpClient.PostAsJsonAsync("api/Administradores", dto);
            else
                response = await _httpClient.PutAsJsonAsync($"api/Administradores/{admin.BannerId}", dto);

            if (response.IsSuccessStatusCode)
            {
                admin.Sincronizado = true;
                await _db.GuardarAdministradorAsync(admin);
                Console.WriteLine("Administrador enviado a la API exitosamente");
            }
            else
            {
                admin.Sincronizado = false;
                await _db.GuardarAdministradorAsync(admin);
                Console.WriteLine("Fallo al subir administrador a la API");
            }
        }
        catch (Exception ex)
        {
            admin.Sincronizado = false;
            await _db.GuardarAdministradorAsync(admin);
            Console.WriteLine($"Error al subir administrador a la API: {ex.Message}");
        }
    }

    public async Task EliminarTotalAsync(Administrador admin)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/Administradores/{admin.BannerId}");
            if (response.IsSuccessStatusCode)
                await _db.EliminarAdministradorAsync(admin);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error eliminando administrador: {ex.Message}");
        }
    }
}