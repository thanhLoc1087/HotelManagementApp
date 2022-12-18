using HotelManagementApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
    }
}
