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
    public class KhoanNghiPhepViewModel:BaseViewModel
    {
        #region DataContext
        private ObservableCollection<KhoanNghiPhep> _ListKhoanNghiPhep_MainWD;
        public ObservableCollection<KhoanNghiPhep> ListKhoanNghiPhep_MainWD { get => _ListKhoanNghiPhep_MainWD; set { _ListKhoanNghiPhep_MainWD = value; OnPropertyChanged(); } }
        private ObservableCollection<KhoanNghiPhep> _ListKhoanNghiPhep_KNPWD;
        public ObservableCollection<KhoanNghiPhep> ListKhoanNghiPhep_KNPWD { get => _ListKhoanNghiPhep_KNPWD; set { _ListKhoanNghiPhep_KNPWD = value; OnPropertyChanged(); } }
        private ObservableCollection<KhoanNghiPhep> _ListKhoanNghiPhepCapNhat;
        public ObservableCollection<KhoanNghiPhep> ListKhoanNghiPhepCapNhat { get => _ListKhoanNghiPhepCapNhat; set { _ListKhoanNghiPhepCapNhat = value; OnPropertyChanged(); } }
        #endregion

        #region Combobox item source
        private ObservableCollection<NhanVien> _ListNhanVien;
        public ObservableCollection<NhanVien> ListNhanVien { get => _ListNhanVien; set { _ListNhanVien = value; OnPropertyChanged(); } }
       
        #endregion

        #region Thuộc tính binding
        private NhanVien _SelectedNhanVien;
        public NhanVien SelectedNhanVien { get => _SelectedNhanVien; set { _SelectedNhanVien = value; OnPropertyChanged(); } }
        private KhoanNghiPhep _SelectedKhoanNghiPhep;
        public KhoanNghiPhep SelectedKhoanNghiPhep { get => _SelectedKhoanNghiPhep; set { _SelectedKhoanNghiPhep = value; OnPropertyChanged(); } }
        #endregion

        #region Thuộc tính khác
        private bool _IsEditable;
        public bool IsEditable { get => _IsEditable; set { _IsEditable = value; OnPropertyChanged(); } }
        private bool _IsSelectNV;
        public bool IsSelectNV { get => _IsSelectNV; set { _IsSelectNV = value; OnPropertyChanged(); } }
        private string _SearchKhoanNghiPhep;
        public string SearchKhoanNghiPhep { get => _SearchKhoanNghiPhep; set { _SearchKhoanNghiPhep = value; OnPropertyChanged(); } }
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
        public ICommand LoadDataKNPCommand { get; set; }
        #endregion
        public KhoanNghiPhepViewModel()
        {

            #region Command

            #region Load Dữ Liệu Khoản Nghỉ Phép

            LoadDataKNPCommand = new RelayCommand<Object>((p) =>
            {
                return true;
            }, (p) =>
            {
                LoadListKhoanNghiPhep_MainWD();
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
                IsSelectNV = true;
                SelectedKhoanNghiPhep = null;
                LoadListKhoanNghiPhep_KNPWD();
                LoadListNhanVien();

                KhoanNghiPhepWindow window = new KhoanNghiPhepWindow();
                window.ShowDialog();
            });
            #endregion

            #region Hiển Thị
            //Hiển Thị
            HienThiCommand = new RelayCommand<Object>((p) =>
            {
                if (SelectedKhoanNghiPhep == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                IsEditable = false;
                IsSelectNV = false;
                LoadListKhoanNghiPhep_KNPWD();
                LoadListNhanVien();
                SelectedNhanVien = SelectedKhoanNghiPhep.NhanVien;
               
                KhoanNghiPhepWindow window = new KhoanNghiPhepWindow();
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
                    IsSelectNV = false;
                    p.Close();
                }
            });
            #endregion

            #region Sửa
            SuaCommand = new RelayCommand<Object>((p) =>
            {
                if (SelectedKhoanNghiPhep == null)
                {
                    MessageBox.Show("Vui lòng chọn Ngày nghỉ phép để chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
                return true;
            }, (p) =>
            {
                IsEditable = true;
                IsSelectNV = false;
            });
            #endregion

            #region Sort
            SortCommand = new RelayCommand<GridViewColumnHeader>((p) =>
            {
                return p == null ? false : true;
            }, (p) =>
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListKhoanNghiPhep_MainWD);
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
                if (string.IsNullOrEmpty(SearchKhoanNghiPhep))
                {
                    CollectionViewSource.GetDefaultView(ListKhoanNghiPhep_MainWD).Filter = (all) => { return true; };
                }
                else
                {
                    CollectionViewSource.GetDefaultView(ListKhoanNghiPhep_MainWD).Filter = (search) =>
                    {
                        return (search as KhoanNghiPhep).NhanVien.HoTen_NV.IndexOf(SearchKhoanNghiPhep, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (search as KhoanNghiPhep).LoaiNghiPhep.Ten_LNP.IndexOf(SearchKhoanNghiPhep, StringComparison.OrdinalIgnoreCase) >= 0;
                    };
                }
            });
            #endregion

            #region Luu
            //Luu
            LuuCommand = new RelayCommand<Window>((p) =>
            {
                if (SelectedNhanVien == null)
                {
                    MessageBox.Show("Vui lòng chọn Nhân viên trước", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                return true;
            }, (p) =>
            {

                if (SelectedKhoanNghiPhep == null)
                {
                    foreach (KhoanNghiPhep item in ListKhoanNghiPhep_KNPWD)
                    {
                        var KhoanNghiPhepMoi = new KhoanNghiPhep()
                        {
                            Ma_NV = SelectedNhanVien.Ma_NV,
                            Ma_LNP = item.Ma_LNP,
                            SoNgayNghi_KNP = item.SoNgayNghi_KNP,
                        };

                        DataProvider.Ins.DB.KhoanNghiPhep.Add(KhoanNghiPhepMoi);
                        DataProvider.Ins.DB.SaveChanges();

                        ListKhoanNghiPhep_MainWD.Add(KhoanNghiPhepMoi);
                    }

                    

                    
                    MessageBox.Show("Thêm Khoản nghỉ phép thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {

                    var listKhoanNghiPhepSua = DataProvider.Ins.DB.KhoanNghiPhep.Where(x => x.Ma_NV == SelectedKhoanNghiPhep.Ma_NV);

                    foreach (KhoanNghiPhep knpCu in listKhoanNghiPhepSua)
                    {
                        foreach (KhoanNghiPhep knpMoi in ListKhoanNghiPhep_KNPWD)
                        {
                            if (knpCu.Ma_LNP == knpMoi.Ma_LNP)
                            {
                                knpCu.SoNgayNghi_KNP = knpMoi.SoNgayNghi_KNP;
                                break;
                            }
                        }
                    }

                    DataProvider.Ins.DB.SaveChanges();

                    if (ListKhoanNghiPhepCapNhat != null)
                    {
                        foreach (KhoanNghiPhep item in ListKhoanNghiPhepCapNhat)
                        {
                            var KhoanNghiPhepMoi = new KhoanNghiPhep()
                            {
                                Ma_NV = SelectedNhanVien.Ma_NV,
                                Ma_LNP = item.Ma_LNP,
                                SoNgayNghi_KNP = item.SoNgayNghi_KNP,
                            };

                            DataProvider.Ins.DB.KhoanNghiPhep.Add(KhoanNghiPhepMoi);
                            DataProvider.Ins.DB.SaveChanges();

                            ListKhoanNghiPhep_MainWD.Add(KhoanNghiPhepMoi);
                        }
                    }

                    MessageBox.Show("Cập nhật thông tin Khoản nghỉ Phép thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                LoadListKhoanNghiPhep_MainWD();
                p.Close();
            });
            #endregion

            #endregion
        }
        #region Các hàm hổ trợ
        void LoadListNhanVien()
        {
            if (SelectedKhoanNghiPhep == null)
            {
                ListNhanVien = new ObservableCollection<NhanVien>();

                var listNhanVien = from nv in DataProvider.Ins.DB.NhanVien.Where(x => x.TrangThai_NV == true)
                                   where (from knp in DataProvider.Ins.DB.KhoanNghiPhep
                                          where nv.Ma_NV == knp.Ma_NV
                                          select knp).FirstOrDefault() == null
                                   select nv;

                if (MainViewModel.TaiKhoan.Quyen_TK == "Trưởng các bộ phận khác")
                {
                    foreach (NhanVien item in listNhanVien)
                    {
                        ListNhanVien.Add(item);
                    }
                }
                else
                {
                    if (MainViewModel.TaiKhoan.Quyen_TK == "Trưởng các bộ phận khác")
                    {
                        ListNhanVien = new ObservableCollection<NhanVien>(DataProvider.Ins.DB.NhanVien.Where(x => x.Ma_PB == MainViewModel.TaiKhoan.NhanVien.Ma_PB && x.TrangThai_NV == true));
                        return;
                    }
                    ListNhanVien = new ObservableCollection<NhanVien>(DataProvider.Ins.DB.NhanVien.Where(x => x.TrangThai_NV == true));
                }
            }
        }
        void LoadListKhoanNghiPhep_KNPWD()
        {
            ListKhoanNghiPhepCapNhat = null;
            ListKhoanNghiPhep_KNPWD = new ObservableCollection<KhoanNghiPhep>();

            if (SelectedKhoanNghiPhep == null )
            {
                var listLoaiNghiPhep = from lnp in DataProvider.Ins.DB.LoaiNghiPhep
                                       select lnp;

                foreach (LoaiNghiPhep item in listLoaiNghiPhep)
                {
                    ListKhoanNghiPhep_KNPWD.Add(new KhoanNghiPhep()
                    {
                        LoaiNghiPhep = item,
                        Ma_LNP = item.Ma_LNP,
                        SoNgayNghi_KNP = 0
                    });
                }
            }
            else
            {
                var listLoaiNghiPhepNV = from knp in DataProvider.Ins.DB.KhoanNghiPhep
                                         where knp.Ma_NV == SelectedKhoanNghiPhep.Ma_NV
                                         select knp;

                if (MainViewModel.TaiKhoan.Quyen_TK == "Trưởng các bộ phận khác")
                {
                    foreach (KhoanNghiPhep item in listLoaiNghiPhepNV)
                    {
                        if (MainViewModel.TaiKhoan.NhanVien.Ma_PB == item.NhanVien.Ma_PB)
                        {
                            ListKhoanNghiPhep_KNPWD.Add(item);
                        }
                    }
                }
                else
                {
                    foreach (KhoanNghiPhep item in listLoaiNghiPhepNV)
                    {
                        ListKhoanNghiPhep_KNPWD.Add(item);
                    }
                }

                //Xử lý việc có thêm loại nghỉ phép cho nhân viên hay không khi đã có khoan nghỉ phép
                var listLoaiNghiPhep = from lnp in DataProvider.Ins.DB.LoaiNghiPhep
                                       where (from lnpNV in listLoaiNghiPhepNV
                                              where lnp.Ma_LNP == lnpNV.Ma_LNP
                                              select lnp).FirstOrDefault() == null
                                       select lnp;

                if (listLoaiNghiPhep.Count() >0)
                {
                    ListKhoanNghiPhepCapNhat = new ObservableCollection<KhoanNghiPhep>();

                    foreach (LoaiNghiPhep item in listLoaiNghiPhep)
                    {
                        KhoanNghiPhep knp = new KhoanNghiPhep()
                        {
                            LoaiNghiPhep = item,
                            Ma_LNP = item.Ma_LNP,
                            SoNgayNghi_KNP = 0
                        };
                        ListKhoanNghiPhep_KNPWD.Add(knp);
                        ListKhoanNghiPhepCapNhat.Add(knp);
                    }
                }
            }
        }

        void LoadListKhoanNghiPhep_MainWD()
        {
            if (MainViewModel.TaiKhoan.Quyen_TK == "Trưởng các bộ phận khác")
            {
                ListKhoanNghiPhep_MainWD = new ObservableCollection<KhoanNghiPhep>(DataProvider.Ins.DB.KhoanNghiPhep.Where(x => x.NhanVien.Ma_PB == MainViewModel.TaiKhoan.NhanVien.Ma_PB && x.NhanVien.TrangThai_NV == true));
            }
            else
            {
                ListKhoanNghiPhep_MainWD = new ObservableCollection<KhoanNghiPhep>(DataProvider.Ins.DB.KhoanNghiPhep.Where(x => x.NhanVien.TrangThai_NV == true));
            }

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListKhoanNghiPhep_MainWD);
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(new PropertyGroupDescription("NhanVien.HoTen_NV"));
        }
        #endregion 
    }
}
