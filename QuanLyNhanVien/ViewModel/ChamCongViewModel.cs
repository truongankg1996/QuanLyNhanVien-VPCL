using QuanLyNhanVien.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuanLyNhanVien.ViewModel
{
    public class ChamCongViewModel:BaseViewModel
    {
        #region DataContext
        private ObservableCollection<ThongTinChamCong> _ListTTChamCong_ALLNV;
        public ObservableCollection<ThongTinChamCong> ListTTChamCong_ALLNV { get => _ListTTChamCong_ALLNV; set { _ListTTChamCong_ALLNV = value; OnPropertyChanged(); } }
        private ObservableCollection<ThongTinChamCong> _ListTTChamCong_1NV;
        public ObservableCollection<ThongTinChamCong> ListTTChamCong_1NV { get => _ListTTChamCong_1NV; set { _ListTTChamCong_1NV = value; OnPropertyChanged(); } }
        private ObservableCollection<ChamCongNgay> _ListChamCong_1NV;
        public ObservableCollection<ChamCongNgay> ListChamCong_1NV { get => _ListChamCong_1NV; set { _ListChamCong_1NV = value; OnPropertyChanged(); } }
        #endregion

        #region Combobox item source
        private ObservableCollection<NhanVien> _ListNhanVien;
        public ObservableCollection<NhanVien> ListNhanVien { get => _ListNhanVien; set { _ListNhanVien = value; OnPropertyChanged(); } }
        private ObservableCollection<int> _ListNam;
        public ObservableCollection<int> ListNam { get => _ListNam; set { _ListNam = value; OnPropertyChanged(); } }
        private ObservableCollection<int> _ListThang;
        public ObservableCollection<int> ListThang { get => _ListThang; set { _ListThang = value; OnPropertyChanged(); } }
        #endregion

        #region Thuộc tính binding
        private int _SelectedNam;
        public int SelectedNam { get => _SelectedNam; set { _SelectedNam = value; OnPropertyChanged(); } }
        private int _SelectedThang;
        public int SelectedThang { get => _SelectedThang; set { _SelectedThang = value; OnPropertyChanged(); } }
        private NhanVien _SelectedNhanVien;
        public NhanVien SelectedNhanVien { get => _SelectedNhanVien; set { _SelectedNhanVien = value; OnPropertyChanged(); } }
        private DateTime? _NgayChamCong;
        public DateTime? NgayChamCong { get => _NgayChamCong; set { _NgayChamCong = value; OnPropertyChanged(); } }
        private bool _IsEditable;
        public bool IsEditable { get => _IsEditable; set { _IsEditable = value; OnPropertyChanged(); } }
        #endregion

        #region Thuộc tính khác
        private string _SearchChamCong;
        public string SearchChamCong { get => _SearchChamCong; set { _SearchChamCong = value; OnPropertyChanged(); } }
        private ObservableCollection<ThongTinNhanVien> BackupTTChamCong_1NV;
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
        public ICommand LoadDataCCCommand { get; set; }
        public ICommand ChangeThangNamCommand { get; set; }
        #endregion
        public ChamCongViewModel()
        {
            #region Khởi tạo
            NgayChamCong = DateTime.Now;
            int[] DSNam = new int[] { 2019, 2020, 2021, 2022, 2023 };
            ListNam = new ObservableCollection<int>(DSNam);
            int[] DSThang = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            ListThang = new ObservableCollection<int>(DSThang);
            IsEditable = false;
            #endregion

            #region Command
            #region Load Data Chấm Công
            LoadDataCCCommand = new RelayCommand<Object>((p) =>
            {
                return true;
            }, (p) =>
            {
                LoadListNhanVien();
            });
            #endregion

            #region Thay đổi ngày tháng năm
            ChangeThangNamCommand = new RelayCommand<Object>((p) =>
            {
                return true;
            }, (p) =>
            {
                LoadListTTChamCong_1NV();
            });
            #endregion
            #region Tạo mới
            // Tạo mới => Button Tạo mới ở Main Window => Phòng Ban
            TaoMoiCommand = new RelayCommand<Object>((p) =>
            {
                return true;
            }, (p) =>
            {
                IsEditable = true;
                ResetControls();

                PhongBanWindow phongBanWindow = new PhongBanWindow();
                phongBanWindow.ShowDialog();
                MainWindow mainWindow = new MainWindow();
                mainWindow.Hide();
            });
            #endregion

            #region Hiện Thị
            //Hiện thị
            HienThiCommand = new RelayCommand<Object>((p) =>
            {
                return SelectedPhongBan == null ? false : true;
            }, (p) =>
            {
                ListNhanVienPhongBan = new ObservableCollection<NhanVien>(DataProvider.Ins.DB.NhanVien.Where(x => x.Ma_PB == SelectedPhongBan.Ma_PB));
                IsEditable = false;

                TenPhongBan = SelectedPhongBan.Ten_PB;
                NgayThanhLap = SelectedPhongBan.NgayThanhLap_PB;
                DiaChi = SelectedPhongBan.DiaChi_PB;
                SelectedTruongPhong = DataProvider.Ins.DB.NhanVien.Where(x => x.Ma_NV == SelectedPhongBan.MaTruongPhong_PB).SingleOrDefault();

                PhongBanWindow phongBanWindow = new PhongBanWindow();
                phongBanWindow.ShowDialog();
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
                    p.Close();
                }
            });
            #endregion

            #region Luu
            //
            LuuCommand = new RelayCommand<Window>((p) =>
            {
                if (string.IsNullOrEmpty(TenPhongBan) || string.IsNullOrEmpty(DiaChi) || NgayThanhLap == null)
                {
                    MessageBox.Show("Vui lòng nhập hết thông tin phòng ban!, Mã Trưởng Phòng có thể để trống ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                if (SelectedPhongBan == null)
                {
                    var PhongBanMoi = new PhongBan()
                    {
                        Ten_PB = TenPhongBan,
                        DiaChi_PB = DiaChi,
                        NgayThanhLap_PB = NgayThanhLap,

                    };

                    DataProvider.Ins.DB.PhongBan.Add(PhongBanMoi);
                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Thêm Nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ListNhanVienPhongBan = new ObservableCollection<NhanVien>(DataProvider.Ins.DB.NhanVien.Where(x => x.Ma_PB == SelectedPhongBan.Ma_PB));
                    var PhongBanSua = DataProvider.Ins.DB.PhongBan.Where(x => x.Ma_PB == SelectedPhongBan.Ma_PB).SingleOrDefault();

                    PhongBanSua.Ten_PB = TenPhongBan;
                    PhongBanSua.DiaChi_PB = DiaChi;
                    PhongBanSua.NgayThanhLap_PB = NgayThanhLap;
                    PhongBanSua.Ma_PB = SelectedPhongBan.Ma_PB;

                    if (SelectedTruongPhong != null)
                    {
                        PhongBanSua.MaTruongPhong_PB = SelectedTruongPhong.Ma_NV;
                    }

                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin Nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                LoadListPhongBan();
                p.Close();
            });
            #endregion

            #region Sửa
            SuaCommand = new RelayCommand<Object>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (SelectedPhongBan == null)
                {
                    MessageBox.Show("Vui lòng chọn Phòng ban để chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    IsEditable = false;
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
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListPhongBan);
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
                if (string.IsNullOrEmpty(SearchPhongBan))
                {
                    CollectionViewSource.GetDefaultView(ListPhongBan).Filter = (all) => { return true; };
                }
                else
                {
                    CollectionViewSource.GetDefaultView(ListPhongBan).Filter = (searchPhongBan) =>
                    {
                        return (searchPhongBan as PhongBan).Ten_PB.IndexOf(SearchPhongBan, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (searchPhongBan as PhongBan).DiaChi_PB.IndexOf(SearchPhongBan, StringComparison.OrdinalIgnoreCase) >= 0;
                    };
                }
            });
            #endregion

            #region Xóa Command
            XoaCommand = new RelayCommand<Window>((p) =>
            {
                if (SelectedPhongBan == null)
                {
                    MessageBox.Show("Vui lòng chọn nhân viên trước khi xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                            int soNhanVienPhongBan = DataProvider.Ins.DB.NhanVien.Where(x => x.Ma_PB == SelectedPhongBan.Ma_PB).Count();
                            if (soNhanVienPhongBan > 0)
                            {
                                MessageBox.Show("Xóa không thành công! Tồn tại Nhân viên thuộc Phòng ban này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                var pb = DataProvider.Ins.DB.PhongBan.Where(x => x.Ma_PB == SelectedPhongBan.Ma_PB).FirstOrDefault();
                                DataProvider.Ins.DB.PhongBan.Remove(pb);
                                DataProvider.Ins.DB.SaveChanges();
                                transaction.Commit();
                                if (MessageBox.Show("Xóa Phòng ban thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
                                {
                                    p.Close();
                                }
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Xóa không thành công!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            transaction.Rollback();
                        }
                        LoadListPhongBan();
                    }
                }
            });
            #endregion
            #endregion
        }
        #region Các hàm hổ trợ
        public void LoadListNhanVien()
        {
            if (MainViewModel.TaiKhoan.Quyen_TK == "Trưởng các bộ phận khác")
            {
                ListNhanVien = new ObservableCollection<NhanVien>(DataProvider.Ins.DB.NhanVien.Where(x => x.TrangThai_NV == true && MainViewModel.TaiKhoan.NhanVien.Ma_PB == x.Ma_PB));
            }
            else
            {
                ListNhanVien = new ObservableCollection<NhanVien>(DataProvider.Ins.DB.NhanVien.Where(x => x.TrangThai_NV == true));
            }
        }

        public void LoadListTTChamCong_1NV()
        {
            ListChamCong_1NV = new ObservableCollection<ChamCongNgay>();
            ListTTChamCong_1NV = new ObservableCollection<ThongTinChamCong>();
            BackupTTChamCong_1NV = new ObservableCollection<ThongTinNhanVien>();

            var listChamCong_1NV = from ccn in DataProvider.Ins.DB.ChamCongNgay
                                   where ccn.Ma_NV == SelectedNhanVien.Ma_NV &&
                                   ccn.ThoiGianBatDau_CCN.Value.Year == SelectedNam &&
                                   ccn.ThoiGianBatDau_CCN.Value.Month == SelectedThang
                                   select ccn;
            //Tạo Ngày dầu tiên cho tháng để khi cuối tháng ta chỉ cần cộng 1 tháng và trừ 1 ngày
            DateTime firstDayofMonth = new DateTime(SelectedNam, SelectedThang, 1);
            if (listChamCong_1NV.Count() <1)
            {
                //Duyệt từng ngày
                for (var i = 1; i <= firstDayofMonth.AddMonths(1).AddDays(-1).Day; i++)
                {
                    ListTTChamCong_1NV.Add(new ThongTinChamCong()
                    {
                        NhanVien = SelectedNhanVien,
                        HanhChinh = false,
                        TangCa = false,
                        NgayChamCong = new DateTime(SelectedNam, SelectedThang, i)
                    });
                }
                return;
            }

            foreach (ChamCongNgay item in listChamCong_1NV)
            {
                ListChamCong_1NV.Add(item);
            }

        }
        public void ResetControls()
        {
            SelectedPhongBan = null;
            TenPhongBan = null;
            SelectedTruongPhong = null;
            DiaChi = null;
            NgayThanhLap = null;
            ListNhanVienPhongBan = null;
        }
        #endregion 
    }
}
