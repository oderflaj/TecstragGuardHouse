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
    
    public partial class persona
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public persona()
        {
            this.personavehiculo = new HashSet<personavehiculo>();
        }
    
        public int id { get; set; }
        public string numeroid { get; set; }
        public string tipoid { get; set; }
        public string nombre { get; set; }
        public string foto { get; set; }
        public string estatus { get; set; }
        public Nullable<bool> listanegra { get; set; }
        public string listanegradetalle { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<personavehiculo> personavehiculo { get; set; }
    }
}