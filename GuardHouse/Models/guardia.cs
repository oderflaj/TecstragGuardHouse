//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GuardHouse.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class guardia
    {
        public int id { get; set; }
        public int idusuario { get; set; }
        public string nombre { get; set; }
        public string edad { get; set; }
        public string sexo { get; set; }
        public string edocivil { get; set; }
        public string puesto { get; set; }
        public string empresa { get; set; }
        public string estatus { get; set; }
        public System.DateTime fecharegistro { get; set; }
    
        public virtual usuario usuario { get; set; }
    }
}
