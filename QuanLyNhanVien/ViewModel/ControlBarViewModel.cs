﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyNhanVien.ViewModel
{
    public class ControlBarViewModel : BaseViewModel
    {
        //private string _tieuDe;
        //public string TieuDe
        //{
        //    get
        //    {
        //        return _tieuDe;
        //    }
        //    set
        //    {
        //        _tieuDe = value;
        //        RaisePropertyChanged("TieuDe");
        //    }
        //}
        public ICommand CloseWindowCommand { get; set; }
        public ICommand MaximizedWindowCommand { get; set; }
        public ICommand MinimizedWindowCommand { get; set; }

        public ICommand MouseMoveWindow { get; set; }

        public ControlBarViewModel()
        {
            //TieuDe = "Chương Trình Quản Lý Bán Hàng Cà Phê";


            CloseWindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) =>
            {
                //FrameworkElement mainWindow = GetParentWindow(p);
                //var window = (mainWindow as Window);

                if (MessageBox.Show("Bạn thực sự muốn thoát chương trình?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
                //CheckClosingView checkClosing = new CheckClosingView();
                //checkClosing.ShowDialog();
            });

            MaximizedWindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement mainWindow = GetParentWindow(p);
                var window = (mainWindow as Window);
                if (window != null)
                {
                    if (window.WindowState != WindowState.Maximized)
                    {
                        window.WindowState = WindowState.Maximized;
                    }
                    else
                    {
                        window.WindowState = WindowState.Normal;
                    }
                }
            });

            MinimizedWindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement mainWindow = GetParentWindow(p);
                var window = (mainWindow as Window);
                if (window != null)
                {
                    if (window.WindowState != WindowState.Minimized)
                    {
                        window.WindowState = WindowState.Minimized;
                    }
                    else
                    {
                        window.WindowState = WindowState.Maximized;
                    }
                }
            });

            MouseMoveWindow = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) =>
            {
                FrameworkElement mainWindow = GetParentWindow(p);
                var window = (mainWindow as Window);
                if (window != null)
                {
                    window.DragMove();
                }
            });
        }

        private FrameworkElement GetParentWindow(UserControl p)
        {
            FrameworkElement parent = p;
            while (parent.Parent != null)
            {
                //Every while, parent increase one step for find MainWindow
                parent = parent.Parent as FrameworkElement;
            }
            return parent;
        }
    }
}
