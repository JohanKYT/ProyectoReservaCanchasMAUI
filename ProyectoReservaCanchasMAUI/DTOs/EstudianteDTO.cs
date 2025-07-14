using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.DTOs
{
    public class EstudianteDTO
    {
        public int BannerId { get; set; }

        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string TipoPersona => "Estudiante";

        public int CarreraId { get; set; }
        public CarreraDTO? Carrera { get; set; }
    }
}
