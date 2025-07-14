using ProyectoReservaCanchasMAUI.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Data
{
    public class AppDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public AppDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);

            // Crear tablas
            _database.CreateTableAsync<Administrador>().Wait();
            _database.CreateTableAsync<Facultad>().Wait();
            _database.CreateTableAsync<Campus>().Wait();
            _database.CreateTableAsync<Cancha>().Wait();
        }

        // --------------- Administrador ----------------

        public Task<List<Administrador>> ObtenerAdministradoresAsync()
        {
            return _database.Table<Administrador>().ToListAsync();
        }

        public Task<List<Administrador>> ObtenerAdministradoresNoSincronizadosAsync()
        {
            return _database.Table<Administrador>()
                .Where(a => !a.Sincronizado)
                .ToListAsync();
        }

        public Task<int> GuardarAdministradorAsync(Administrador admin)
        {
            if (admin.BannerId != 0)
                return _database.UpdateAsync(admin);
            else
                return _database.InsertAsync(admin);
        }

        public Task<int> EliminarAdministradorAsync(Administrador admin)
        {
            return _database.DeleteAsync(admin);
        }


        // --------------- Facultad ----------------

        public Task<List<Facultad>> ObtenerFacultadesAsync()
        {
            return _database.Table<Facultad>().ToListAsync();
        }

        public Task<List<Facultad>> ObtenerFacultadesNoSincronizadasAsync()
        {
            return _database.Table<Facultad>()
                .Where(f => !f.Sincronizado)
                .ToListAsync();
        }

        public Task<int> GuardarFacultadAsync(Facultad facultad)
        {
            if (facultad.FacultadId != 0)
                return _database.UpdateAsync(facultad);
            else
                return _database.InsertAsync(facultad);
        }

        public Task<int> EliminarFacultadAsync(Facultad facultad)
        {
            return _database.DeleteAsync(facultad);
        }


        // --------------- Campus ----------------

        public Task<List<Campus>> ObtenerCampusAsync()
        {
            return _database.Table<Campus>().ToListAsync();
        }

        public Task<List<Campus>> ObtenerCampusNoSincronizadosAsync()
        {
            return _database.Table<Campus>()
                .Where(c => !c.Sincronizado)
                .ToListAsync();
        }

        public async Task<int> GuardarCampusAsync(Campus campus)
        {
            var existente = await _database.Table<Campus>()
                                           .Where(c => c.CampusId == campus.CampusId)
                                           .FirstOrDefaultAsync();

            if (existente == null)
            {
                return await _database.InsertAsync(campus);
            }
            else
            {
                return await _database.UpdateAsync(campus);
            }
        }
        public Task EliminarTodosCampusAsync()
        {
            return _database.DeleteAllAsync<Campus>();
        }
        public async Task<int> EliminarCampusAsync(Campus campus)
        {
            // Borrar por ID directamente en la base de datos
            return await _database.Table<Campus>()
                .Where(c => c.CampusId == campus.CampusId)
                .DeleteAsync();
        }
        // Método para limpiar la base de datos (útil para pruebas)

        // --------------- Cancha ----------------
        public Task<List<Cancha>> ObtenerCanchasAsync()
        {
            return _database.Table<Cancha>().ToListAsync();
        }

        public Task<List<Cancha>> ObtenerCanchasPorCampusAsync(int campusId)
        {
            return _database.Table<Cancha>()
                            .Where(c => c.CampusId == campusId)
                            .ToListAsync();
        }

        public Task<List<Cancha>> ObtenerCanchasNoSincronizadasAsync()
        {
            return _database.Table<Cancha>()
                            .Where(c => !c.Sincronizado)
                            .ToListAsync();
        }

        public Task<int> GuardarCanchaAsync(Cancha cancha)
        {
            if (cancha.CanchaId != 0)
                return _database.UpdateAsync(cancha);
            else
                return _database.InsertAsync(cancha);
        }

        public Task<int> EliminarCanchaAsync(Cancha cancha)
        {
            return _database.DeleteAsync(cancha);
        }
    }
}