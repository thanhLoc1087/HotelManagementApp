using HotelManagementApp.Model;
using HotelManagementApp.View;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace HotelManagementApp.ViewModel
{

    public class MainViewModel : BaseViewModel
    {      
        public bool IsLoaded = false;
        // Lists
        private ObservableCollection<BillDetail> _listBills;
        public ObservableCollection<BillDetail> ListBills { get => _listBills; set { _listBills = value; OnPropertyChanged(); } }
        private ObservableCollection<Room> _listRooms;
        public ObservableCollection<Room> ListRooms { get => _listRooms; set { _listRooms = value; OnPropertyChanged(); } }
        private ObservableCollection<RoomType> _listRoomTypes;
        public ObservableCollection<RoomType> ListRoomTypes { get => _listRoomTypes; set { _listRoomTypes = value; OnPropertyChanged(); } }
        private ObservableCollection<FoodsAndService> _listFnSs;
        public ObservableCollection<FoodsAndService> ListFnSs { get => _listFnSs; set { _listFnSs = value; OnPropertyChanged(); } }
        private ObservableCollection<Order> _listOrders;
        public ObservableCollection<Order> ListOrder{ get => _listOrders; set { _listOrders = value; OnPropertyChanged(); } }
        private ObservableCollection<RoomsReservation> _listRoomsRevs;
        public ObservableCollection<RoomsReservation> ListRoomsRevs { get => _listRoomsRevs; set { _listRoomsRevs = value; OnPropertyChanged(); } }
        //Statictis
        private string alltimerevenue = "0";
        public string Alltimerevenue { get => alltimerevenue; set { alltimerevenue = value; OnPropertyChanged(); } }
        private string alltimerevenueUSD = "0";
        public string AlltimerevenueUSD { get => alltimerevenueUSD; set { alltimerevenueUSD = value; OnPropertyChanged(); } }
        private string thisMonthRevenue = "0";
        public string ThisMonthRevenue { get => thisMonthRevenue; set { thisMonthRevenue = value; OnPropertyChanged(); } }

        //PieChart
        private SeriesCollection _SeriesCollectionPie;
        public SeriesCollection SeriesCollectionPie { get => _SeriesCollectionPie; set { _SeriesCollectionPie = value; OnPropertyChanged(); } }
        //CartesianChart
        private SeriesCollection _SeriesCollectionCart;
        public SeriesCollection SeriesCollectionCart { get => _SeriesCollectionCart; set { _SeriesCollectionCart = value; OnPropertyChanged(); } }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        //Load View
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand ShowSingleBedroomWindowCommand { get; set; }
        public ICommand RefreshStatictics { get; set; }

        public MainViewModel()
        {
            ListBills = new ObservableCollection<BillDetail>(DataProvider.Instance.DB.BillDetails);
            ListRooms = new ObservableCollection<Room>(DataProvider.Instance.DB.Rooms);
            ListRoomTypes = new ObservableCollection<RoomType>(DataProvider.Instance.DB.RoomTypes);
            ListFnSs = new ObservableCollection<FoodsAndService>(DataProvider.Instance.DB.FoodsAndServices);
            ListOrder = new ObservableCollection<Order>(DataProvider.Instance.DB.Orders);
            ListRoomsRevs = new ObservableCollection<RoomsReservation>(DataProvider.Instance.DB.RoomsReservations);

            _SeriesCollectionPie = new SeriesCollection();
            _SeriesCollectionCart = new SeriesCollection();

            LoadedWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                IsLoaded = true;
                var mainMenu = p as MainWindow;
                if (mainMenu == null)
                    return;
                mainMenu.Hide();
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();
                if (loginWindow.DataContext == null)
                    return;
                var loginVM = loginWindow.DataContext as LoginViewModel;
                if(loginVM.IsLogin)
                {
                    Authorise();
                    mainMenu.Show();
                }
                else
                {
                    mainMenu.Close();
                }
            }
            );

            ShowSingleBedroomWindowCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                //SingleBedroomWindow singleBedroomWindow = new SingleBedroomWindow();
                //singleBedroomWindow.ShowDialog();
                SingleBedroomInfoWindow singleBedroomWindow = new SingleBedroomInfoWindow();
                singleBedroomWindow.Show();
            }
            );

            RefreshStatictics = new RelayCommand<object>((p) => true, (p) => LoadStatistics());

            LoadStatistics();
        }
        public void Authorise()
        {
            Staff activeStaff = DataProvider.Instance.DB.Staffs.Single(x => x.ID == Const.ActiveAccount.IDStaff);
            if (activeStaff.Role == "Admin")
            {
                Const.AdminVisibility = Visibility.Visible;
                Const.StaffVisibility = Visibility.Collapsed;
            }
            else if (activeStaff.Role == "Staff")
            {
                Const.AdminVisibility = Visibility.Collapsed;
                Const.StaffVisibility = Visibility.Visible;
            }
        }
        public void LoadStatistics()
        {
            DateTime currentTime = DateTime.Now;
            decimal? allIncome = 0;
            decimal? monthIncome = 0;
            decimal? roomIncome = 0;
            decimal? foodIncome = 0;
            decimal? serviceIncome = 0;
            try
            {
                // All income
                allIncome = ListBills.Select(x => x.TotalMoney).Sum();
                Alltimerevenue = allIncome.ToString();
                AlltimerevenueUSD = (allIncome / 23035).ToString();
                // This month income
                monthIncome = ListBills.Where(x => ((DateTime)x.BillDate).ToString("MM") == currentTime.ToString("MM")).Select(x => x.TotalMoney).Sum();
                ThisMonthRevenue = monthIncome.ToString();

                // Room monthly income
                IEnumerable<RoomType> roomBills = from a in ListBills
                                                  join b in ListRoomsRevs on a.ID equals b.IDBillDetail
                                                  join c in ListRooms on b.IDRoom equals c.ID
                                                  join d in ListRoomTypes on c.IDRoomType equals d.ID
                                                  where ((DateTime)a.BillDate).ToString("MM") == currentTime.ToString("MM")
                                                  select d;
                roomIncome = roomBills.Select(x => x.Price).Sum();
                // Food monthly income
                IEnumerable<Order> foodBills = from a in ListBills
                                               join b in ListOrder on a.ID equals b.IDBillDetail
                                               join c in ListFnSs on b.IDFoodsAndServices equals c.ID
                                               where ((DateTime)a.BillDate).ToString("MM") == currentTime.ToString("MM")
                                               where c.Type == "Food"
                                               select b;
                foodIncome = (decimal?)foodBills.Select(x => x.TotalPrice).Sum();
                // Service monthly income
                IEnumerable<Order> serviceBills = from a in ListBills
                                                  join b in ListOrder on a.ID equals b.IDBillDetail
                                                  join c in ListFnSs on b.IDFoodsAndServices equals c.ID
                                                  where ((DateTime)a.BillDate).ToString("MM") == currentTime.ToString("MM")
                                                  where c.Type == "Service"
                                                  select b;
                serviceIncome = (decimal?)serviceBills.Select(x => x.TotalPrice).Sum();
            } catch { }
            
            SeriesCollectionPie.Clear();

            //Stactictics to PieChart
            var roomSeries = new PieSeries
            {
                Title = "Room Reservations",
                Values = new ChartValues<ObservableValue> { new ObservableValue(100) },
                //Values = new ChartValues<ObservableValue> { new ObservableValue(roomIncome) },
                DataLabels = true,
                FontSize = 16,
                LabelPoint = ChartPoint => string.Format("{0} ({1:P})", ChartPoint.Y, ChartPoint.Participation)
            };
            SeriesCollectionPie.Add(roomSeries);

            var foodSeries = new PieSeries
            {
                Title = "Food",
                Values = new ChartValues<ObservableValue> { new ObservableValue(200) },
                //Values = new ChartValues<ObservableValue> { new ObservableValue(foodIncome) },
                DataLabels = true,
                FontSize = 16,
                LabelPoint = ChartPoint => string.Format("{0} ({1:P})", ChartPoint.Y, ChartPoint.Participation)
            };
            SeriesCollectionPie.Add(foodSeries);

            var serviceSeries = new PieSeries
            {
                Title = "Services",
                Values = new ChartValues<ObservableValue> { new ObservableValue(300) },
                //Values = new ChartValues<ObservableValue> { new ObservableValue(serviceIncome) },
                DataLabels = true,
                FontSize = 16,
                LabelPoint = ChartPoint => string.Format("{0} ({1:P})", ChartPoint.Y, ChartPoint.Participation)
            };
            SeriesCollectionPie.Add(serviceSeries);

            int month = currentTime.Month;
            Labels = new[] { $"{month - 3}", $"{month - 2}", $"{month - 1}", $"{month}" };
            YFormatter = value => value.ToString("C");
        }
    }
}
