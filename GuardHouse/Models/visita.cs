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
    
    public partial class visita
    {
        public int id { get; set; }
        public System.DateTime fecEntrada { get; set; }
        public string horaEntrada { get; set; }
        public Nullable<System.DateTime> fecSalida { get; set; }
        public string horaSalida { get; set; }
        public int idvisitante { get; set; }
        public int idpropiedad { get; set; }
        public string estatuspropiedad { get; set; }
        public string detalle1 { get; set; }
        public string detalle2 { get; set; }
        public string nopropiedad { get; set; }
        public Nullable<int> idvehiculo { get; set; }
        public string domicilio { get; set; }
    
        public virtual visitante visitante { get; set; }
        public virtual vehiculo vehiculo { get; set; }
    }
}
