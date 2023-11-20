using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models.Composite
{
    public class vTitularDeLaPatente : TitularDeLaPatente
    {
        public string Nombre { get; set; }
        public string NombrePais { get; set; }
        public string CodigoPais { get; set; }
    }
}
