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
    
    public partial class NhanVien
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NhanVien()
        {
            this.BangLuong = new HashSet<BangLuong>();
            this.ChamCongNgay = new HashSet<ChamCongNgay>();
            this.KhoanLuong = new HashSet<KhoanLuong>();
            this.KhoanNghiPhep = new HashSet<KhoanNghiPhep>();
            this.LichSuNhanVien = new HashSet<LichSuNhanVien>();
            this.NghiPhep = new HashSet<NghiPhep>();
            this.PhieuChi = new HashSet<PhieuChi>();
            this.TaiKhoan = new HashSet<TaiKhoan>();
        }
    
        public int Ma_NV { get; set; }
        public int Ma_PB { get; set; }
        public string HoTen_NV { get; set; }
        public Nullable<bool> GioiTinh_NV { get; set; }
        public Nullable<System.DateTime> NgaySinh_NV { get; set; }
        public Nullable<bool> TrangThai_NV { get; set; }
        public string ChucVu_NV { get; set; }
        public Nullable<System.DateTime> NgayVaoLam_NV { get; set; }
        public string Email_NV { get; set; }
        public string SoDienThoai_NV { get; set; }
        public string DiaChi_NV { get; set; }
        public string Avatar_NV { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BangLuong> BangLuong { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChamCongNgay> ChamCongNgay { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KhoanLuong> KhoanLuong { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KhoanNghiPhep> KhoanNghiPhep { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LichSuNhanVien> LichSuNhanVien { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NghiPhep> NghiPhep { get; set; }
        public virtual PhongBan PhongBan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhieuChi> PhieuChi { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TaiKhoan> TaiKhoan { get; set; }
    }
}