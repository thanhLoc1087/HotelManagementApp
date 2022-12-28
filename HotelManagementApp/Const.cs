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
        static private bool _isAdmin;
        static public bool IsAdmin { get => _isAdmin; set { _isAdmin = value; OnStaticPropertyChanged(); } }
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
        static public string _statErrorMsg = "Không có dữ liệu trong giai đoạn này.";
        static public string statErrorMsg
        {
            get => _statErrorMsg;
            set { _statErrorMsg = value; OnStaticPropertyChanged("statErrorMsg"); }
        }
        static public string _hotelName = "Hotel";
        static public string hotelName
        {
            get => _hotelName;
            set { _hotelName = value; OnStaticPropertyChanged("hotelName"); }
        }
        static public string _hotelMoto = "One of the hotels";
        static public string hotelMoto
        {
            get => _hotelMoto;
            set { _hotelMoto = value; OnStaticPropertyChanged("hotelMoto"); }
        }
        static public string _hotelAddress = "123 St. Street, Ward 12, District 10,   Ho Chi Minh City";
        static public string hotelAddress
        {
            get => _hotelAddress;
            set
            {
                _hotelAddress = value; OnStaticPropertyChanged("hotelAddress");
            }
        }
        static public string _hotelPhone = "0987654321";
        static public string hotelPhone
        {
            get => _hotelPhone;
            set { _hotelPhone = value; OnStaticPropertyChanged("hotelPhone"); }
        }
        static public string _hotelMail = "contact@hotel.com";
        static public string hotelMail
        {
            get => _hotelMail;
            set { _hotelMail = value; OnStaticPropertyChanged("hotelMail"); }
        }
        
    }
}
