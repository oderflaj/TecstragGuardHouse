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
    
    public partial class visitante
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public visitante()
        {
            this.vehiculo = new HashSet<vehiculo>();
            this.visita = new HashSet<visita>();
        }
    
        public int id { get; set; }
        public string numeroid { get; set; }
        public string tipoid { get; set; }
        public string nombre { get; set; }
        public string foto { get; set; }
        public string estatus { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<vehiculo> vehiculo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<visita> visita { get; set; }
    }
}