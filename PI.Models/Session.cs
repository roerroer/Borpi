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
    
    public partial class Session
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Token { get; set; }
        public System.DateTime TokenExp { get; set; }
        public Nullable<int> FootMark { get; set; }
    }
}
