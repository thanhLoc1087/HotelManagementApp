using HotelManagementApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HotelManagementApp.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public bool IsLoaded = false;
        public MainViewModel()
        {
            if (IsLoaded)
            {
                IsLoaded = true;
                LoginView loginView = new LoginView();
                loginView.ShowDialog();
            }
        }
    }
}
