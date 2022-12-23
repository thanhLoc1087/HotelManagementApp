using HotelManagementApp.Model;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace HotelManagementApp
{
    public class Const : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void OnStaticPropertyChanged([CallerMemberName] string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
        public static Account ActiveAccount;
        static public Visibility _adminVisibility = Visibility.Visible;
        static public Visibility AdminVisibility
        {
            get => _adminVisibility;
            set
            {
                if (_adminVisibility != value)
                {
                    _adminVisibility = value;
                    OnStaticPropertyChanged("AdminVisibility");
                }
            }
        }
        static public Visibility _staffVisibility = Visibility.Collapsed;
        static public Visibility StaffVisibility
        {
            get => _staffVisibility;
            set
            {
                if (_staffVisibility != value)
                {
                    _staffVisibility = value;
                    OnStaticPropertyChanged("StaffVisibility");
                }
            }
        }
        static public string _loginMsg = "Sai tài khoản hoặc mật khẩu!";
        static public string loginMsg
        {
            get => _loginMsg;
            set { _loginMsg = value; OnStaticPropertyChanged("loginMsg"); }
        }
    }
}
