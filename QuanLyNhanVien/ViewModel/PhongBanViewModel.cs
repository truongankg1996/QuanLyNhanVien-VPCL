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
    public class PhongBanViewModel:BaseViewModel
    {
        #region DataContext
        private ObservableCollection<PhongBan> _ListPhongBan;
        public ObservableCollection<PhongBan> ListPhongBan { get => _ListPhongBan; set { _ListPhongBan = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _ListTruongPhong;
        public ObservableCollection<string> ListTruongPhong { get => _ListTruongPhong; set { _ListTruongPhong = value; OnPropertyChanged(); } }
        #endregion

        #region Combobox item source
        private ObservableCollection<NhanVien> _ListNhanVienPhongBan;
        public ObservableCollection<NhanVien> ListNhanVienPhongBan { get => _ListNhanVienPhongBan; set { _ListNhanVienPhongBan = value; OnPropertyChanged(); } }
        #endregion

        #region Thuộc tính binding
        private string _TenPhongBan;
        public string TenPhongBan { get => _TenPhongBan; set { _TenPhongBan = value; OnPropertyChanged(); } }
        private NhanVien _SelectedTruongPhong;
        public NhanVien SelectedTruongPhong { get => _SelectedTruongPhong; set { _SelectedTruongPhong = value; OnPropertyChanged(); } }
        private DateTime? _NgayThanhLap;
        public DateTime? NgayThanhLap { get => _NgayThanhLap; set { _NgayThanhLap = value; OnPropertyChanged(); } }
        private string _DiaChi;
        public string DiaChi { get => _DiaChi; set { _DiaChi = value; OnPropertyChanged(); } }
        private PhongBan _SelectedPhongBan;
        public PhongBan SelectedPhongBan { get => _SelectedPhongBan; set { _SelectedPhongBan = value; OnPropertyChanged(); } }
        private bool _IsEditable;
        public bool IsEditable { get => _IsEditable; set { _IsEditable = value; OnPropertyChanged(); } }
        #endregion

        #region Thuộc tính khác
        private string _SearchPhongBan;
        public string SearchPhongBan { get => _SearchPhongBan; set { _SearchPhongBan = value; OnPropertyChanged(); } }
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
        #endregion
        public PhongBanViewModel()
        {
            #region Khởi tạo
            LoadListPhongBan();

            IsEditable = false;
            #endregion

            #region Command

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
                return SelectedPhongBan == null ? false :true;
            }, (p) =>
            {
                ListNhanVienPhongBan = new ObservableCollection<NhanVien>(DataProvider.Ins.DB.NhanVien.Where(x=> x.Ma_PB == SelectedPhongBan.Ma_PB));
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
                return p == null ? false :true;
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
        public void LoadListPhongBan()
        {
            ListPhongBan = new ObservableCollection<PhongBan>(DataProvider.Ins.DB.PhongBan);
            ListTruongPhong = new ObservableCollection<string>();
            var listTruongPhong = from pb in DataProvider.Ins.DB.PhongBan
                                  join nv in DataProvider.Ins.DB.NhanVien
                                  on pb.MaTruongPhong_PB equals nv.Ma_NV
                                  where (nv.TrangThai_NV == true)
                                  select nv.HoTen_NV;
            foreach (string item in listTruongPhong)
            {
                ListTruongPhong.Add(item);
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
