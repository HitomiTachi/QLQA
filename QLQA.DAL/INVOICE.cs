//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QLQA.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class INVOICE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public INVOICE()
        {
            this.INVOICE_DETAILS = new HashSet<INVOICE_DETAILS>();
        }
    
        public string MaHoaDon { get; set; }
        public System.DateTime NgayLap { get; set; }
        public string MaNV { get; set; }
    
        public virtual EMPLOYERS EMPLOYERS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INVOICE_DETAILS> INVOICE_DETAILS { get; set; }
    }
}
