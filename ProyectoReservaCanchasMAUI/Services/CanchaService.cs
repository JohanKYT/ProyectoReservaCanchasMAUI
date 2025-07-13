using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;
using ProyectoReservaCanchasMAUI.Data;
using System.Net.Http.Json;
using ProyectoReservaCanchasMAUI.Interfaces;

namespace ProyectoReservaCanchasMAUI.Services;

public class CanchaService : ISincronizable
{
    private readonly HttpClient _httpClient;
    private readonly AppDatabase _db;

    public CanchaService(HttpClient httpClient, AppDatabase db)
    {
        _httpClient = httpClient;
        _db = db;
    }

    public async Task SincronizarDesdeApiAsync()
    {
        try
        {
            var listaDTO = await _httpClient.GetFromJsonAsync<List<CanchaDTO>>("api/Canchas");
            if (listaDTO == null) return;

            foreach (var dto in listaDTO)
            {
                var cancha = new Cancha
                {
                    CanchaId = dto.CanchaId,
                    Nombre = dto.Nombre,
                    Tipo = dto.Tipo,
                    Disponible = dto.Disponible,
                    Sincronizado = true
                };

                await _db.GuardarCanchaAsync(cancha);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al sincronizar canchas: {ex.Message}");
        }
    }

    public async Task SincronizarLocalesConApiAsync()
    {
        var locales = await _db.ObtenerCanchasNoSincronizadasAsync();

        foreach (var cancha in locales)
        {
            try
            {
                var dto = new CanchaDTO
                {
                    CanchaId = cancha.CanchaId,
                    Nombre = cancha.Nombre,
                    Tipo = cancha.Tipo,
                    Disponible = cancha.Disponible
                };

                HttpResponseMessage response;

                if (cancha.CanchaId == 0)
                    response = await _httpClient.PostAsJsonAsync("api/Canchas", dto);
                else
                    response = await _httpClient.PutAsJsonAsync($"api/Canchas/{cancha.CanchaId}", dto);

                if (response.IsSuccessStatusCode)
                {
                    cancha.Sincronizado = true;
                    await _db.GuardarCanchaAsync(cancha);
                    Console.WriteLine($"Cancha sincronizada: {cancha.Nombre}");
                }
                else
                {
                    Console.WriteLine($"Error al sincronizar cancha: {cancha.Nombre}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al sincronizar cancha: {ex.Message}");
            }
        }
    }

    public Task<List<Cancha>> ObtenerCanchasLocalesAsync() => _db.ObtenerCanchasAsync();
    public Task<int> GuardarLocalAsync(Cancha c) => _db.GuardarCanchaAsync(c);
    public Task<int> EliminarCanchaLocalAsync(Cancha c) => _db.EliminarCanchaAsync(c);

    public async Task GuardarCanchaTotalAsync(Cancha cancha)
    {
        await _db.GuardarCanchaAsync(cancha);

        try
        {
            var dto = new CanchaDTO
            {
                CanchaId = cancha.CanchaId,
                Nombre = cancha.Nombre,
                Tipo = cancha.Tipo,
                Disponible = cancha.Disponible
            };

            HttpResponseMessage response;

            if (cancha.CanchaId == 0)
                response = await _httpClient.PostAsJsonAsync("api/Canchas", dto);
            else
                response = await _httpClient.PutAsJsonAsync($"api/Canchas/{cancha.CanchaId}", dto);

            if (response.IsSuccessStatusCode)
            {
                cancha.Sincronizado = true;
                await _db.GuardarCanchaAsync(cancha);
                Console.WriteLine("Cancha enviada a la API exitosamente");
            }
            else
            {
                cancha.Sincronizado = false;
                await _db.GuardarCanchaAsync(cancha);
                Console.WriteLine("Fallo al subir cancha a la API");
            }
        }
        catch (Exception ex)
        {
            cancha.Sincronizado = false;
            await _db.GuardarCanchaAsync(cancha);
            Console.WriteLine($"Error al subir cancha a la API: {ex.Message}");
        }
    }

    public async Task EliminarTotalAsync(Cancha c)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/Canchas/{c.CanchaId}");
            if (response.IsSuccessStatusCode)
                await _db.EliminarCanchaAsync(c);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error eliminando cancha: {ex.Message}");
        }
    }
}
