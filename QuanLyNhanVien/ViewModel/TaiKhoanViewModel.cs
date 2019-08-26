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
    public class TaiKhoanViewModel:BaseViewModel
    {
            #region DataContext
            private ObservableCollection<TaiKhoan> _ListTaiKhoan;
            public ObservableCollection<TaiKhoan> ListTaiKhoan { get => _ListTaiKhoan; set { _ListTaiKhoan = value; OnPropertyChanged(); } }
            private ObservableCollection<string> _ListTruongPhong;
            public ObservableCollection<string> ListTruongPhong { get => _ListTruongPhong; set { _ListTruongPhong = value; OnPropertyChanged(); } }
            #endregion

            #region Combobox item source
            private ObservableCollection<NhanVien> _ListNhanVien;
            public ObservableCollection<NhanVien> ListNhanVien { get => _ListNhanVien; set { _ListNhanVien = value; OnPropertyChanged(); } }
            private ObservableCollection<string> _ListQuyenHan;
            public ObservableCollection<string> ListQuyenHan { get => _ListQuyenHan; set { _ListQuyenHan = value; OnPropertyChanged(); } }
            #endregion

            #region Thuộc tính binding
            private string _TenTaiKhoan;
            public string TenTaiKhoan { get => _TenTaiKhoan; set { _TenTaiKhoan = value; OnPropertyChanged(); } }
            private string _MatKhau;
            public string MatKhau { get => _MatKhau; set { _MatKhau = value; OnPropertyChanged(); } }
            private string _MatKhauMaHoa;
            private string _NhapLaiMatKhau;
            public string NhapLaiMatKhau { get => _NhapLaiMatKhau; set { _NhapLaiMatKhau = value; OnPropertyChanged(); } }
            private string _NhapLaiMatKhauMaHoa;
            private string _QuyenHan;
            public string QuyenHan { get => _QuyenHan; set { _QuyenHan = value; OnPropertyChanged(); } }
            private NhanVien _SelectedNhanVien;
            public NhanVien SelectedNhanVien { get => _SelectedNhanVien; set { _SelectedNhanVien = value; OnPropertyChanged(); } }
            private string _SelectedQuyenHan;
            public string SelectedQuyenHan { get => _SelectedQuyenHan; set { _SelectedQuyenHan = value; OnPropertyChanged(); } }
            private TaiKhoan _SelectedTaiKhoan;
            public TaiKhoan SelectedTaiKhoan { get => _SelectedTaiKhoan; set { _SelectedTaiKhoan = value; OnPropertyChanged(); } }
            #endregion

            #region Thuộc tính khác
            private bool _IsEditable;
            public bool IsEditable { get => _IsEditable; set { _IsEditable = value; OnPropertyChanged(); } }
            private string _SearchTaiKhoan;
            public string SearchTaiKhoan { get => _SearchTaiKhoan; set { _SearchTaiKhoan = value; OnPropertyChanged(); } }

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
            public ICommand PasswordChangedCommand { get; set; }
            public ICommand RepeatPasswordChangedCommand { get; set; }
            #endregion

            #region Thuộc tính ẩn hiện các Tab
            public enum ChucNangCaiDat
            {
                TaiKhoan, NgayNghiLe, ThongTinPhanMem
            };
            private int _ChucNangCD;
            public int ChucNangCD { get => _ChucNangCD; set { _ChucNangCD = value; OnPropertyChanged(); } }
            public ICommand TabTaiKhoanCommand { get; set; }
            public ICommand TabNgayNghiLeCommand { get; set; }
            public ICommand TabThongTinPhanMemCommand { get; set; }
            #endregion
            public TaiKhoanViewModel()
            {
                #region Xử lý ẩn hiện Tab
                TabTaiKhoanCommand = new RelayCommand<Object>((p) =>
                {
                    return true;
                }, (p) =>
                {
                    ChucNangCD = (int)ChucNangCaiDat.TaiKhoan;
                });

                TabNgayNghiLeCommand = new RelayCommand<Object>((p) =>
                {
                    return true;
                }, (p) =>
                {
                    ChucNangCD = (int)ChucNangCaiDat.NgayNghiLe;
                });

                TabThongTinPhanMemCommand = new RelayCommand<Object>((p) =>
                {
                    return true;
                }, (p) =>
                {
                    ChucNangCD = (int)ChucNangCaiDat.ThongTinPhanMem;
                });
                #endregion

                #region Khởi tạo
                LoadListTaiKhoan();
                string[] DSQuyenHan = new string[] { "Trưởng bộ phận Hành chính-Nhân sự", "Quản trị hệ thống", "Trưởng các bộ phận khác", "Nhân viên hành chính nhân sự" };
                ListQuyenHan = new ObservableCollection<string>(DSQuyenHan);
                LoadListNhanVien();
                IsEditable = false;
                #endregion

                #region Command

                #region Tạo mới
                // Tạo mới => Button Tạo mới ở Main Window => Phòng Ban
                TaoMoiCommand = new RelayCommand<Object>((p) =>
                {
                    LoadListNhanVien();
                    if (ListNhanVien.Count == 0)
                    {
                        MessageBox.Show("Hiện tại chưa có nhân viên chưa có tài khoản.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        return false;
                    }
                    return true;
                }, (p) =>
                {
                    IsEditable = true;
                    SelectedTaiKhoan = null;
                    ResetControls();
                    LoadListNhanVien();

                    TaiKhoanWindow window = new TaiKhoanWindow();
                    window.ShowDialog();
                });
                #endregion

                #region Hiện Thị
                //Hiện thị
                HienThiCommand = new RelayCommand<Object>((p) =>
                {
                    if (SelectedTaiKhoan == null)
                    {
                        return false;
                    }
                    return true;
                }, (p) =>
                {
                    IsEditable = false;
                    LoadListNhanVien();
                    TenTaiKhoan = SelectedTaiKhoan.TenDangNhap_TK;
                    SelectedQuyenHan = SelectedTaiKhoan.Quyen_TK;
                    SelectedNhanVien = SelectedTaiKhoan.NhanVien;
                    ListNhanVien.Add(SelectedTaiKhoan.NhanVien);

                    TaiKhoanWindow window = new TaiKhoanWindow();
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

                #region MaHoaMatKhau
                //Mã háo mật khẩu
                PasswordChangedCommand = new RelayCommand<PasswordBox>((p) =>
                {
                    return p== null ? false : true;
                }, (p) =>
                {
                    _MatKhauMaHoa = LoginViewModel.MD5Hash(LoginViewModel.Base64Encode(p.Password));
                });

                // Mã hóa nhập lại mật khẩu
                RepeatPasswordChangedCommand = new RelayCommand<PasswordBox>((p) =>
                {
                    return p == null ? false : true;
                }, (p) =>
                {
                    _NhapLaiMatKhauMaHoa = LoginViewModel.MD5Hash(LoginViewModel.Base64Encode(p.Password));
                });
                #endregion

                #region Luu
                //Luu
                LuuCommand = new RelayCommand<Window>((p) =>
                {
                    if (string.IsNullOrEmpty(TenTaiKhoan) || SelectedNhanVien == null || SelectedQuyenHan == null)
                    {
                        MessageBox.Show("Vui lòng nhập hết thông tin tài khoản", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    if (_MatKhauMaHoa != _NhapLaiMatKhauMaHoa)
                    {
                        MessageBox.Show("Nhập lại mật khẩu và mật khẩu chưa được nhập hay nhập không trùng khớp!", "Thông báo!", MessageBoxButton.OK, MessageBoxImage.Information);
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

                    if (SelectedTaiKhoan == null)
                    {
                        if ( KiemTraTenDangNhap(TenTaiKhoan) == false)
                        {
                            MessageBox.Show("Tên đăng nhập đã tồn tại, vui lòng nhập tên đăng nhập khác!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            return ;
                        }

                        var TaiKhoanMoi = new TaiKhoan()
                        {
                            TenDangNhap_TK = TenTaiKhoan,
                            MatKhau_TK = _MatKhauMaHoa,
                            Quyen_TK = SelectedQuyenHan,
                            Ma_NV = SelectedNhanVien.Ma_NV,

                        };

                        DataProvider.Ins.DB.TaiKhoan.Add(TaiKhoanMoi);
                        DataProvider.Ins.DB.SaveChanges();
                        ListTaiKhoan.Add(TaiKhoanMoi);
                        MessageBox.Show("Thêm Tài Khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                   
                        var TaiKhoanSua = DataProvider.Ins.DB.TaiKhoan.Where(x => x.Ma_TK == SelectedTaiKhoan.Ma_TK).SingleOrDefault();

                        if (KiemTraTenDangNhap(TaiKhoanSua) == false)
                        {
                            MessageBox.Show("Tên đăng nhập đã tồn tại, vui lòng nhập tên đăng nhập khác!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }

                        if (_MatKhauMaHoa == null)
                        {
                            TaiKhoanSua.TenDangNhap_TK = TenTaiKhoan;
                            TaiKhoanSua.Quyen_TK = SelectedQuyenHan;
                            TaiKhoanSua.Ma_NV = SelectedNhanVien.Ma_NV;
                        }
                        else
                        {
                            TaiKhoanSua.TenDangNhap_TK = TenTaiKhoan;
                            TaiKhoanSua.MatKhau_TK = _MatKhauMaHoa;
                            TaiKhoanSua.Quyen_TK = SelectedQuyenHan;
                            TaiKhoanSua.Ma_NV = SelectedNhanVien.Ma_NV;
                        }                       
                   
                        DataProvider.Ins.DB.SaveChanges();
                        MessageBox.Show("Cập nhật thông tin Nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    LoadListTaiKhoan();
                    p.Close();
                });
                #endregion

                #region Sửa
                SuaCommand = new RelayCommand<Object>((p) =>
                {
                    return true;
                }, (p) =>
                {
                    if (SelectedTaiKhoan == null)
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
                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListTaiKhoan);
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
                    if (string.IsNullOrEmpty(SearchTaiKhoan))
                    {
                        CollectionViewSource.GetDefaultView(ListTaiKhoan).Filter = (all) => { return true; };
                    }
                    else
                    {
                        CollectionViewSource.GetDefaultView(ListTaiKhoan).Filter = (searchTaiKhoan) =>
                        {
                            return (searchTaiKhoan as TaiKhoan).TenDangNhap_TK.IndexOf(SearchTaiKhoan, StringComparison.OrdinalIgnoreCase) >= 0;
                        };
                    }
                });
                #endregion

                #region Xóa Command
                XoaCommand = new RelayCommand<Window>((p) =>
                {
                    if (SelectedTaiKhoan == null)
                    {
                        MessageBox.Show("Vui lòng chọn tài khoản trước khi xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }

                    if (MainViewModel.TaiKhoan == SelectedTaiKhoan)
                    {
                        MessageBox.Show("Tài khoản đang sử dụng không thể xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                                var tk = DataProvider.Ins.DB.TaiKhoan.Where(x => x.Ma_TK == SelectedTaiKhoan.Ma_TK).FirstOrDefault();
                                DataProvider.Ins.DB.TaiKhoan.Remove(tk);
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
                            LoadListTaiKhoan();
                        }
                    }
                });
                #endregion
                #endregion
            }
            #region Các hàm hổ trợ
            void LoadListTaiKhoan()
            {
                ListTaiKhoan = new ObservableCollection<TaiKhoan>(DataProvider.Ins.DB.TaiKhoan);  
            }

            void ResetControls()
            {
                TenTaiKhoan = null;
                MatKhau = null;
                NhapLaiMatKhau = null;
                QuyenHan = null;
                SelectedNhanVien = null;
                SelectedQuyenHan = null;
            }

            void LoadListNhanVien()
            {
                ListNhanVien = new ObservableCollection<NhanVien>();

                var query = from nv in DataProvider.Ins.DB.NhanVien
                            where
                                (from tk in DataProvider.Ins.DB.TaiKhoan
                                where
                                    nv.Ma_NV == tk.Ma_NV
                                select
                                    tk).FirstOrDefault() == null
                            select nv;
                foreach (NhanVien item in query)
                {
                    if (item.TrangThai_NV == true)
                    {
                        ListNhanVien.Add(item);
                    }
                }
            }

            bool KiemTraTenDangNhap(string tdn)
            {
                var taiKhoan = DataProvider.Ins.DB.TaiKhoan.Where(x => x.TenDangNhap_TK == tdn).SingleOrDefault();

                if (taiKhoan != null)
                {
                    return false;
                }
                return true;
            }

            bool KiemTraTenDangNhap(TaiKhoan tk)
            {
                if (tk.TenDangNhap_TK == TenTaiKhoan)
                {
                    return true;
                }

                var taiKhoan = DataProvider.Ins.DB.TaiKhoan.Where(x => x.TenDangNhap_TK == tk.TenDangNhap_TK).SingleOrDefault()
    ;
                if (taiKhoan != null)
                {
                    return false;
                }
                return true;
            }
            #endregion 
        }
}
