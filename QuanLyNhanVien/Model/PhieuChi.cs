//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuanLyNhanVien.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class PhieuChi
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PhieuChi()
        {
            this.ChiTietPhieuChi = new HashSet<ChiTietPhieuChi>();
        }
    
        public int Ma_PC { get; set; }
        public int Ma_NV { get; set; }
        public Nullable<decimal> TriGia_PC { get; set; }
        public Nullable<System.DateTime> ThoiGianLap_PC { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietPhieuChi> ChiTietPhieuChi { get; set; }
        public virtual NhanVien NhanVien { get; set; }
    }
}
