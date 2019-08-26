using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhanVien.Model
{
    public class ThongTinChamCong
    {
        public NhanVien NhanVien { get; set; }
        public bool HanhChinh { get; set; }
        public bool TangCa { get; set; }
        public DateTime? GioBatDau { get; set; }
        public DateTime? GioKetThuc { get; set; }
        public DateTime? NgayChamCong { get; set; }

        public ThongTinChamCong() { }

        public ThongTinChamCong(ThongTinChamCong h)
        {
            NhanVien = h.NhanVien;
            HanhChinh = h.HanhChinh;
            TangCa = h.TangCa;
            GioBatDau = h.GioBatDau;
            GioKetThuc = h.GioKetThuc;
            NgayChamCong = h.NgayChamCong;
        }
    }
}
