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
    
    public partial class LichSuNhanVien
    {
        public int Ma_LSNV { get; set; }
        public int Ma_NV { get; set; }
        public string MoTa_LSNV { get; set; }
        public Nullable<System.DateTime> ThoiGian_LSNV { get; set; }
    
        public virtual NhanVien NhanVien { get; set; }
    }
}
