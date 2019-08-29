using QuanLyNhanVien.Model;
using QuanLyNhanVien.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace QuanLyNhanVien.ViewModel
{
    public class TuyenDungViewModel : BaseViewModel
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
            ListTrangThai_HSUT = new ObservableCollection<string>(DSTrangThai);
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
                SelectedUngVien = null;
                ResetControls();
                ReloadListHoSoUngTuyen();

                UngVienWIndow window = new UngVienWIndow();
                window.ShowDialog();
            });
            #endregion

            #region Hiển Thị
            //Hiển thị
            HienThiCommand = new RelayCommand<Object>((p) =>
            {
                if (SelectedUngVien == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                IsEditable = false;

                HoTen = SelectedUngVien.HoTen_UV;
                NgaySinh = SelectedUngVien.NgaySinh_UV;
                DiaChi = SelectedUngVien.DiaChi_UV;
                NgaySinh = SelectedUngVien.NgaySinh_UV;
                Email = SelectedUngVien.Email_UV;
                SDT = SelectedUngVien.SoDienThoai_UV;
                SelectedGioiTinh = SelectedUngVien.GioiTinh_UV == true ? "Nữ" : "Nam";

                ReloadListHoSoUngTuyen();
                UngVienWIndow window = new UngVienWIndow();
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
                if (SelectedUngVien == null)
                {
                    MessageBox.Show("Vui lòng chọn ứng viên để chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListUngVien);
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
                if (string.IsNullOrEmpty(SearchUngVien))
                {
                    CollectionViewSource.GetDefaultView(ListUngVien).Filter = (all) => { return true; };
                }
                else
                {
                    CollectionViewSource.GetDefaultView(ListUngVien).Filter = (search) =>
                    {
                        return (search as UngVien).HoTen_UV.IndexOf(SearchUngVien, StringComparison.OrdinalIgnoreCase) >= 0;
                    };
                }
            });
            #endregion

            #region Luu
            //Luu
            LuuCommand = new RelayCommand<Window>((p) =>
            {
                if (string.IsNullOrEmpty(HoTen) ||
                string.IsNullOrEmpty(DiaChi) ||
                string.IsNullOrEmpty(SDT) ||
                string.IsNullOrEmpty(Email) ||
                NgaySinh == null ||
                SelectedGioiTinh == null)
                {
                    MessageBox.Show("Vui lòng nhập hết thông tin ứng viên", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                if (SelectedUngVien == null)
                {
                    var Moi = new UngVien()
                    {
                        HoTen_UV = HoTen,
                        DiaChi_UV = DiaChi,
                        Email_UV = Email,
                        SoDienThoai_UV = SDT,
                        NgaySinh_UV = NgaySinh,
                        GioiTinh_UV = SelectedGioiTinh == "Nữ" ? true : false
                    };

                    DataProvider.Ins.DB.UngVien.Add(Moi);

                    DataProvider.Ins.DB.SaveChanges();
                    ListUngVien.Add(Moi);
                    MessageBox.Show("Thêm ứng viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {

                    var Sua = DataProvider.Ins.DB.UngVien.Where(x => x.Ma_UV == SelectedUngVien.Ma_UV).SingleOrDefault();

                    Sua.HoTen_UV = HoTen;
                    Sua.DiaChi_UV = DiaChi;
                    Sua.Email_UV = Email;
                    Sua.NgaySinh_UV = NgaySinh;
                    Sua.SoDienThoai_UV = SDT;
                    Sua.GioiTinh_UV = SelectedGioiTinh == "Nữ" ? true : false;

                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin Nghỉ Phép thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                LoadListUngVien();
                p.Close();
            });
            #endregion

            #region Xóa
            XoaCommand = new RelayCommand<Window>((p) =>
            {
                if (SelectedUngVien == null)
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
                            var Xoa = DataProvider.Ins.DB.UngVien.Where(x => x.Ma_UV == SelectedUngVien.Ma_UV).FirstOrDefault();
                            DataProvider.Ins.DB.UngVien.Remove(Xoa);

                            DataProvider.Ins.DB.SaveChanges();
                            transaction.Commit();
                            if (MessageBox.Show("Xóa Ứng Viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
                            {
                                ResetControls();
                                p.Close();
                            }

                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Xóa không thành công!" + e.ToString(), "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            transaction.Rollback();
                        }
                        LoadListUngVien();
                    }
                }
            });
            #endregion

            //----------------------------------------------//
            //Hồ Sơ Ứng Tuyển
            #region Tạo mới HSUT
            // Tạo mới => Button Tạo mới ở Main Window => Phòng Ban
            TaoMoi_HSUTCommand = new RelayCommand<Object>((p) =>
            {
                if (IsEditable == false)
                {
                    MessageBox.Show("Vui lòng bấm nút chỉnh sửa trước khi thêm mới hồ sơ ứng tuyển", "Thông Báo!", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
                return true;
            }, (p) =>
            {
                IsEditable_HSUT = true;
                SelectedHoSoUngTuyen = null;
                SelectedTrangThai = "Chưa xử lý";
                ResetContronl_HSUT();

                HoSoUngTuyenWindow window = new HoSoUngTuyenWindow();
                window.ShowDialog();
            });
            #endregion

            #region Hiển Thị HSUT
            //Hiển thị
            HienThi_HSUTCommand = new RelayCommand<Object>((p) =>
            {
                if (SelectedHoSoUngTuyen == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                IsEditable = false;

                ViTriCongViec = SelectedHoSoUngTuyen.ViTriCongViec_HSUT;
                NgayNop = SelectedHoSoUngTuyen.NgayNop_HSUT;
                CV_HoSoUngTuyen = SelectedHoSoUngTuyen.Cv_HSUT;
                SelectedTrangThai = SelectedHoSoUngTuyen.TrangThai_HSUT;
                TenFileCV = "cv.pdf";

                HoSoUngTuyenWindow window = new HoSoUngTuyenWindow();
                window.ShowDialog();
            });
            #endregion

            #region Hủy HSUT
            //Button Hủy
            Huy_HSUTCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {
                MessageBoxResult result = MessageBox.Show("Mọi thay đổi nếu có sẽ không được lưu, bạn có chắc chắn không?", "Thông báo", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    IsEditable_HSUT = false;
                    p.Close();
                }
            });
            #endregion

            #region Sửa  HSUT
            Sua_HSUTCommand = new RelayCommand<Object>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (SelectedHoSoUngTuyen == null)
                {
                    MessageBox.Show("Vui lòng chọn hồ sơ ứng tuyển để chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                IsEditable_HSUT = true;
            });
            #endregion

            #region Sort HSUT
            Sort_HSUTCommand = new RelayCommand<GridViewColumnHeader>((p) =>
            {
                return p == null ? false : true;
            }, (p) =>
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListHoSoUngTuyen);
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

            #region Luu HSUT
            //Luu
            Luu_HSUTCommand = new RelayCommand<Window>((p) =>
            {
                if (string.IsNullOrEmpty(ViTriCongViec) ||
                NgayNop == null ||
                CV_HoSoUngTuyen == null ||
                SelectedTrangThai == null)
                {
                    MessageBox.Show("Vui lòng nhập hết thông tin hồ sơ ứng tuyển", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (IsEditable_HSUT == false)
                {
                    MessageBox.Show("Vui lòng chỉnh sửa thông tin trước khi lưu!", "Thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                return true;
            }, (p) =>
            {

                if (SelectedHoSoUngTuyen == null) // Trường hợp thêm mới
                {
                    if (SelectedUngVien == null)
                    {
                        var Moi = new HoSoUngTuyen()
                        {
                            ViTriCongViec_HSUT = ViTriCongViec,
                            NgayNop_HSUT = NgayNop,
                            TrangThai_HSUT = SelectedTrangThai,
                            Cv_HSUT = CV_HoSoUngTuyen,
                            Ma_UV = -1,
                        };

                        ListHoSoUngTuyen.Add(Moi);

                        MessageBox.Show("Thêm hồ sơ ứng tuyển thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        var Moi = new HoSoUngTuyen()
                        {
                            ViTriCongViec_HSUT = ViTriCongViec,
                            NgayNop_HSUT = NgayNop,
                            TrangThai_HSUT = SelectedTrangThai,
                            Cv_HSUT = CV_HoSoUngTuyen,
                            Ma_UV = SelectedUngVien.Ma_UV,
                        };

                        ListHoSoUngTuyen.Add(Moi);
                        DataProvider.Ins.DB.HoSoUngTuyen.Add(Moi);

                        MessageBox.Show("Thêm hồ sơ ứng tuyển thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    p.Close();
                }
                else
                {
                    if (SelectedHoSoUngTuyen != null)
                    {
                        SelectedHoSoUngTuyen.ViTriCongViec_HSUT = ViTriCongViec;
                        SelectedHoSoUngTuyen.NgayNop_HSUT = NgayNop;
                        SelectedHoSoUngTuyen.TrangThai_HSUT = SelectedTrangThai;
                        SelectedHoSoUngTuyen.Cv_HSUT = CV_HoSoUngTuyen;

                        if (SelectedUngVien != null)
                        {
                            var Sua = DataProvider.Ins.DB.HoSoUngTuyen.Where(x => x.Ma_HSUT == SelectedHoSoUngTuyen.Ma_HSUT).SingleOrDefault();

                            Sua.ViTriCongViec_HSUT = ViTriCongViec;
                            Sua.NgayNop_HSUT = NgayNop;
                            Sua.TrangThai_HSUT = SelectedTrangThai;
                            Sua.Cv_HSUT = CV_HoSoUngTuyen;

                            DataProvider.Ins.DB.SaveChanges();
                            MessageBox.Show("Cập nhật thông tin hồ sơ ứng tuyển thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                LoadListHoSoUngTuyen();
                p.Close();
            });
            #endregion

            #region Xóa HSUT
            Xoa_HSUTCommand = new RelayCommand<Window>((p) =>
            {
                if (SelectedHoSoUngTuyen == null)
                {
                    MessageBox.Show("Vui lòng chọn hồ sơ ứng tuyển trước khi xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                return true;
            }, (p) =>
            {
                MessageBoxResult result = MessageBox.Show("Thao tác này không thể hoàn tác!", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (SelectedUngVien != null)
                    {
                        DataProvider.Ins.DB.HoSoUngTuyen.Remove(SelectedHoSoUngTuyen);

                        ListHoSoUngTuyen.Remove(SelectedHoSoUngTuyen);
                    }
                    else
                    {
                        ListHoSoUngTuyen.Remove(SelectedHoSoUngTuyen);
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
            HoTen = null;
            NgaySinh = null;
            Email = null;
            SDT = null;
            DiaChi = null;
            SelectedGioiTinh = null;
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
        public ICommand ChonFile_HSUTCommand { get; set; }
        public ICommand XemFile_HSUTCommand { get; set; }
        #endregion

        #region Các hàm hổ trợ
        // Xử lý lưu và đọc file
        public byte[] FileToBinaryString(string filePath)
        {
            try
            {
                byte[] binaryString;
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        binaryString = reader.ReadBytes((int)stream.Length);
                    }
                }
                return binaryString;
            }
            catch (IOException e)
            {
                MessageBox.Show("Đã xảy ra lỗi!" + e.ToString(), "Thông báo!", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public string BinaryStringToFile(byte[] binaryString)
        {
            if (binaryString != null)
            {
                var fs = new FileStream("../../ResurceXAML/TempFile/cv.pdf", FileMode.Create, FileAccess.Write);
                fs.Write(binaryString, 0, binaryString.Length);
                return fs.Name;
            }
            else
            {
                MessageBox.Show("Đã xảy ra lỗi!", "Thông báo!", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        //Các hàm Load
        void LoadListHoSoUngTuyen()
        {
            ListHoSoUngTuyen = new ObservableCollection<HoSoUngTuyen>(DataProvider.Ins.DB.HoSoUngTuyen);
        }

        void ReloadListHoSoUngTuyen()
        {
            if (SelectedUngVien == null)
            {
                ListHoSoUngTuyen = new ObservableCollection<HoSoUngTuyen>();
                return;
            }
            else
            {
                ListHoSoUngTuyen = new ObservableCollection<HoSoUngTuyen>(DataProvider.Ins.DB.HoSoUngTuyen.Where(x => x.Ma_UV == SelectedUngVien.Ma_UV));
            }
        }

        void ResetContronl_HSUT()
        {
            ViTriCongViec = null;
            NgayNop = null;
            CV_HoSoUngTuyen = null;
            TenFileCV = null;
        }

        void UnchangedAllActions()
        {
            // if (ListHoSoUngTuyen == null) return;
            var changedEntries = DataProvider.Ins.DB.ChangeTracker.Entries()
                .Where(x => x.State != System.Data.Entity.EntityState.Unchanged).ToList();

            foreach (var entry in changedEntries)
            {
                switch (entry.State)
                {
                    case System.Data.Entity.EntityState.Modified:
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = System.Data.Entity.EntityState.Unchanged;
                        break;
                    case System.Data.Entity.EntityState.Added:
                        entry.State = System.Data.Entity.EntityState.Detached;
                        break;
                    case System.Data.Entity.EntityState.Deleted:
                        entry.State = System.Data.Entity.EntityState.Unchanged;
                        break;
                }
            }
        }
       
        #endregion
        #endregion

    }
}
