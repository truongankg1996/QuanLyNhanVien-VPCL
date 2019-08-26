using QuanLyNhanVien.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuanLyNhanVien.UserControlAn
{
    /// <summary>
    /// Interaction logic for ControlBarUC.xaml
    /// </summary>
    public partial class ControlBarUC : UserControl
    {
        private ControlBarViewModel _controlBarViewModel;
        public ControlBarViewModel ControlBarViewModel { get => _controlBarViewModel; set => _controlBarViewModel = value; }

        public ControlBarUC()
        {
            InitializeComponent();
            this.DataContext = ControlBarViewModel = new ControlBarViewModel();
        }
    }
}
