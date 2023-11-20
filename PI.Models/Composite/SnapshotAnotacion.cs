using PI.Models.Composite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models
{
    public class SnapshotAnotacion
    {
        public int Id { get; set; }
        public int ModuloId { get; set; }
        public int TipoDeRegistroId { get; set; }
        public string Numero { get; set; }
        public System.DateTime FechaDeSolicitud { get; set; }
        public System.TimeSpan Hora { get; set; }
        public int EstatusId { get; set; }
        public System.DateTime FechaDeEstatus { get; set; }
        public int LeyId { get; set; }
        public Nullable<int> UbicacionId { get; set; }
        public Nullable<int> UbicacionUsuarioId { get; set; }
        public Nullable<System.DateTime> FechaTraslado { get; set; }
        public Nullable<int> ActualizadoPorUsuarioId { get; set; }
        public Nullable<System.DateTime> FechaActualizacion { get; set; }

        public Cronologia cronologia { get; set; }
        public Gaceta Gaceta { get; set; }

        public string getSello(string llave)
        {
            return "#Id:" + llave + "#Exp:" + this.Numero + "#Fecha:" + this.FechaDeEstatus.ToShortDateString() + "#Emisor:YOLODEMO";
        }

        public string getFirma()
        {
            return "#Exp:" + this.Numero + "#Emisor:" + this.cronologia.UsuarioIniciales;
        }
    }
}
