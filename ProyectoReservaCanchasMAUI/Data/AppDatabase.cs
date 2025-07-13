using ProyectoReservaCanchasMAUI.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Data
{
    public class AppDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public AppDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Campus>().Wait();
            _database.CreateTableAsync<Facultad>().Wait();
            _database.CreateTableAsync<Cancha>().Wait();
            _database.CreateTableAsync<Administrador>().Wait();
        }

        // Métodos para Administrador
        public Task<List<Administrador>> ObtenerAdministradoresAsync()
            => _database.Table<Administrador>().ToListAsync();

        public Task<int> GuardarAdministradorAsync(Administrador administrador)
        {
            return administrador.BannerId == 0
                ? _database.InsertAsync(administrador)
                : _database.UpdateAsync(administrador);
        }

        public Task<int> EliminarAdministradorAsync(Administrador administrador)
            => _database.DeleteAsync(administrador);
        
        // Métodos para Campus
        public Task<List<Campus>> ObtenerCampusAsync() => _database.Table<Campus>().ToListAsync();
        public Task<int> GuardarCampusAsync(Campus campus)
        {
            return campus.CampusId == 0 
                ? _database.InsertAsync(campus) 
                : _database.UpdateAsync(campus);
        }
        public Task<int> EliminarCampusAsync(Campus campus) => _database.DeleteAsync(campus);
        // Métodos para Facultad
        public Task<List<Facultad>> ObtenerFacultadesAsync() => _database.Table<Facultad>().ToListAsync();
        public Task<int> GuardarFacultadAsync(Facultad facultad)
        {
            return facultad.FacultadId == 0 
                ? _database.InsertAsync(facultad) 
                : _database.UpdateAsync(facultad);
        }
        public Task<int> EliminarFacultadAsync(Facultad facultad) => _database.DeleteAsync(facultad);
        // Métodos para Cancha
        public Task<List<Cancha>> ObtenerCanchasAsync() => _database.Table<Cancha>().ToListAsync();

        public Task<int> GuardarCanchaAsync(Cancha cancha)
        {
            return cancha.CanchaId == 0 
                ? _database.InsertAsync(cancha) 
                : _database.UpdateAsync(cancha);
        } 
        public Task<int> EliminarCanchaAsync(Cancha cancha) => _database.DeleteAsync(cancha);
        //Metodos para obtener los modelos no sincronizados
        public Task<List<Campus>> ObtenerCampusNoSincronizadosAsync()
        => _database.Table<Campus>().Where(c => !c.Sincronizado).ToListAsync();

        public Task<List<Facultad>> ObtenerFacultadesNoSincronizadasAsync()
            => _database.Table<Facultad>().Where(f => !f.Sincronizado).ToListAsync();

        public Task<List<Cancha>> ObtenerCanchasNoSincronizadasAsync()
            => _database.Table<Cancha>().Where(c => !c.Sincronizado).ToListAsync();
        public Task<List<Administrador>> ObtenerAdministradoresNoSincronizadosAsync()
            => _database.Table<Administrador>().Where(a => !a.Sincronizado).ToListAsync();
    }
}
