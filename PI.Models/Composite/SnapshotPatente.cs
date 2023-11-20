using PI.Models.Composite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models
{
    public class SnapshotPatente
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

        public Patente Patente { get; set; }
        public Gaceta Gaceta { get; set; }
        public Cronologia cronologia { get; set; }
        public List<IPC> classificationIPC { get; set; }
        public List<vTitularDeLaPatente> titulares { get; set; }
        public Agente agente { get; set; }
        public List<vInventorDeLaPatente> inventores { get; set; }
        public List<vPrioridadDeLaPatente> prioridades { get; set; }

        public class _patente //data formated for printing purposes 
        {
            public string tituloP  { get; set; }
            public string kind { get; set; }
            public string solicitante { get; set; }
            public bool esEnNombrePropio { get; set; }
        }

        public _patente getFORMATEDDATA()
        {
            _patente res = new _patente();

            switch (this.TipoDeRegistroId)
            {
                case 10:
                    res.tituloP = "Nombre de la Invención";
                    res.kind = "A";
                    break;
                case 9:
                    res.tituloP = "Nombre del Modelo de Utilidad";            
                    res.kind = "U";
                    break;                    
                case 7:
                    res.tituloP = "Nombre del Diseño Industrial";            
                    res.kind = "S";
                    break;                    
                case 6:
                    res.tituloP = "Nombre del Diseño Industrial";
                    res.kind = "F";
                    break;     
            }               
    
            res.solicitante = String.Empty;
            this.titulares.ForEach(t => res.solicitante += t.Nombre + ", ");
            if (res.solicitante.Length > 0)
            {
                res.solicitante = res.solicitante.Substring(0, res.solicitante.Length - 2);
            }

            if (this.agente == null || ( this.agente != null && String.IsNullOrEmpty(this.agente.Nombre))){
                res.solicitante = res.solicitante + ", quien actúa en nombre propio. ";
                res.esEnNombrePropio = true;
            }
            return res;
        }

        public string getSello(string llave)
        {
            return "#Id:" + llave + "#Exp:" + this.Numero + "#Dsc:" + this.Patente.Descripcion + "#Fecha:" + this.FechaDeEstatus.ToShortDateString() + "#Emisor:YOLODEMO";
        }

        public string getFirma()
        {
            return "#Exp:" + this.Numero + "#Emisor:" + this.cronologia.UsuarioIniciales;
        }
    }
}
