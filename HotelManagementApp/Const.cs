using HotelManagementApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            set {
                int count = DataProvider.Instance.DB.Consts.Where(x => x.Name == nameof(Const.loginMsg)).Count();
                if (count > 0)
                {
                    var name = DataProvider.Instance.DB.Consts.Single(x => x.Name == nameof(Const.loginMsg));
                    name.Value = value;
                    DataProvider.Instance.DB.SaveChanges();
                }
                _loginMsg = value; 
                OnStaticPropertyChanged("loginMsg"); 
            }
        }
        static public string _statErrorMsg = "Không có dữ liệu trong giai đoạn này.";
        static public string statErrorMsg
        {
            get => _statErrorMsg;
            set
            {
                int count = DataProvider.Instance.DB.Consts.Where(x => x.Name == nameof(Const.statErrorMsg)).Count();
                if (count > 0)
                {
                    var name = DataProvider.Instance.DB.Consts.Single(x => x.Name == nameof(Const.statErrorMsg));
                    name.Value = value;
                    DataProvider.Instance.DB.SaveChanges();
                }
                _statErrorMsg = value; 
                OnStaticPropertyChanged("statErrorMsg"); 
            }
        }
        static public string _hotelName = "Hotel Name";
        static public string hotelName
        {
            get => _hotelName;
            set {
                int count = DataProvider.Instance.DB.Consts.Where(x => x.Name == nameof(Const.hotelName)).Count();
                if (count > 0)
                {
                    var name = DataProvider.Instance.DB.Consts.Single(x => x.Name == nameof(Const.hotelName));
                    name.Value = value;
                    DataProvider.Instance.DB.SaveChanges();
                }
                _hotelName = value; 
                OnStaticPropertyChanged("hotelName");
            }
        }
        static public string _hotelMoto = "This is a slogan";
        static public string hotelMoto
        {
            get => _hotelMoto;
            set
            {
                int count = DataProvider.Instance.DB.Consts.Where(x => x.Name == nameof(Const.hotelMoto)).Count();
                if (count > 0)
                {
                    var name = DataProvider.Instance.DB.Consts.Single(x => x.Name == nameof(Const.hotelMoto));
                    name.Value = value;
                    DataProvider.Instance.DB.SaveChanges();
                }
                _hotelMoto = value; 
                OnStaticPropertyChanged("hotelMoto");
            }
        }
        static public string _hotelAddress = "123, This is, the hotel's, Address";
        static public string hotelAddress
        {
            get => _hotelAddress;
            set
            {
                int count = DataProvider.Instance.DB.Consts.Where(x => x.Name == nameof(Const.hotelAddress)).Count();
                if (count > 0)
                {
                    var name = DataProvider.Instance.DB.Consts.Single(x => x.Name == nameof(Const.hotelAddress));
                    name.Value = value;
                    DataProvider.Instance.DB.SaveChanges();
                }
                _hotelAddress = value; 
                OnStaticPropertyChanged("hotelAddress");
            }
        }
        static public string _hotelPhone = "0xxxxxxxxx";
        static public string hotelPhone
        {
            get => _hotelPhone;
            set
            {
                int count = DataProvider.Instance.DB.Consts.Where(x => x.Name == nameof(Const.hotelPhone)).Count();
                if (count > 0)
                {
                    var name = DataProvider.Instance.DB.Consts.Single(x => x.Name == nameof(Const.hotelPhone));
                    name.Value = value;
                    DataProvider.Instance.DB.SaveChanges();
                }
                _hotelPhone = value; 
                OnStaticPropertyChanged("hotelPhone"); 
            }
        }
        static public string _hotelMail = "contact@hotel.com";
        static public string hotelMail
        {
            get => _hotelMail;
            set
            {
                int count = DataProvider.Instance.DB.Consts.Where(x => x.Name == nameof(Const.hotelMail)).Count();
                if (count > 0)
                {
                    var name = DataProvider.Instance.DB.Consts.Single(x => x.Name == nameof(Const.hotelMail));
                    name.Value = value;
                    DataProvider.Instance.DB.SaveChanges();
                }
                _hotelMail = value; 
                OnStaticPropertyChanged("hotelMail"); 
            }
        }
        
    }
}
