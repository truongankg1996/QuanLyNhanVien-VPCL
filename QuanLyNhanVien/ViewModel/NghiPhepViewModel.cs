using QuanLyNhanVien.Model;
using QuanLyNhanVien.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace QuanLyNhanVien.ViewModel
{
    public class NghiPhepViewModel:BaseViewModel
    {
        #region DataContext
        private ObservableCollection<NghiPhep> _ListNghiPhep;
        public ObservableCollection<NghiPhep> ListNghiPhep { get => _ListNghiPhep; set { _ListNghiPhep = value; OnPropertyChanged(); } }
        #endregion

        #region Combobox item source
        private ObservableCollection<NhanVien> _ListNhanVien;
        public ObservableCollection<NhanVien> ListNhanVien { get => _ListNhanVien; set { _ListNhanVien = value; OnPropertyChanged(); } }
        private ObservableCollection<KhoanNghiPhep> _ListKhoanNghiPhep;
        public ObservableCollection<KhoanNghiPhep> ListKhoanNghiPhep { get => _ListKhoanNghiPhep; set { _ListKhoanNghiPhep = value; OnPropertyChanged(); } }
        #endregion

        #region Thuộc tính binding
        private DateTime? _NgayBatDau;
        public DateTime? NgayBatDau { get => _NgayBatDau; set { _NgayBatDau = value; OnPropertyChanged(); } }
        private DateTime? _NgayKetThuc;
        public DateTime? NgayKetThuc { get => _NgayKetThuc; set { _NgayKetThuc = value; OnPropertyChanged(); } }
        private string _LiDo;
        public string LiDo { get => _LiDo; set { _LiDo = value; OnPropertyChanged(); } }
        private NhanVien _SelectedNhanVien;
        public NhanVien SelectedNhanVien { get => _SelectedNhanVien; set { _SelectedNhanVien = value; OnPropertyChanged(); } }
        private KhoanNghiPhep _SelectedKhoanNghiPhep;
        public KhoanNghiPhep SelectedKhoanNghiPhep { get => _SelectedKhoanNghiPhep; set { _SelectedKhoanNghiPhep = value; OnPropertyChanged(); } }
        private NghiPhep _SelectedNghiPhep;
        public NghiPhep SelectedNghiPhep { get => _SelectedNghiPhep; set { _SelectedNghiPhep = value; OnPropertyChanged(); } }
        #endregion

        #region Thuộc tính khác
        private bool _IsEditable;
        public bool IsEditable { get => _IsEditable; set { _IsEditable = value; OnPropertyChanged(); } }
        private bool _IsNVChangeable;
        public bool IsNVChangeable { get => _IsNVChangeable; set { _IsNVChangeable = value; OnPropertyChanged(); } }
        private string _SearchNghiPhep;
        public string SearchNghiPhep { get => _SearchNghiPhep; set { _SearchNghiPhep = value; OnPropertyChanged(); } }
        private double _TongNgayNghi;
        public double TongNgayNghi { get => _TongNgayNghi; set { _TongNgayNghi = value; OnPropertyChanged(); } }
        public bool sort;
        #endregion

        #region Command
        public ICommand TaoMoiCommand { get; set; }
        public ICommand HienThiCommand { get; set; }
        public ICommand HuyCommand { get; set; }
        public ICommand LuuCommand { get; set; }
        public ICommand SuaCommand { get; set; }
        public ICommand XoaCommand { get; set; }
        public ICommand SortCommand { get; set; } // 2 Command này đc MainWindow biding để xử lý phần tim kiếm
        public ICommand SearchCommand { get; set; }
        public ICommand LoadDataNPCommand { get; set; }
        public ICommand HienThiKhoanNghiPhepCommand { get; set; }
        #endregion

        #region Thuộc tính ẩn hiện các Tab
        public enum ChucNangNghiPhep
        {
            NgayNghiPhep, KhoanNghiPhep, LoaiNghiPhep
        };
        private int _ChucNangNP;
        public int ChucNangNP { get => _ChucNangNP; set { _ChucNangNP = value; OnPropertyChanged(); } }
        public ICommand TabNgayNghiPhepCommand { get; set; }
        public ICommand TabKhoanNghiPhepCommand { get; set; }
        public ICommand TabLoaiNghiPhepCommand { get; set; }
        #endregion
        public NghiPhepViewModel()
        {
            #region Xử lý ẩn hiện Tab
            TabNgayNghiPhepCommand = new RelayCommand<Object>((p) =>
            {
                return true;
            }, (p) =>
            {
                ChucNangNP = (int)ChucNangNghiPhep.NgayNghiPhep;
            });

            TabKhoanNghiPhepCommand = new RelayCommand<Object>((p) =>
            {
                return true;
            }, (p) =>
            {
                ChucNangNP = (int)ChucNangNghiPhep.KhoanNghiPhep;
            });

            TabLoaiNghiPhepCommand = new RelayCommand<Object>((p) =>
            {
                return true;
            }, (p) =>
            {
                ChucNangNP = (int)ChucNangNghiPhep.LoaiNghiPhep;
            });
            #endregion

            #region Khởi tạo
            LoadListNhanVien();
            IsEditable = false;
            #endregion

            #region Command

            #region Load Dữ Liệu Nghỉ Phép

            LoadDataNPCommand = new RelayCommand<Object>((p) =>
             {
                 return true;
             }, (p) =>
             {
                 LoadListNghiPhep();
             });
            #endregion

            #region Hiện thị Khoản Nghỉ Phép
            HienThiKhoanNghiPhepCommand = new RelayCommand<Object>((p) =>
            {
                return SelectedNhanVien == null ? false : true;
            }, (p) =>
            {
                LoadListKhoanNghiPhep(SelectedNhanVien.Ma_NV);
            });
            #endregion

            #region Tạo mới
            // Tạo mới => Button Tạo mới ở Main Window => Phòng Ban
            TaoMoiCommand = new RelayCommand<Object>((p) =>
            {
                if (ListNhanVien == null)
                {
                    MessageBox.Show("Vui lòng thiết lập Khoản nghỉ phép cho Nhân viên trước khi tạo Nghỉ phép mới", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
                return true;
            }, (p) =>
            {
                IsEditable = true;
                IsNVChangeable = true;
                ResetControls();
                LoadListNhanVien();

                NghiPhepWindow window = new NghiPhepWindow();
                window.ShowDialog();
            });
            #endregion

            #region Hiện Thị
            //Hiện thị
            HienThiCommand = new RelayCommand<Object>((p) =>
            {
                if (SelectedNghiPhep == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                IsEditable = false;
                IsNVChangeable = false;

                HieenThi();

                NghiPhepWindow window = new NghiPhepWindow();
                window.ShowDialog();
            });
            #endregion

            #region Hủy
            //Button Hủy
            HuyCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {
                MessageBoxResult result = MessageBox.Show("Mọi thay đổi nếu có sẽ không được lưu, bạn có chắc chắn không?", "Thông báo", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    IsEditable = false;
                    IsNVChangeable = false;
                    p.Close();
                }
            });
            #endregion

            #region Sửa
            SuaCommand = new RelayCommand<Object>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (SelectedNghiPhep == null)
                {
                    MessageBox.Show("Vui lòng chọn Ngày nghỉ phép để chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    IsNVChangeable = true;
                }
                else
                {

                    IsNVChangeable = false;
                }
                IsEditable = true;
            });
            #endregion

            #region Sort
            SortCommand = new RelayCommand<GridViewColumnHeader>((p) =>
            {
                return p == null ? false : true;
            }, (p) =>
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListNghiPhep);
                if (sort)
                {
                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription(p.Tag.ToString(), ListSortDirection.Ascending));
                }
                else
                {
                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription(p.Tag.ToString(), ListSortDirection.Descending));
                }
                sort = !sort;
            });
            #endregion

            #region Search
            SearchCommand = new RelayCommand<Object>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (string.IsNullOrEmpty(SearchNghiPhep))
                {
                    CollectionViewSource.GetDefaultView(ListNghiPhep).Filter = (all) => { return true; };
                }
                else
                {
                    CollectionViewSource.GetDefaultView(ListNghiPhep).Filter = (search) =>
                    {
                        return (search as NghiPhep).NhanVien.HoTen_NV.IndexOf(SearchNghiPhep, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (search as NghiPhep).NgayBatDau_NP.ToString().IndexOf(SearchNghiPhep, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (search as NghiPhep).NgayKetThuc_NP.ToString().IndexOf(SearchNghiPhep, StringComparison.OrdinalIgnoreCase) >= 0;
                    };
                }
            });
            #endregion

            #region Luu
            //Luu
            LuuCommand = new RelayCommand<Window>((p) =>
            {
                if (NgayBatDau == null || NgayKetThuc == null || LiDo == null || SelectedKhoanNghiPhep == null || SelectedNhanVien == null)
                {
                    MessageBox.Show("Vui lòng nhập hết thông tin nghỉ phép", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (NgayBatDau > NgayKetThuc)
                {
                    MessageBox.Show("Ngày kết thúc phải sau Ngày bất đầu !", "Thông báo!", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
                if (IsEditable == false)
                {
                    MessageBox.Show("Vui lòng chỉnh sửa thông tin trước khi lưu!", "Thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                return true;
            }, (p) =>
            {

                if (SelectedNghiPhep == null)
                {
                    TongNgayNghi = (NgayKetThuc.Value - NgayBatDau.Value).TotalDays + 1;

                    if (TongNgayNghi > SelectedKhoanNghiPhep.SoNgayNghi_KNP)
                    {
                        MessageBox.Show("Số ngày nghỉ của Khoảng nghỉ phép đang chọn không đủ! Vui lòng chọn lại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    var NghiPhepMoi = new NghiPhep()
                    {
                        NgayBatDau_NP = NgayBatDau,
                        NgayKetThuc_NP = NgayKetThuc,
                        LiDo_NP = LiDo,
                        Ma_NV = SelectedNhanVien.Ma_NV,
                        Ma_KNP = SelectedKhoanNghiPhep.Ma_KNP,

                    };

                    DataProvider.Ins.DB.NghiPhep.Add(NghiPhepMoi);

                    SelectedKhoanNghiPhep.SoNgayNghi_KNP -= (int)TongNgayNghi;

                    DataProvider.Ins.DB.SaveChanges();
                    ListNghiPhep.Add(NghiPhepMoi);
                    MessageBox.Show("Thêm Nghỉ Phếp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {

                    var NghiPhepSua = DataProvider.Ins.DB.NghiPhep.Where(x => x.Ma_NP == SelectedNghiPhep.Ma_NP).SingleOrDefault();

                    //Khôi phục lại số ngày nghỉ của Khoản nghỉ phép đã lưu
                    var oldKNP = DataProvider.Ins.DB.KhoanNghiPhep.Where(x => x.Ma_KNP == NghiPhepSua.Ma_KNP).SingleOrDefault();
                    double oldDays = (NghiPhepSua.NgayKetThuc_NP.Value - NghiPhepSua.NgayBatDau_NP.Value).TotalDays + 1;
                    oldKNP.SoNgayNghi_KNP += (int)oldDays;

                    //Kiểm tra số ngày còn lại nghỉ của Khoản nghỉ phép
                    TongNgayNghi = (NgayKetThuc.Value - NgayBatDau.Value).TotalDays + 1;

                    if (TongNgayNghi > SelectedKhoanNghiPhep.SoNgayNghi_KNP)
                    {
                        MessageBox.Show("Số ngày nghỉ của Khoảng nghỉ phép đang chọn không đủ! Vui lòng chọn lại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    NghiPhepSua.NgayBatDau_NP = NgayBatDau;
                    NghiPhepSua.NgayKetThuc_NP = NgayKetThuc;
                    NghiPhepSua.LiDo_NP = LiDo;
                    NghiPhepSua.Ma_KNP = SelectedKhoanNghiPhep.Ma_KNP;

                    var updateKNP = DataProvider.Ins.DB.KhoanNghiPhep.Where(x => x.Ma_KNP == NghiPhepSua.Ma_KNP).SingleOrDefault();
                    updateKNP.SoNgayNghi_KNP -= (int)TongNgayNghi;

                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin Nghỉ Phép thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                LoadListNghiPhep();
                p.Close();
            });
            #endregion

            #region Xóa
            XoaCommand = new RelayCommand<Window>((p) =>
            {
                if (SelectedNghiPhep == null)
                {
                    MessageBox.Show("Vui lòng chọn Nghỉ Phép trước khi xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                return true;
            }, (p) =>
            {
                MessageBoxResult result = MessageBox.Show("Thao tác này không thể hoàn tác!", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var transaction = DataProvider.Ins.DB.Database.BeginTransaction())
                    {
                        try
                        {
                            var np = DataProvider.Ins.DB.NghiPhep.Where(x => x.Ma_NP == SelectedNghiPhep.Ma_NP).FirstOrDefault();
                            DataProvider.Ins.DB.NghiPhep.Remove(np);

                            //Khôi phục lại số ngày nghỉ của Khoản nghỉ phép đã lưu
                            var oldKNP = DataProvider.Ins.DB.KhoanNghiPhep.Where(x => x.Ma_KNP == np.Ma_KNP).SingleOrDefault();
                            double oldDays = (np.NgayKetThuc_NP.Value - np.NgayBatDau_NP.Value).TotalDays + 1;
                            oldKNP.SoNgayNghi_KNP += (int)oldDays;

                            DataProvider.Ins.DB.SaveChanges();
                            transaction.Commit();
                            if (MessageBox.Show("Xóa Tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
                            {
                                ResetControls();
                                p.Close();
                            }

                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Xóa không thành công!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            transaction.Rollback();
                        }
                        LoadListNghiPhep();
                    }
                }
            });
            #endregion
            #endregion
        }
        #region Các hàm hổ trợ
        void LoadListNghiPhep()
        {
            if (MainViewModel.TaiKhoan.Quyen_TK == "Trưởng các bộ phận khác")
            {
                ListNghiPhep = new ObservableCollection<NghiPhep>(DataProvider.Ins.DB.NghiPhep.Where(x => x.NhanVien.Ma_PB == MainViewModel.TaiKhoan.NhanVien.Ma_PB && x.NhanVien.TrangThai_NV == true));
            }
            else
            {
                ListNghiPhep = new ObservableCollection<NghiPhep>(DataProvider.Ins.DB.NghiPhep.Where(x => x.NhanVien.TrangThai_NV == true));
            }
        }

        void ResetControls()
        {
            NgayBatDau = null;
            NgayKetThuc = null;
            LiDo = null;
            ListKhoanNghiPhep = null;
            SelectedNhanVien = null;
            SelectedNghiPhep = null;
            SelectedKhoanNghiPhep = null;
        }

        void LoadListNhanVien()
        {
            ListNhanVien = new ObservableCollection<NhanVien>();

            var query = from nv in DataProvider.Ins.DB.NhanVien.Where(x => x.TrangThai_NV == true)
                        where
                            (from knp in DataProvider.Ins.DB.KhoanNghiPhep
                             where
                                 nv.Ma_NV == knp.Ma_NV
                             select
                                 knp).FirstOrDefault() != null
                        select nv;
            if (MainViewModel.TaiKhoan.Quyen_TK == "Trưởng bộ phận khác")
            {
                foreach (NhanVien item in query)
                {
                    if (MainViewModel.TaiKhoan.NhanVien.Ma_PB == item.Ma_PB)
                    {
                        ListNhanVien.Add(item);
                    }
                }
                return;
            }

            foreach (NhanVien item in query)
            {
                ListNhanVien.Add(item);
            }
        }

       void LoadListKhoanNghiPhep(int id)
        {
            ListKhoanNghiPhep = new ObservableCollection<KhoanNghiPhep>();

            var listKNP = DataProvider.Ins.DB.KhoanNghiPhep.Where(x => x.Ma_NV == id);
            foreach (KhoanNghiPhep item in listKNP)
            {
                ListKhoanNghiPhep.Add(item);
            }
        }

        void HieenThi()
        {
            NgayBatDau = SelectedNghiPhep.NgayBatDau_NP;
            NgayKetThuc = SelectedNghiPhep.NgayKetThuc_NP;
            LiDo = SelectedNghiPhep.LiDo_NP;
            SelectedNhanVien = DataProvider.Ins.DB.NhanVien.Where(x=>x.Ma_NV == SelectedNhanVien.Ma_NV && x.TrangThai_NV == true).SingleOrDefault();
            LoadListKhoanNghiPhep(SelectedNhanVien.Ma_NV);
            SelectedKhoanNghiPhep = DataProvider.Ins.DB.KhoanNghiPhep.Where(x => x.Ma_KNP == SelectedNghiPhep.Ma_KNP).SingleOrDefault();
        } //Hiện thị thồng tin lên Main Window với dạng command HienThiCommand
        #endregion `
    }
}
