using HotelManagementApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManagementApp.ViewModel
{
    
    public class MainViewModel : BaseViewModel
    {      
        public bool IsLoaded = false;

        public ICommand LoadedWindowCommand { get; set; }
        //Load View
        public ICommand ShowSingleBedroomWindowCommand { get; set; }
        public MainViewModel()
        {
            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                IsLoaded = true;
                LoginWindow loginWindow =  new LoginWindow();
                loginWindow.ShowDialog();
            }
            );

            ShowSingleBedroomWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                SingleBedroomWindow singleBedroomWindow = new SingleBedroomWindow();
                singleBedroomWindow.ShowDialog();
            }
            );
        }
    }
}
