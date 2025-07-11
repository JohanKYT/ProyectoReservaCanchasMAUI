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
        }

        public Task<List<Campus>> ObtenerCampusAsync()
        {
            return _database.Table<Campus>().ToListAsync();
        }

        public Task<int> GuardarCampusAsync(Campus campus)
        {
            return campus.Id == 0 ? _database.InsertAsync(campus) : _database.UpdateAsync(campus);
        }

        public Task<int> EliminarCampusAsync(Campus campus)
        {
            return _database.DeleteAsync(campus);
        }
    }
}
