using Microsoft.Win32;
using QuanLyNhanVien.Model;
using QuanLyNhanVien.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QuanLyNhanVien.ViewModel
{
    public class NhanVienViewModel : BaseViewModel
    {
        #region DataContext
        private ObservableCollection<NhanVien> _ListNhanVien;
        public ObservableCollection<NhanVien> ListNhanVien { get => _ListNhanVien; set { _ListNhanVien = value; OnPropertyChanged(); } }
        private ObservableCollection<LichSuNhanVien> _ListLichSuNhanVien;
        public ObservableCollection<LichSuNhanVien> ListLichSuNhanVien { get => _ListLichSuNhanVien; set { _ListLichSuNhanVien = value; OnPropertyChanged(); } }
        private ObservableCollection<PhongBan> _ListPhongBan;
        public ObservableCollection<PhongBan> ListPhongBan { get => _ListPhongBan; set { _ListPhongBan = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _ListGioiTinh;
        public ObservableCollection<string> ListGioiTinh { get => _ListGioiTinh; set { _ListGioiTinh = value; OnPropertyChanged(); } }
        #endregion

        #region Thuộc tính Biding
        private string _HoTen;
        public string HoTen { get => _HoTen; set { _HoTen = value; OnPropertyChanged(); } }

        private PhongBan _SelectedPhongBan;
        public PhongBan SelectedPhongBan { get => _SelectedPhongBan; set { _SelectedPhongBan = value; OnPropertyChanged(); } }
        private string _SelectedGioiTinh;
        public string SelectedGioiTinh { get => _SelectedGioiTinh; set { _SelectedGioiTinh = value; OnPropertyChanged(); } }
        private DateTime? _NgaySinh;
        public DateTime? NgaySinh { get => _NgaySinh; set { _NgaySinh = value; OnPropertyChanged(); } }
        private string _ChucVu;
        public string ChucVu { get => _ChucVu; set { _ChucVu = value; OnPropertyChanged(); } }
        private DateTime? _NgayVaoLam;
        public DateTime? NgayVaoLam { get => _NgayVaoLam; set { _NgayVaoLam = value; OnPropertyChanged(); } }
        private string _Email;
        public string Email { get => _Email; set { _Email = value; OnPropertyChanged(); } }
        private string _SoDienThoai;
        public string SoDienThoai { get => _SoDienThoai; set { _SoDienThoai = value; OnPropertyChanged(); } }
        private string _DiaChi;
        public string DiaChi { get => _DiaChi; set { _DiaChi = value; OnPropertyChanged(); } }
        private string _Avatar;
        public string Avatar { get => _Avatar; set { _Avatar = value; OnPropertyChanged(); } }
        private ImageSource _AvatarSource;
        public ImageSource AvatarSource { get => _AvatarSource; set { _AvatarSource = value; OnPropertyChanged(); } }
        private NhanVien _SelectedNhanVien;
        public NhanVien SelectedNhanVien { get => _SelectedNhanVien; set { _SelectedNhanVien = value; OnPropertyChanged(); } }

        private bool _IsEditable;
        public bool IsEditable { get => _IsEditable; set { _IsEditable = value; OnPropertyChanged(); } }
        #endregion

        #region Binding command      
        public ICommand TaoMoiCommand { get; set; }
        public ICommand LuuCommand { get; set; }
        public ICommand HuyCommand { get; set; }
        public ICommand SuaCommand { get; set; }
        public ICommand HienThiCommand { get; set; }
        public ICommand SortCommand { get; set; }
        public ICommand SortLichSuNVCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand SearchLichSuNVCommand { get; set; }
        public ICommand ThayAnhCommand { get; set; }
        public ICommand XoaCommand { get; set; }
        #endregion

        #region Thuộc tính ẩn hiện tab
        public enum ChucNangNhanVien
        {
            ThongTinNhanVien, LichSuNhanVien
        };
        
        private int _ChucNangNV;
        public int ChucNangNV { get => _ChucNangNV; set { _ChucNangNV = value; OnPropertyChanged(); } }

        public ICommand TabThongTinNhanVienCommand { get; set; }
        public ICommand TabLichSuNhanVienCommand { get; set; }
        #endregion

        #region Thuộc tính khác
        private string _SearchNhanVien;
        public string SearchNhanVien { get => _SearchNhanVien; set { _SearchNhanVien = value; OnPropertyChanged(); } }
        private string _SearchLichSuNV;
        public string SearchLichSuNV { get => _SearchLichSuNV; set { _SearchLichSuNV = value; OnPropertyChanged(); } }
        public bool sort;
        #endregion

        #region Xử lý ảnh

        //Hàm hiển thị hình ảnh từ một string
        //public static BitmapImage GetImage(string imageSourceString)
        //{
        //    var img = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(imageSourceString)));
        //    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img);
        //    BitmapImage result = BitmapToImageSource(bmp);
        //    return result;
        //}

        //Hàm chuyển đổi image thành một string
        //public static string ImageToString(string imagePath)
        //{
        //    int width = 150;
        //    int height = 150;

        //    var source = Bitmap.FromFile(imagePath);
        //    var result = (Bitmap)ResizeImageKeepAspectRatio(source, width, height);
        //    result.Save("../../ResourceXAML/TempFiles/avatar.jpg");

        //    byte[] imageArray = System.IO.File.ReadAllBytes("../../ResourceXAML/TempFiles/avatar.jpg");
        //    return Convert.ToBase64String(imageArray);
        //}

        //Hàm chuyển đổi bitmap thành image
        //public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        //{
        //    using (MemoryStream memory = new MemoryStream())
        //    {
        //        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
        //        memory.Position = 0;
        //        BitmapImage bitmapimage = new BitmapImage();
        //        bitmapimage.BeginInit();
        //        bitmapimage.StreamSource = memory;
        //        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapimage.EndInit();

        //        return bitmapimage;
        //    }
        //}

        //Hàm resize ảnh mà vẫn giữ tỉ lệ gốc
        //public static System.Drawing.Image ResizeImageKeepAspectRatio(System.Drawing.Image source, int width, int height)
        //{
        //    System.Drawing.Image result = null;
        //    try
        //    {
        //        if (source.Width != width || source.Height != height)
        //        {
        //            Resize image
        //            float sourceRatio = (float)source.Width / source.Height;
        //            using (var target = new Bitmap(width, height))
        //            {
        //                using (var g = System.Drawing.Graphics.FromImage(target))
        //                {
        //                    g.CompositingQuality = CompositingQuality.HighQuality;
        //                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //                    g.SmoothingMode = SmoothingMode.HighQuality;

        //                    Scaling
        //                    float scaling;
        //                    float scalingY = (float)source.Height / height;
        //                    float scalingX = (float)source.Width / width;
        //                    if (scalingX < scalingY) scaling = scalingX; else scaling = scalingY;
        //                    int newWidth = (int)(source.Width / scaling);
        //                    int newHeight = (int)(source.Height / scaling);

        //                    Correct float to int rounding
        //                    if (newWidth < width) newWidth = width;
        //                    if (newHeight < height) newHeight = height;

        //                    See if image needs to be cropped
        //                    int shiftX = 0;
        //                    int shiftY = 0;
        //                    if (newWidth > width)
        //                    {
        //                        shiftX = (newWidth - width) / 2;
        //                    }

        //                    if (newHeight > height)
        //                    {
        //                        shiftY = (newHeight - height) / 2;
        //                    }

        //                    Draw image
        //                    g.DrawImage(source, -shiftX, -shiftY, newWidth, newHeight);
        //                }
        //                result = (System.Drawing.Image)target.Clone();
        //            }
        //        }
        //        else
        //        {
        //            Image size matched the given size
        //            result = (System.Drawing.Image)source.Clone();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        result = null;
        //    }

        //    return result;
        //}
        #endregion

        public NhanVienViewModel()
        {
            #region Khởi tạo
            LoadListNhanVien();
            string[] DSGioiTinh = new string[] { "Nam", "Nữ" };
            ListGioiTinh = new ObservableCollection<string>(DSGioiTinh);
            #endregion

            #region Xử lý ẩn hiện tab
            TabThongTinNhanVienCommand = new RelayCommand<Object>((p) =>
            {
                return true;
            }, (p) =>
            {
                ChucNangNV = (int)ChucNangNhanVien.ThongTinNhanVien;
            });

            TabLichSuNhanVienCommand = new RelayCommand<Object>((p) =>
            {
                return true;
            }, (p) =>
            {
                ChucNangNV = (int)ChucNangNhanVien.LichSuNhanVien;
            });
            #endregion

            #region Tạo mới command
            TaoMoiCommand = new RelayCommand<Object>((p) =>
            {
                var listPhongBan = DataProvider.Ins.DB.PhongBan.Count();
                if (listPhongBan > 0)
                {
                    return true;
                }
                MessageBox.Show("Vui lòng tạo phòng ban trước khi thêm nhân viên.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }, (p) =>
            {
                IsEditable = true;
                ResetControls();
                LoadListLichSuNhanVien();
                LoadListPhongBan();

                NhanVienWindow nhanVienWindow = new NhanVienWindow();
                nhanVienWindow.ShowDialog();
            });
            #endregion

            #region Hiển thị command
            HienThiCommand = new RelayCommand<Object>((p) =>
            {
                return SelectedNhanVien == null ? false : true;
            }, (p) =>
            {
                LoadListLichSuNhanVien();
                LoadListPhongBan();
                IsEditable = false;

                HoTen = SelectedNhanVien.HoTen_NV;
                SelectedPhongBan = SelectedNhanVien.PhongBan;
                SelectedGioiTinh = SelectedNhanVien.GioiTinh_NV == true ? "Nữ" : "Nam";
                NgaySinh = SelectedNhanVien.NgaySinh_NV;
                ChucVu = SelectedNhanVien.ChucVu_NV;
                NgayVaoLam = SelectedNhanVien.NgayVaoLam_NV;
                Email = SelectedNhanVien.Email_NV;
                SoDienThoai = SelectedNhanVien.SoDienThoai_NV;
                DiaChi = SelectedNhanVien.DiaChi_NV;
                Avatar = SelectedNhanVien.Avatar_NV;
                //AvatarSource = GetImage(Avatar);

                NhanVienWindow nhanVienWindow = new NhanVienWindow();
                nhanVienWindow.ShowDialog();
            });
            #endregion

            #region Hủy Command
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

            #region Sửa Command
            SuaCommand = new RelayCommand<Window>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (SelectedNhanVien == null)
                {
                    MessageBoxResult result = MessageBox.Show("Vui lòng chọn nhân viên trước khi chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                IsEditable = true;
            });
            #endregion

            #region Sort Command
            SortCommand = new RelayCommand<GridViewColumnHeader>((p) =>
            {
                return p == null ? false : true;
            }, (p) =>
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListNhanVien);    
                if (sort)
                {
                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription(p.Tag.ToString(), ListSortDirection.Descending));
                }
                sort = !sort;
            });
            #endregion

            #region Search Command
            SearchCommand = new RelayCommand<Object>((p) =>
            {
                return  true;
            }, (p) =>
            {
                if (string.IsNullOrEmpty(SearchNhanVien))
                {
                    CollectionViewSource.GetDefaultView(ListNhanVien).Filter = (all) => { return true; };
                }
                else
                {
                    CollectionViewSource.GetDefaultView(ListNhanVien).Filter = (searchNhanVien) =>
                    {
                        return (searchNhanVien as NhanVien).HoTen_NV.IndexOf(SearchNhanVien, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (searchNhanVien as NhanVien).ChucVu_NV.IndexOf(SearchNhanVien, StringComparison.OrdinalIgnoreCase) >= 0;
                    };
                }
            });
            #endregion

            #region Luu Command
            LuuCommand = new RelayCommand<Window>((p) =>
            {
                if (string.IsNullOrEmpty(HoTen) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(SoDienThoai) || 
                string.IsNullOrEmpty(DiaChi) || string.IsNullOrEmpty(ChucVu) || SelectedGioiTinh == null || SelectedPhongBan == null ||
                NgaySinh == null || NgayVaoLam == null)
                {
                    MessageBox.Show("Vui lòng nhập hết thông tin nhân viên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                if (SelectedNhanVien == null)
                {
                    var NhanVienMoi = new NhanVien()
                    {
                        HoTen_NV = HoTen,
                        Ma_PB = SelectedPhongBan.Ma_PB,
                        GioiTinh_NV = SelectedGioiTinh == "Nữ" ? true: false,
                        NgaySinh_NV = NgaySinh,
                        Email_NV = Email,
                        SoDienThoai_NV = SoDienThoai,
                        DiaChi_NV = DiaChi,
                        ChucVu_NV = ChucVu,
                        NgayVaoLam_NV = NgayVaoLam,
                        TrangThai_NV = true,
                        Avatar_NV = Avatar
                    };

                    DataProvider.Ins.DB.NhanVien.Add(NhanVienMoi);
                    DataProvider.Ins.DB.SaveChanges();
                    ListNhanVien.Add(NhanVienMoi);
                    MessageBox.Show("Thêm Nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);         
                }
                else
                {
                    LoadListLichSuNhanVien();
                    var NhanVienSua = DataProvider.Ins.DB.NhanVien.Where(x => x.Ma_NV == SelectedNhanVien.Ma_NV).SingleOrDefault();

                    AddLichSuNhanVien(NhanVienSua);

                    NhanVienSua.HoTen_NV = HoTen;
                    NhanVienSua.Ma_PB = SelectedPhongBan.Ma_PB;
                    NhanVienSua.GioiTinh_NV = SelectedGioiTinh == "Nữ" ? true : false;
                    NhanVienSua.NgaySinh_NV = NgaySinh;
                    NhanVienSua.Email_NV = Email;
                    NhanVienSua.SoDienThoai_NV = SoDienThoai;
                    NhanVienSua.DiaChi_NV = DiaChi;
                    NhanVienSua.ChucVu_NV = ChucVu;
                    NhanVienSua.NgayVaoLam_NV = NgayVaoLam;
                    NhanVienSua.TrangThai_NV = true;
                    NhanVienSua.Avatar_NV = Avatar;

                    DataProvider.Ins.DB.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin Nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                p.Close();
            });
            #endregion

            #region Xóa Command
            XoaCommand = new RelayCommand<Window>((p) =>
            {
                if (SelectedNhanVien == null)
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
                    var NhanVienSua = DataProvider.Ins.DB.NhanVien.Where(x => x.Ma_NV == SelectedNhanVien.Ma_NV).SingleOrDefault();
                    NhanVienSua.TrangThai_NV = false;
                    
                    DataProvider.Ins.DB.SaveChanges();
                    LoadListNhanVien();
                    p.Close();
                }
            });
            #endregion

            #region SortLichSuNV Command
            SortLichSuNVCommand = new RelayCommand<GridViewColumnHeader>((p) =>
            {
                return p==null ? false :true;
            }, (p) =>
            {
                if (SelectedNhanVien == null)
                {
                    return;
                }
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListLichSuNhanVien);
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

            #region SearchLichSuNV Command
            SearchLichSuNVCommand = new RelayCommand<Object>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (SelectedNhanVien == null)
                {
                    return;
                } 
                if (string.IsNullOrEmpty(SearchNhanVien))
                {
                    CollectionViewSource.GetDefaultView(ListLichSuNhanVien).Filter = (all) => { return true; };
                }
                else
                {
                    CollectionViewSource.GetDefaultView(ListLichSuNhanVien).Filter = (searchLichSuNV) => 
                    {
                        return (searchLichSuNV as LichSuNhanVien).MoTa_LSNV.IndexOf(SearchLichSuNV, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (searchLichSuNV as LichSuNhanVien).ThoiGian_LSNV.ToString().IndexOf(SearchLichSuNV, StringComparison.OrdinalIgnoreCase) >= 0;
                    };
                }
            });
            #endregion

            #region Thay anh Command
            //ThayAnhCommand = new RelayCommand<Object>((p) =>
            //{
            //    if (IsEditable == false)
            //    {
            //        MessageBox.Show("Vui lòng nhất nút chỉnh sửa trước khi thay đổi ảnh đại diện", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            //        return false;
            //    }
            //    return true;
            //}, (p) =>
            //{
            //    OpenFileDialog openFileDialog = new OpenFileDialog
            //    {
            //        InitialDirectory = @"C:\",
            //        Title = "Chọn ảnh đại diện",

            //        CheckFileExists = true,
            //        CheckPathExists = true,

            //        DefaultExt = "txt",
            //        Filter = "Images(*.JPG;*.PNG)| *.JPG;*.PNG",
            //        RestoreDirectory = true,
            //        ReadOnlyChecked = true,
            //        ShowReadOnly = true
            //    };

            //    if (openFileDialog.ShowDialog() == true)
            //    {
            //        Avatar = ImageToString(openFileDialog.FileName);
            //        AvatarSource = GetImage(Avatar);
            //    }
            //});
            #endregion
        }
        #region  Các hàm hỗ trợ
        public void LoadListNhanVien()
        {
            ListNhanVien = new ObservableCollection<NhanVien>(DataProvider.Ins.DB.NhanVien.Where(x => x.TrangThai_NV == true));
        }
        public void LoadListPhongBan()
        {
            ListPhongBan = new ObservableCollection<PhongBan>(DataProvider.Ins.DB.PhongBan);
        }
        public void LoadListLichSuNhanVien()
        {
            if (SearchNhanVien == null)
            {
                ListLichSuNhanVien = null;
            }
            else
            {
                ListLichSuNhanVien = new ObservableCollection<LichSuNhanVien>(DataProvider.Ins.DB.LichSuNhanVien.Where(x => x.Ma_NV == SelectedNhanVien.Ma_NV));
            }
        }
        
        public void AddLichSuNhanVien(NhanVien nv)
        {
            //Thêm lịch sử chức vụ
            string mota = "";
            if (nv.ChucVu_NV != ChucVu)
            {
                mota = "Thay đổi chức vụ từ" + nv.ChucVu_NV + "thành" + ChucVu;
            }

            if (mota != "")
            {
                var LichSuNhanVienMoi = new LichSuNhanVien()
                {
                    Ma_NV = nv.Ma_NV,
                    MoTa_LSNV = mota,
                    ThoiGian_LSNV = DateTime.Now
                };
                DataProvider.Ins.DB.LichSuNhanVien.Add(LichSuNhanVienMoi);
                DataProvider.Ins.DB.SaveChanges();
            }

            mota = "";
            //Thêm lịch sử Phòng ban
            if (nv.Ma_PB != SelectedPhongBan.Ma_PB)
            {
                mota = "Thay đổi phòng ban từ " + nv.PhongBan.Ten_PB + "thành" + SelectedPhongBan.Ten_PB;
            }

            if (mota != null)
            {
                var LichSuNhanVienMoi = new LichSuNhanVien()
                {
                    Ma_NV = nv.Ma_NV,
                    MoTa_LSNV = mota,
                    ThoiGian_LSNV = DateTime.Now
                };
                DataProvider.Ins.DB.LichSuNhanVien.Add(LichSuNhanVienMoi);
                DataProvider.Ins.DB.SaveChanges();
            }

        }
        public void ResetControls()
        {
            SelectedNhanVien = null;
            HoTen = null;
            SelectedPhongBan = null;
            SelectedGioiTinh = null;
            NgaySinh = null;
            ChucVu = null;
            NgayVaoLam = null;
            Email = null;
            SoDienThoai = null;
            DiaChi = null;
           // Avatar = ImageToString("../../ResourceXAML/Icons/default_user.png");
            //AvatarSource = GetImage(Avatar);
        }
        #endregion
    }
}
