using System;
using System.IO;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Auxiliares
{
    public static class Logger
    {
        private static readonly string LogsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Logs");

        static Logger()
        {
            if (!Directory.Exists(LogsDirectory))
                Directory.CreateDirectory(LogsDirectory);
        }

        public static async Task LogAsync(string entidad, string accion, string mensaje)
        {
            try
            {
                string archivoLog = Path.Combine(LogsDirectory, $"{entidad.ToLower()}.log");
                string textoLog = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {accion.ToUpper()} | {mensaje}{Environment.NewLine}";
                await File.AppendAllTextAsync(archivoLog, textoLog);
            }
            catch
            {
                // Ignorar errores para no afectar la app
            }
        }
    }
}
