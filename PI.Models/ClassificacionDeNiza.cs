//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ClassificacionDeNiza
    {
        public ClassificacionDeNiza()
        {
            this.Marcas = new HashSet<Marca>();
        }
    
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string ProductosQueAmpara { get; set; }
    
        public virtual ICollection<Marca> Marcas { get; set; }
    }
}
