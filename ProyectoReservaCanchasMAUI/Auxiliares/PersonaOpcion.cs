using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Auxiliares
{
    public class PersonaOpcion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; } // "Estudiante", "Administrador", "PersonalMantenimiento"
        public override string ToString() => $"{Tipo}: {Nombre}";
    }
}
