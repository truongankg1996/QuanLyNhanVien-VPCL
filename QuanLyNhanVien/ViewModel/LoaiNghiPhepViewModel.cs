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
    public class LoaiNghiPhepViewModel:BaseViewModel
    {
        #region DataContext
        private ObservableCollection<LoaiNghiPhep> _ListLoaiNghiPhep;
        public ObservableCollection<LoaiNghiPhep> ListLoaiNghiPhep { get => _ListLoaiNghiPhep; set { _ListLoaiNghiPhep = value; OnPropertyChanged(); } }
        #endregion

        #region Combobox item source
        private ObservableCollection<string> _ListCoLuong;
        public ObservableCollection<string> ListCoLuong { get => _ListCoLuong; set { _ListCoLuong = value; OnPropertyChanged(); } }
        #endregion

        #region Thuộc tính binding
        private string _TenLoaiNghiPhep;
        public string TenLoaiNghiPhep { get => _TenLoaiNghiPhep; set { _TenLoaiNghiPhep = value; OnPropertyChanged(); } }
        private string _SelectedCoLuong;
        public string SelectedCoLuong { get => _SelectedCoLuong; set { _SelectedCoLuong = value; OnPropertyChanged(); } }
        private LoaiNghiPhep _SelectedLoaiNghiPhep;
        public LoaiNghiPhep SelectedLoaiNghiPhep { get => _SelectedLoaiNghiPhep; set { _SelectedLoaiNghiPhep = value; OnPropertyChanged(); } }
        #endregion

        #region Thuộc tính khác
        private bool _IsEditable;
        public bool IsEditable { get => _IsEditable; set { _IsEditable = value; OnPropertyChanged(); } }
        private string _SearchLoaiNghiPhep;
        public string SearchLoaiNghiPhep { get => _SearchLoaiNghiPhep; set { _SearchLoaiNghiPhep = value; OnPropertyChanged(); } }
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
                SelectedCoLuong = SelectedLoaiNghiPhep.CoLuong_LNP == true ? "Có":"Không";

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
                        CoLuong_LNP = SelectedCoLuong == "Có" ? true:false
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
                    LoaiNghiPhepSua.CoLuong_LNP = SelectedCoLuong == "Có" ? true:false ;

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
                        catch (Exception )
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
    }
}
