using HotelManagementApp.Model;
using HotelManagementApp.View;
using HotelManagementApp.View.Reservation;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManagementApp.ViewModel
{

    public class MainViewModel : BaseViewModel
    {      
        public bool IsLoaded = false;
        public string _tbkStatError = "";
        public string tbkStatError { get => _tbkStatError; set { _tbkStatError = value; OnPropertyChanged(); } }
        //Statictis
        public string dateFormat = "dd/MM/yyyy";
        private string alltimerevenue = "0";
        public string Alltimerevenue { get => alltimerevenue; set { alltimerevenue = value; OnPropertyChanged(); } }
        private string alltimerevenueUSD = "0";
        public string AlltimerevenueUSD { get => alltimerevenueUSD; set { alltimerevenueUSD = value; OnPropertyChanged(); } }
        private string selectedDateRevenue = "0";
        public string SelectedDateRevenue { get => selectedDateRevenue; set { selectedDateRevenue = value; OnPropertyChanged(); } }
        private string _lblDateFilter;
        public string lblDateFilter { get => _lblDateFilter; set { _lblDateFilter = value; OnPropertyChanged(); } }
        private CalendarMode _caDisplayMode = CalendarMode.Month;
        public CalendarMode caDisplayMode { get => _caDisplayMode; set { _caDisplayMode = value; OnPropertyChanged("caDisplayMode"); } }
        public string _cbxSelectedValue;
        public string cbxSelectedValue
        {
            get { return _cbxSelectedValue; }
            set
            {
                _cbxSelectedValue = value;
                base.OnPropertyChanged("cbxSelectedValue");
                SetFilterFormat();
            }
        }
        public DateTime? _caSelectedDate = DateTime.Now;
        public DateTime? caSelectedDate
        {
            get { return _caSelectedDate; }
            set
            {
                _caSelectedDate = value;
                base.OnPropertyChanged("caSelectedDate");
                LoadStatistics();
            }
        }

        //PieChart
        private SeriesCollection _SeriesCollectionPie;
        public SeriesCollection SeriesCollectionPie { get => _SeriesCollectionPie; set { _SeriesCollectionPie = value; OnPropertyChanged(); } }
        //CartesianChart
        private SeriesCollection _SeriesCollectionCart;
        public SeriesCollection SeriesCollectionCart { get => _SeriesCollectionCart; set { _SeriesCollectionCart = value; OnPropertyChanged(); } }
        public string[] XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

        //Load View
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand ShowSingleBedroomWindowCommand { get; set; }
        public ICommand RefreshStatictics { get; set; }

        //test
        public ICommand TestCommand { get; set; }

        public MainViewModel()
        {
            _SeriesCollectionPie = new SeriesCollection();
            _SeriesCollectionCart = new SeriesCollection();

            TestCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                AddReservationWindow window = new AddReservationWindow();
                window.Show();
            }
            );

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

            RefreshStatictics = new RelayCommand<object>((p) => true, (p) =>
            {
                caSelectedDate = DateTime.Now;
                LoadStatistics();
            });

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
        public void SetFilterFormat()
        {
            switch (cbxSelectedValue.ToString())
            {
                case "System.Windows.Controls.ComboBoxItem: Date":
                    dateFormat = "dd/MM/yyyy";
                    caDisplayMode = CalendarMode.Month;
                    break;
                case "System.Windows.Controls.ComboBoxItem: Month":
                    dateFormat = "MM/yyyy";
                    caDisplayMode = CalendarMode.Year;
                    break;
                case "System.Windows.Controls.ComboBoxItem: Year":
                    dateFormat = "yyyy";
                    caDisplayMode = CalendarMode.Decade;
                    break;
            }
        }
        public void LoadStatistics()
        {
            DateTime selectedTime = (DateTime)caSelectedDate;
            decimal? allIncome = 0;
            decimal? selectedDateIncome = 0;
            decimal? roomIncome = 0;
            decimal? foodIncome = 0;
            decimal? serviceIncome = 0;
            var quart1 = new[] { "01", "02", "03" };
            var quart2 = new[] { "04", "05", "06" };
            var quart3 = new[] { "07", "08", "09" };
            var quart4 = new[] { "10", "11", "12" };
            decimal? roomIncomeQ1 = 0;
            decimal? roomIncomeQ2 = 0;
            decimal? roomIncomeQ3 = 0;
            decimal? roomIncomeQ4 = 0;
            decimal? foodIncomeQ1 = 0;
            decimal? foodIncomeQ2 = 0;
            decimal? foodIncomeQ3 = 0;
            decimal? foodIncomeQ4 = 0;
            decimal? serviceIncomeQ1 = 0;
            decimal? serviceIncomeQ2 = 0;
            decimal? serviceIncomeQ3 = 0;
            decimal? serviceIncomeQ4 = 0;
            lblDateFilter = ((DateTime)caSelectedDate).ToString(dateFormat);
            try
            {
                // All income
                allIncome = Global.BillsList.Select(x => x.TotalMoney).Sum();
                Alltimerevenue = ((decimal)allIncome).ToString("N0");
                AlltimerevenueUSD = ((decimal)allIncome / 23035).ToString("N4");
                // This selected date income
                selectedDateIncome = Global.BillsList.Where(x => ((DateTime)x.BillDate).ToString(dateFormat) == selectedTime.ToString(dateFormat)).Select(x => x.TotalMoney).Sum();
                SelectedDateRevenue = ((decimal)selectedDateIncome).ToString("N0");

                ////    PIE STATISTIC   ////    
                // Room monthly income
                IEnumerable<RoomType> roomBills = from a in Global.BillsList
                                                  join b in Global.ReservationsList on a.ID equals b.IDBillDetail
                                                  join c in Global.RoomsList on b.IDRoom equals c.ID
                                                  join d in Global.Types on c.IDRoomType equals d.ID
                                                  where ((DateTime)a.BillDate).ToString(dateFormat) == selectedTime.ToString(dateFormat)
                                                  select d;
                roomIncome = roomBills.Select(x => x.Price).Sum();
                // Food monthly income
                IEnumerable<Order> foodBills = from a in Global.BillsList
                                               join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                               join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                               where ((DateTime)a.BillDate).ToString(dateFormat) == selectedTime.ToString(dateFormat)
                                               where c.Type == "Food"
                                               select b;
                foodIncome = foodBills.Select(x => x.TotalPrice).Sum();
                // Service monthly income
                IEnumerable<Order> serviceBills = from a in Global.BillsList
                                                  join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                  join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                  where c.Type == "Service"
                                                  where ((DateTime)a.BillDate).ToString(dateFormat) == selectedTime.ToString(dateFormat)
                                                  select b;
                serviceIncome = serviceBills.Select(x => x.TotalPrice).Sum();

                ////    CARTESAN STATTISTIC     ////
                ///Imcome for room quarter 1 
                IEnumerable<RoomType> roomBillsQ1 = from a in Global.BillsList
                                                    join b in Global.ReservationsList on a.ID equals b.IDBillDetail
                                                    join c in Global.RoomsList on b.IDRoom equals c.ID
                                                    join d in Global.Types on c.IDRoomType equals d.ID
                                                    where quart1.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                    where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                    select d;
                roomIncomeQ1 = roomBillsQ1.Select(y => y.Price).Sum();
                ///Imcome for room quarter 2 
                IEnumerable<RoomType> roomBillsQ2 = from a in Global.BillsList
                                                    join b in Global.ReservationsList on a.ID equals b.IDBillDetail
                                                    join c in Global.RoomsList on b.IDRoom equals c.ID
                                                    join d in Global.Types on c.IDRoomType equals d.ID
                                                    where quart2.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                    where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                    select d;
                roomIncomeQ2 = roomBillsQ2.Select(y => y.Price).Sum();
                ///Imcome for room quarter 3
                IEnumerable<RoomType> roomBillsQ3 = from a in Global.BillsList
                                                    join b in Global.ReservationsList on a.ID equals b.IDBillDetail
                                                    join c in Global.RoomsList on b.IDRoom equals c.ID
                                                    join d in Global.Types on c.IDRoomType equals d.ID
                                                    where quart3.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                    where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                    select d;
                roomIncomeQ3 = roomBillsQ3.Select(y => y.Price).Sum();
                ///Imcome for room quarter 4
                IEnumerable<RoomType> roomBillsQ4 = from a in Global.BillsList
                                                    join b in Global.ReservationsList on a.ID equals b.IDBillDetail
                                                    join c in Global.RoomsList on b.IDRoom equals c.ID
                                                    join d in Global.Types on c.IDRoomType equals d.ID
                                                    where quart4.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                    where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                    select d;
                roomIncomeQ4 = roomBillsQ4.Select(y => y.Price).Sum();
                ///Imcome for FOOD quarter 1        
                IEnumerable<Order> foodBillsQ1 = from a in Global.BillsList
                                                 join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                 join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                 where quart1.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                 where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                 where c.Type == "Food"
                                                 select b;
                foodIncomeQ1 = foodBillsQ1.Select(y => y.TotalPrice).Sum();
                ///Imcome for FOOD quarter 2  
                IEnumerable<Order> foodBillsQ2 = from a in Global.BillsList
                                                 join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                 join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                 where quart2.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                 where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                 where c.Type == "Food"
                                                 select b;
                foodIncomeQ2 = foodBillsQ2.Select(y => y.TotalPrice).Sum();
                ///Imcome for FOOD quarter 3     
                IEnumerable<Order> foodBillsQ3 = from a in Global.BillsList
                                                 join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                 join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                 where quart3.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                 where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                 where c.Type == "Food"
                                                 select b;
                foodIncomeQ3 = foodBillsQ3.Select(y => y.TotalPrice).Sum();
                ///Imcome for FOOD quarter 4      
                IEnumerable<Order> foodBillsQ4 = from a in Global.BillsList
                                                 join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                 join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                 where quart4.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                 where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                 where c.Type == "Food"
                                                 select b;
                foodIncomeQ4 = foodBillsQ4.Select(y => y.TotalPrice).Sum();
                ///Imcome for SERVICE quarter 1 
                IEnumerable<Order> serviceBillsQ1 = from a in Global.BillsList
                                                    join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                    join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                    where c.Type == "Service"
                                                    where quart1.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                    where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                    select b;
                serviceIncomeQ1 = serviceBillsQ1.Select(x => x.TotalPrice).Sum();
                ///Imcome for SERVICE quarter 2
                IEnumerable<Order> serviceBillsQ2 = from a in Global.BillsList
                                                    join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                    join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                    where c.Type == "Service"
                                                    where quart2.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                    where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                    select b;
                serviceIncomeQ2 = serviceBillsQ2.Select(x => x.TotalPrice).Sum();
                ///Imcome for SERVICE quarter 3
                IEnumerable<Order> serviceBillsQ3 = from a in Global.BillsList
                                                    join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                    join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                    where c.Type == "Service"
                                                    where quart3.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                    where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                    select b;
                serviceIncomeQ3 = serviceBillsQ3.Select(x => x.TotalPrice).Sum();
                ///Imcome for SERVICE quarter 4
                IEnumerable<Order> serviceBillsQ4 = from a in Global.BillsList
                                                    join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                    join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                    where c.Type == "Service"
                                                    where quart4.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                    where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                    select b;
                serviceIncomeQ4 = serviceBillsQ4.Select(x => x.TotalPrice).Sum();
            }
            catch { }

            SeriesCollectionPie.Clear();
            SeriesCollectionCart.Clear();

            //Stactictics to PieChart
            {
                var roomSeries = new PieSeries
                {
                    Title = "Room Reservations",
                    //Values = new ChartValues<ObservableValue> { new ObservableValue(100) },
                    Values = new ChartValues<ObservableValue> { new ObservableValue((double)roomIncome) },
                    DataLabels = true,
                    FontSize = 16,
                    LabelPoint = ChartPoint => string.Format("{0} ({1:P})", ChartPoint.Y, ChartPoint.Participation)
                };
                SeriesCollectionPie.Add(roomSeries);

                var foodSeries = new PieSeries
                {
                    Title = "Food",
                    //Values = new ChartValues<ObservableValue> { new ObservableValue(200) },
                    Values = new ChartValues<ObservableValue> { new ObservableValue((double)foodIncome) },
                    DataLabels = true,
                    FontSize = 16,
                    LabelPoint = ChartPoint => string.Format("{0} ({1:P})", ChartPoint.Y, ChartPoint.Participation)
                };
                SeriesCollectionPie.Add(foodSeries);

                var serviceSeries = new PieSeries
                {
                    Title = "Services",
                    //Values = new ChartValues<ObservableValue> { new ObservableValue(300) },
                    Values = new ChartValues<ObservableValue> { new ObservableValue((double)serviceIncome) },
                    DataLabels = true,
                    FontSize = 16,
                    LabelPoint = ChartPoint => string.Format("{0} ({1:P})", ChartPoint.Y, ChartPoint.Participation)
                };
                SeriesCollectionPie.Add(serviceSeries);
            }

            var roomLine = new LineSeries
            {
                Title = "Room",
                Values = new ChartValues<decimal> { (decimal)roomIncomeQ1, (decimal)roomIncomeQ2, (decimal)roomIncomeQ3, (decimal)roomIncomeQ4 }
            };
            SeriesCollectionCart.Add(roomLine);
            var foodLine = new LineSeries
            {
                Title = "Food",
                Values = new ChartValues<decimal> { (decimal)foodIncomeQ1, (decimal)foodIncomeQ2, (decimal)foodIncomeQ3, (decimal)foodIncomeQ4 }
            };
            SeriesCollectionCart.Add(foodLine);
            var serviceLine = new LineSeries
            {
                Title = "Service",
                Values = new ChartValues<decimal> { (decimal)serviceIncomeQ1, (decimal)serviceIncomeQ2, (decimal)serviceIncomeQ3, (decimal)serviceIncomeQ4 }
            };
            SeriesCollectionCart.Add(serviceLine);

            XFormatter = new[] { "Quarter 1", "Quarter 2", "Quarter 3", "Quarter 4" };
            YFormatter = value => value.ToString("C");
            if (foodIncome ==0 && serviceIncome ==0 && roomIncome == 0)
            {
                tbkStatError = Const.statErrorMsg;
            } 
            else
            {
                tbkStatError = "";
            }
        }
    }
}
