using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Common
{
    public enum TiposDeEdicto : int
    {
        Marca = 1,
        Sonora = 2,
        NombreComercial = 3,
        Propaganda = 4,
        Origen = 5,
        IndicacionGeografica = 6,
        Colectiva = 7,
        Certificacion = 8
    }


    public class TiposDeRegistroEnPatentes
    {
        public static RegistroYLetra DibujosIndustriales = new RegistroYLetra(6, "F"); 
        public static RegistroYLetra RegistrosDeDisenoIndustrial = new RegistroYLetra(7, "S");
        public static RegistroYLetra PatentesPrecautorias = new RegistroYLetra(8, "P");            
        public static RegistroYLetra ModelosDeUtilidad = new RegistroYLetra(9, "U");
        public static RegistroYLetra PatentesDeInvencion = new RegistroYLetra(10, "A");
        public static RegistroYLetra SolicitudesInternacionales = new RegistroYLetra(12, "W");

    }

    public class RegistroYLetra {
        public int Id;
        public string Codigo;

        public RegistroYLetra(int id, string codigo) {
            this.Id = id;
            this.Codigo = codigo;
        }
    }
}
