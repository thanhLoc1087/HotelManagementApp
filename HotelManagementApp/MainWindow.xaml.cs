using HotelManagementApp.Model;
using HotelManagementApp.ViewModel;
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

namespace HotelManagementApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = Tab.SelectedIndex;
            switch (index)
            {
                case 0:
                    this.DataContext = new MainViewModel();
                    break;
                case 1:
                    this.DataContext = new OrderViewModel();
                    break;
                case 2:
                    this.DataContext = new CheckOutViewModel();
                    break;
                case 3:
                    this.DataContext = new BookingViewModel();
                    break;
                case 4:
                    this.DataContext = new BillsViewModel();
                    break;
                case 5:
                    this.DataContext = new FoodViewModel();
                    break;
                case 6:
                    this.DataContext = new StaffViewModel();
                    break;
                case 7:
                    this.DataContext = new AddRoomForAdminViewModel();
                    break;
                case 8:
                    this.DataContext = new AddRoomTypes();
                    break;
                case 9:
                    this.DataContext = new CustomersViewModel();
                    break;
                case 10:
                    this.DataContext = new SettingsViewModel();
                    break;
            }

        }
    }
}
