using HotelManagementApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HotelManagementApp.ViewModel
{
    public class SettingsViewModel : BaseViewModel
    {
        public ICommand LogoutCommand { get; set; }

        public SettingsViewModel()
        {
            LogoutCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            );
        }
    }
}
