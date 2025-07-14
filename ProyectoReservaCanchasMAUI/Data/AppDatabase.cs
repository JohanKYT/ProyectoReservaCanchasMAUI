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
            _database.CreateTableAsync<Carrera>().Wait();
            _database.CreateTableAsync<Estudiante>().Wait();
            _database.CreateTableAsync<PersonalMantenimiento>().Wait();
            _database.CreateTableAsync<Calendario>().Wait();
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

        public async Task<int> GuardarAdministradorAsync(Administrador admin)
        {
            var existente = await _database.Table<Administrador>()
                                           .Where(a => a.BannerId == admin.BannerId)
                                           .FirstOrDefaultAsync();

            if (existente == null)
            {
                return await _database.InsertAsync(admin);
            }
            else
            {
                return await _database.UpdateAsync(admin);
            }
        }

        public Task EliminarTodosAdministradoresAsync()
        {
            return _database.DeleteAllAsync<Administrador>();
        }

        public async Task<int> EliminarAdministradorAsync(Administrador admin)
        {
            return await _database.Table<Administrador>()
                .Where(a => a.BannerId == admin.BannerId)
                .DeleteAsync();
        }

        // Método para verificar si tiene dependencias (si aplica, sino retorna false)
        public Task<bool> TieneDependenciasAdministradorAsync(int bannerId)
        {
            // Si no tiene dependencias, retorna false
            return Task.FromResult(false);
        }

        // Estudiante
        public Task<List<Estudiante>> ObtenerEstudiantesAsync()
        {
            return _database.Table<Estudiante>().ToListAsync();
        }

        public Task<List<Estudiante>> ObtenerEstudiantesNoSincronizadosAsync()
        {
            return _database.Table<Estudiante>().Where(e => !e.Sincronizado).ToListAsync();
        }

        public async Task<int> GuardarEstudianteAsync(Estudiante estudiante)
        {
            var existente = await _database.Table<Estudiante>()
                .Where(e => e.BannerId == estudiante.BannerId)
                .FirstOrDefaultAsync();

            if (existente == null)
                return await _database.InsertAsync(estudiante);
            else
                return await _database.UpdateAsync(estudiante);
        }

        public Task<int> EliminarEstudianteAsync(Estudiante estudiante)
        {
            return _database.Table<Estudiante>()
                .Where(e => e.BannerId == estudiante.BannerId)
                .DeleteAsync();
        }

        // PersonalMantenimiento
        public Task<List<PersonalMantenimiento>> ObtenerPersonalMantenimientoAsync()
        {
            return _database.Table<PersonalMantenimiento>().ToListAsync();
        }

        public Task<List<PersonalMantenimiento>> ObtenerPersonalMantenimientoNoSincronizadosAsync()
        {
            return _database.Table<PersonalMantenimiento>()
                .Where(p => !p.Sincronizado)
                .ToListAsync();
        }

        public async Task<int> GuardarPersonalMantenimientoAsync(PersonalMantenimiento personal)
        {
            var existente = await _database.Table<PersonalMantenimiento>()
                .Where(p => p.BannerId == personal.BannerId)
                .FirstOrDefaultAsync();

            if (existente == null)
                return await _database.InsertAsync(personal);
            else
                return await _database.UpdateAsync(personal);
        }

        public Task<int> EliminarPersonalMantenimientoAsync(PersonalMantenimiento personal)
        {
            return _database.Table<PersonalMantenimiento>()
                .Where(p => p.BannerId == personal.BannerId)
                .DeleteAsync();
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

        public async Task<int> GuardarFacultadAsync(Facultad facultad)
        {
            var existente = await _database.Table<Facultad>()
                                           .Where(f => f.FacultadId == facultad.FacultadId)
                                           .FirstOrDefaultAsync();

            if (existente == null)
            {
                return await _database.InsertAsync(facultad);
            }
            else
            {
                return await _database.UpdateAsync(facultad);
            }
        }

        public Task EliminarTodosFacultadesAsync()
        {
            return _database.DeleteAllAsync<Facultad>();
        }

        public async Task<int> EliminarFacultadAsync(Facultad facultad)
        {
            // Borrar por ID directamente en la base de datos
            return await _database.Table<Facultad>()
                .Where(f => f.FacultadId == facultad.FacultadId)
                .DeleteAsync();
        }

        // Método extra para verificar si tiene dependencias (ejemplo: Carreras)
        public async Task<bool> TieneDependenciasFacultadAsync(int facultadId)
        {
            // Verificar si existen registros de administradores asociados a la facultad
            var count = await _database.Table<Administrador>()
                                        .Where(a => a.FacultadId == facultadId)
                                        .CountAsync();
            return count > 0;
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

        // ----------- Cancha --------------

        public Task<List<Cancha>> ObtenerCanchasAsync()
        {
            return _database.Table<Cancha>().ToListAsync();
        }

        public Task<List<Cancha>> ObtenerCanchasNoSincronizadasAsync()
        {
            return _database.Table<Cancha>()
                .Where(c => !c.Sincronizado)
                .ToListAsync();
        }

        public async Task<int> GuardarCanchaAsync(Cancha cancha)
        {
            var existente = await _database.Table<Cancha>()
                                           .Where(c => c.CanchaId == cancha.CanchaId)
                                           .FirstOrDefaultAsync();

            if (existente == null)
            {
                return await _database.InsertAsync(cancha);
            }
            else
            {
                return await _database.UpdateAsync(cancha);
            }
        }

        public Task EliminarCanchaAsync(Cancha cancha)
        {
            return _database.DeleteAsync(cancha);
        }

        //Metodos para Carrera
        public Task<List<Carrera>> ObtenerCarrerasAsync()
        {
            return _database.Table<Carrera>().ToListAsync();
        }

        public Task<List<Carrera>> ObtenerCarrerasNoSincronizadasAsync()
        {
            return _database.Table<Carrera>()
                            .Where(c => !c.Sincronizado)
                            .ToListAsync();
        }

        public async Task<int> GuardarCarreraAsync(Carrera carrera)
        {
            var existente = await _database.Table<Carrera>()
                                           .Where(c => c.CarreraId == carrera.CarreraId)
                                           .FirstOrDefaultAsync();

            if (existente == null)
                return await _database.InsertAsync(carrera);
            else
                return await _database.UpdateAsync(carrera);
        }

        public Task EliminarCarreraAsync(Carrera carrera)
        {
            return _database.DeleteAsync(carrera);
        }

        // -------------------- Calendario ---------------------

        public Task<List<Calendario>> ObtenerCalendariosAsync()
        {
            return _database.Table<Calendario>().ToListAsync();
        }

        public Task<List<Calendario>> ObtenerCalendariosNoSincronizadosAsync()
        {
            return _database.Table<Calendario>()
                            .Where(c => !c.Sincronizado)
                            .ToListAsync();
        }

        public async Task<int> GuardarCalendarioAsync(Calendario calendario)
        {
            if (calendario == null) throw new ArgumentNullException(nameof(calendario));

            var existente = await _database.Table<Calendario>()
                                           .Where(c => c.CalendarioId == calendario.CalendarioId)
                                           .FirstOrDefaultAsync();

            if (existente == null)
                return await _database.InsertAsync(calendario);

            // Actualizar solo si hay cambios o siempre actualizar?
            // Para robustez, siempre actualizar para mantener sincronización.
            return await _database.UpdateAsync(calendario);
        }

        public Task EliminarTodosCalendariosAsync()
        {
            return _database.DeleteAllAsync<Calendario>();
        }

        public async Task<int> EliminarCalendarioAsync(Calendario calendario)
        {
            if (calendario == null) throw new ArgumentNullException(nameof(calendario));

            return await _database.Table<Calendario>()
                                  .Where(c => c.CalendarioId == calendario.CalendarioId)
                                  .DeleteAsync();
        }

        // Para consultas específicas, como ObtenerReservasPorPersonaAsync, sería algo así:
        public Task<List<Calendario>> ObtenerReservasPorPersonaAsync(int personaUdlaId)
        {
            return _database.Table<Calendario>()
                            .Where(c => c.PersonaUdlaId == personaUdlaId)
                            .ToListAsync();
        }

    }
}