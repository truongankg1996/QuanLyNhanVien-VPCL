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
    public class TuyenDungViewModel:BaseViewModel
    {
        #region Ứng Viên ViewModel
        #region DataContext
        private ObservableCollection<UngVien> _ListUngVien;
        public ObservableCollection<UngVien> ListUngVien { get => _ListUngVien; set { _ListUngVien = value; OnPropertyChanged(); } }
        #endregion

        #region Combobox item source
        private ObservableCollection<string> _ListGioiTinh;
        public ObservableCollection<string> ListGioiTinh { get => _ListGioiTinh; set { _ListGioiTinh = value; OnPropertyChanged(); } }
        #endregion

        #region Thuộc tính binding
        private string _HoTen;
        public string HoTen { get => _HoTen; set { _HoTen = value; OnPropertyChanged(); } }
        private DateTime? _NgaySinh;
        public DateTime? NgaySinh { get => _NgaySinh; set { _NgaySinh = value; OnPropertyChanged(); } }
        private string _Email;
        public string Email { get => _Email; set { _Email = value; OnPropertyChanged(); } }
        private string _SDT;
        public string SDT { get => _SDT; set { _SDT = value; OnPropertyChanged(); } }
        private string _DiaChi;
        public string DiaChi { get => _DiaChi; set { _DiaChi = value; OnPropertyChanged(); } }
        private string _SelectedGioiTinh;
        public string SelectedGioiTinh { get => _SelectedGioiTinh; set { _SelectedGioiTinh = value; OnPropertyChanged(); } }
        private UngVien _SelectedUngVien;
        public UngVien SelectedUngVien { get => _SelectedUngVien; set { _SelectedUngVien = value; OnPropertyChanged(); } }
        #endregion

        #region Thuộc tính khác
        private bool _IsEditable;
        public bool IsEditable { get => _IsEditable; set { _IsEditable = value; OnPropertyChanged(); } }
        private string _SearchUngVien;
        public string SearchUngVien { get => _SearchUngVien; set { _SearchUngVien = value; OnPropertyChanged(); } }
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

        public TuyenDungViewModel()
        {
            #region Khởi tạo
            LoadListUngVien();
            string[] DSGioiTinh = new string[] { "Nam", "Nữ" };
            ListGioiTinh = new ObservableCollection<string>(DSGioiTinh);
            string[] DSTrangThai = new string[] { "Chưa Xử Lý", "Chấp Nhận", "Từ Chối" };

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

                LoaiNghiPhepWindow window = new LoaiNghiPhepWindow();
                window.ShowDialog();
            });
            #endregion

            #region Hiện Thị
            //Hiện thị
            HienThiCommand = new RelayCommand<Object>((p) =>
            {
                if (SelectedLoaiNghiPhep == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                IsEditable = false;

                TenLoaiNghiPhep = SelectedLoaiNghiPhep.Ten_LNP;
                SelectedCoLuong = SelectedLoaiNghiPhep.CoLuong_LNP == true ? "Có" : "Không";

                LoaiNghiPhepWindow window = new LoaiNghiPhepWindow();
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
                if (SelectedLoaiNghiPhep == null)
                {
                    MessageBox.Show("Vui lòng chọn loại nghỉ phép để chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListLoaiNghiPhep);
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
                if (string.IsNullOrEmpty(SearchLoaiNghiPhep))
                {
                    CollectionViewSource.GetDefaultView(ListLoaiNghiPhep).Filter = (all) => { return true; };
                }
                else
                {
                    CollectionViewSource.GetDefaultView(ListLoaiNghiPhep).Filter = (search) =>
                    {
                        return (search as LoaiNghiPhep).Ten_LNP.IndexOf(SearchLoaiNghiPhep, StringComparison.OrdinalIgnoreCase) >= 0;
                    };
                }
            });
            #endregion

            #region Luu
            //Luu
            LuuCommand = new RelayCommand<Window>((p) =>
            {
                if (TenLoaiNghiPhep == null || SelectedCoLuong == null)
                {
                    MessageBox.Show("Vui lòng nhập hết thông tin loại nghỉ phép", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                if (SelectedLoaiNghiPhep == null)
                {
                    var LoaiNghiPhepMoi = new LoaiNghiPhep()
                    {
                        Ten_LNP = TenLoaiNghiPhep,
                        CoLuong_LNP = SelectedCoLuong == "Có" ? true : false
                    };

                    DataProvider.Ins.DB.LoaiNghiPhep.Add(LoaiNghiPhepMoi);

                    DataProvider.Ins.DB.SaveChanges();
                    ListLoaiNghiPhep.Add(LoaiNghiPhepMoi);
                    MessageBox.Show("Thêm loại nghỉ Phếp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {

                    var LoaiNghiPhepSua = DataProvider.Ins.DB.LoaiNghiPhep.Where(x => x.Ma_LNP == SelectedLoaiNghiPhep.Ma_LNP).SingleOrDefault();

                    LoaiNghiPhepSua.Ten_LNP = TenLoaiNghiPhep;
                    LoaiNghiPhepSua.CoLuong_LNP = SelectedCoLuong == "Có" ? true : false;

                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin Nghỉ Phép thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                LoadListLoaiNghiPhep();
                p.Close();
            });
            #endregion

            #region Xóa
            XoaCommand = new RelayCommand<Window>((p) =>
            {
                if (SelectedLoaiNghiPhep == null)
                {
                    MessageBox.Show("Vui lòng chọn Loại Nghỉ Phép trước khi xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                            var lnp = DataProvider.Ins.DB.LoaiNghiPhep.Where(x => x.Ma_LNP == SelectedLoaiNghiPhep.Ma_LNP).FirstOrDefault();
                            DataProvider.Ins.DB.LoaiNghiPhep.Remove(lnp);

                            DataProvider.Ins.DB.SaveChanges();
                            transaction.Commit();
                            if (MessageBox.Show("Xóa Tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
                            {
                                ResetControls();
                                p.Close();
                            }

                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Xóa không thành công!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            transaction.Rollback();
                        }
                        LoadListLoaiNghiPhep();
                    }
                }
            });
            #endregion
            #endregion
        }
        #region Các hàm hổ trợ
        void LoadListUngVien()
        {
            ListUngVien = new ObservableCollection<UngVien>(DataProvider.Ins.DB.UngVien);
        }

        void ResetControls()
        {
            TenLoaiNghiPhep = null;
            SelectedLoaiNghiPhep = null;
            SelectedCoLuong = null;
        }
        #endregion
        #endregion

        #region Hồ Sơ Ứng Tuyển
        #region DataContext
        private ObservableCollection<HoSoUngTuyen> _ListHoSoUngTuyen;
        public ObservableCollection<HoSoUngTuyen> ListHoSoUngTuyen { get => _ListHoSoUngTuyen; set { _ListHoSoUngTuyen = value; OnPropertyChanged(); } }
        #endregion

        #region Combobox item source
        private ObservableCollection<string> _ListTrangThai_HSUT;
        public ObservableCollection<string> ListTrangThai_HSUT { get => _ListTrangThai_HSUT; set { _ListTrangThai_HSUT = value; OnPropertyChanged(); } }
        #endregion

        #region Thuộc tính binding
        private string _ViTriCongViec;
        public string ViTriCongViec { get => _ViTriCongViec; set { _ViTriCongViec = value; OnPropertyChanged(); } }
        private DateTime? _NgayNop;
        public DateTime? NgayNop { get => _NgayNop; set { _NgayNop = value; OnPropertyChanged(); } }
        private string _TenFileCV;
        public string TenFileCV { get => _TenFileCV; set { _TenFileCV = value; OnPropertyChanged(); } }
        private byte[] _CV_HoSoUngTuyen;
        public byte[] CV_HoSoUngTuyen { get => _CV_HoSoUngTuyen; set { _CV_HoSoUngTuyen = value; OnPropertyChanged(); } }
        private string _SelectedTrangThai;
        public string SelectedTrangThai { get => _SelectedTrangThai; set { _SelectedTrangThai = value; OnPropertyChanged(); } }
        private HoSoUngTuyen _SelectedHoSoUngTuyen;
        public HoSoUngTuyen SelectedHoSoUngTuyen { get => _SelectedHoSoUngTuyen; set { _SelectedHoSoUngTuyen = value; OnPropertyChanged(); } }
        #endregion

        #region Thuộc tính khác
        private bool _IsEditable_HSUT;
        public bool IsEditable_HSUT { get => _IsEditable_HSUT; set { _IsEditable = value; OnPropertyChanged(); } }
        private string _SearchLoaiNghiPhep;
        public string SearchLoaiNghiPhep { get => _SearchLoaiNghiPhep; set { _SearchLoaiNghiPhep = value; OnPropertyChanged(); } }
        public bool sort_HSUT;
        #endregion

        #region Command
        public ICommand TaoMoi_HSUTCommand { get; set; }
        public ICommand HienThi_HSUTCommand { get; set; }
        public ICommand Huy_HSUTCommand { get; set; }
        public ICommand Luu_HSUTCommand { get; set; }
        public ICommand Sua_HSUTCommand { get; set; }
        public ICommand Xoa_HSUTCommand { get; set; }
        public ICommand Sort_HSUTCommand { get; set; } // 2 Command này đc MainWindow biding để xử lý phần tim kiếm
        public ICommand Search_HSUTCommand { get; set; }
        public ICommand ChonFile_HSUTCommand { get; set; }
        public ICommand XemFile_HSUTCommand { get; set; }
        #endregion

        public LoaiNghiPhepViewModel()
        {
            #region Khởi tạo
            LoadListLoaiNghiPhep();
            string[] CoLuongArray = new string[] { "Có", "Không" };
            ListCoLuong = new ObservableCollection<string>(CoLuongArray);
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

                LoaiNghiPhepWindow window = new LoaiNghiPhepWindow();
                window.ShowDialog();
            });
            #endregion

            #region Hiện Thị
            //Hiện thị
            HienThiCommand = new RelayCommand<Object>((p) =>
            {
                if (SelectedLoaiNghiPhep == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                IsEditable = false;

                TenLoaiNghiPhep = SelectedLoaiNghiPhep.Ten_LNP;
                SelectedCoLuong = SelectedLoaiNghiPhep.CoLuong_LNP == true ? "Có" : "Không";

                LoaiNghiPhepWindow window = new LoaiNghiPhepWindow();
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
                if (SelectedLoaiNghiPhep == null)
                {
                    MessageBox.Show("Vui lòng chọn loại nghỉ phép để chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListLoaiNghiPhep);
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
                if (string.IsNullOrEmpty(SearchLoaiNghiPhep))
                {
                    CollectionViewSource.GetDefaultView(ListLoaiNghiPhep).Filter = (all) => { return true; };
                }
                else
                {
                    CollectionViewSource.GetDefaultView(ListLoaiNghiPhep).Filter = (search) =>
                    {
                        return (search as LoaiNghiPhep).Ten_LNP.IndexOf(SearchLoaiNghiPhep, StringComparison.OrdinalIgnoreCase) >= 0;
                    };
                }
            });
            #endregion

            #region Luu
            //Luu
            LuuCommand = new RelayCommand<Window>((p) =>
            {
                if (TenLoaiNghiPhep == null || SelectedCoLuong == null)
                {
                    MessageBox.Show("Vui lòng nhập hết thông tin loại nghỉ phép", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                if (SelectedLoaiNghiPhep == null)
                {
                    var LoaiNghiPhepMoi = new LoaiNghiPhep()
                    {
                        Ten_LNP = TenLoaiNghiPhep,
                        CoLuong_LNP = SelectedCoLuong == "Có" ? true : false
                    };

                    DataProvider.Ins.DB.LoaiNghiPhep.Add(LoaiNghiPhepMoi);

                    DataProvider.Ins.DB.SaveChanges();
                    ListLoaiNghiPhep.Add(LoaiNghiPhepMoi);
                    MessageBox.Show("Thêm loại nghỉ Phếp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {

                    var LoaiNghiPhepSua = DataProvider.Ins.DB.LoaiNghiPhep.Where(x => x.Ma_LNP == SelectedLoaiNghiPhep.Ma_LNP).SingleOrDefault();

                    LoaiNghiPhepSua.Ten_LNP = TenLoaiNghiPhep;
                    LoaiNghiPhepSua.CoLuong_LNP = SelectedCoLuong == "Có" ? true : false;

                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin Nghỉ Phép thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                LoadListLoaiNghiPhep();
                p.Close();
            });
            #endregion

            #region Xóa
            XoaCommand = new RelayCommand<Window>((p) =>
            {
                if (SelectedLoaiNghiPhep == null)
                {
                    MessageBox.Show("Vui lòng chọn Loại Nghỉ Phép trước khi xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                            var lnp = DataProvider.Ins.DB.LoaiNghiPhep.Where(x => x.Ma_LNP == SelectedLoaiNghiPhep.Ma_LNP).FirstOrDefault();
                            DataProvider.Ins.DB.LoaiNghiPhep.Remove(lnp);

                            DataProvider.Ins.DB.SaveChanges();
                            transaction.Commit();
                            if (MessageBox.Show("Xóa Tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
                            {
                                ResetControls();
                                p.Close();
                            }

                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Xóa không thành công!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            transaction.Rollback();
                        }
                        LoadListLoaiNghiPhep();
                    }
                }
            });
            #endregion
            #endregion
        }
        #region Các hàm hổ trợ
        void LoadListLoaiNghiPhep()
        {
            ListLoaiNghiPhep = new ObservableCollection<LoaiNghiPhep>(DataProvider.Ins.DB.LoaiNghiPhep);
        }

        void ResetControls()
        {
            TenLoaiNghiPhep = null;
            SelectedLoaiNghiPhep = null;
            SelectedCoLuong = null;
        }
        #endregion
        #endregion
    }
}
