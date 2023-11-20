using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Models.Composite
{
    public class vInventorDeLaPatente : Inventor
    {
        public string NombrePais { get; set; }
        public string CodigoPais { get; set; }
    }
}
