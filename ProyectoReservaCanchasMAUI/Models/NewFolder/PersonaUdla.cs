using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Models.NewFolder
{
    public abstract class PersonaUdla
    {
        public int BannerId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string TipoPersona { get; set; } = string.Empty;
    }
}
