using HotelManagementApp.Model;
using HotelManagementApp.View;
using HotelManagementApp.View.Reservation;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HotelManagementApp.ViewModel
{

    public class MainViewModel : BaseViewModel
    {
        public bool IsLoaded = false;
        public string _tbkStatError = "";
        public string tbkStatError { get => _tbkStatError; set { _tbkStatError = value; OnPropertyChanged(); } }
        //Statictis
        public string dateFormat = "dd/MM/yyyy";
        private FoodsAndService _bestSeller;
        public FoodsAndService bestSeller { get => _bestSeller; set { _bestSeller = value; OnPropertyChanged(); } }
        private int? _bestSellerQuantity = null;
        public int? bestSellerQuantity { get => _bestSellerQuantity; set { _bestSellerQuantity = value; OnPropertyChanged(); } }
        private string alltimerevenue = "0";
        public string Alltimerevenue { get => alltimerevenue; set { alltimerevenue = value; OnPropertyChanged(); } }
        private string alltimerevenueUSD = "0";
        public string AlltimerevenueUSD { get => alltimerevenueUSD; set { alltimerevenueUSD = value; OnPropertyChanged(); } }
        private string selectedDateRevenue = "0";
        public string SelectedDateRevenue { get => selectedDateRevenue; set { selectedDateRevenue = value; OnPropertyChanged(); } }
        public DateTime oldest = (DateTime)Global.BillsList.Min(x => x.BillDate);
        public DateTime Oldest { get => oldest; set { oldest = value; OnPropertyChanged(); } }
        private string _lblDateFilter;
        public string lblDateFilter { get => _lblDateFilter; set { _lblDateFilter = value; OnPropertyChanged(); } }
        private Visibility _isDate = Visibility.Visible;
        public Visibility isDate { get => _isDate; set { _isDate = value; OnPropertyChanged(); } }
        private Visibility _isNotDate = Visibility.Collapsed;
        public Visibility isNotDate { get => _isNotDate; set { _isNotDate = value; OnPropertyChanged(); } }
        private Visibility _isMonth = Visibility.Collapsed;
        public Visibility isMonth { get => _isMonth; set { _isMonth = value; OnPropertyChanged(); } }
        private List<int> _months = new List<int>{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        public List<int> months { get => _months; set { _months = value; OnPropertyChanged(); } }
        private List<int> _years = new List<int>();
        public List<int> years { get => _years; set { _years = value; OnPropertyChanged(); } }
        private int _selectedMonth = DateTime.Now.Month;
        public int selectedMonth { 
            get => _selectedMonth; 
            set 
            { 
                _selectedMonth = value; OnPropertyChanged();
                caSelectedDate = DateTime.Parse(String.Format("1/{0}/{1}", value, selectedYear));
            } 
        }
        private int _selectedYear = DateTime.Now.Year;
        public int selectedYear { 
            get => _selectedYear; 
            set 
            { 
                _selectedYear = value; OnPropertyChanged();
                caSelectedDate = DateTime.Parse(String.Format("1/1/{0}", value));
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
        public string _cbxSelectedValue = "Date";
        public string cbxSelectedValue
        {
            get { return _cbxSelectedValue; }
            set
            {
                _cbxSelectedValue = value;
                SetFilterFormat();
                base.OnPropertyChanged("cbxSelectedValue");
                LoadStatistics();
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

        //Load View
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand ShowSingleBedroomWindowCommand { get; set; }
        public ICommand RefreshStatictics { get; set; }
        public ICommand ExitApplicationCommand { get; set; }


        public MainViewModel()
        {
            SeriesCollectionPie = new SeriesCollection();
            SeriesCollectionCart = new SeriesCollection();

            ExitApplicationCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                System.Windows.Application.Current.Shutdown();
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
                if (loginVM.IsLogin)
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

            for (int i = oldest.Year; i <= DateTime.Now.Year; i++)
            {
                years.Add(i);
            }
            LoadStatistics();
        }
        public void Authorise()
        {
            Staff activeStaff = DataProvider.Instance.DB.Staffs.Single(x => x.ID == Const.ActiveAccount.IDStaff);
            if (activeStaff.Role == "Admin")
            {
                Const.IsAdmin = true;
                Const.AdminVisibility = Visibility.Visible;
                Const.StaffVisibility = Visibility.Collapsed;
            }
            else if (activeStaff.Role == "Staff")
            {
                Const.IsAdmin = false;
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
                    isDate = Visibility.Visible;
                    isMonth = Visibility.Collapsed;
                    isNotDate = Visibility.Collapsed;
                    break;
                case "System.Windows.Controls.ComboBoxItem: Month":
                    dateFormat = "MM/yyyy";
                    isDate = Visibility.Collapsed;
                    isMonth = Visibility.Visible;
                    isNotDate = Visibility.Visible;
                    break;
                case "System.Windows.Controls.ComboBoxItem: Year":
                    dateFormat = "yyyy";
                    isDate = Visibility.Collapsed;
                    isMonth = Visibility.Collapsed;
                    isNotDate = Visibility.Visible;
                    break;
            }
        }
        public void LoadStatistics()
        {
            DateTime selectedTime = (DateTime)caSelectedDate;

            // Best seller
            try
            {
                var query = from a in Global.BillsList
                            join b in Global.OrdersList on a.ID equals b.IDBillDetail
                            join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                            where ((DateTime)a.BillDate).ToString(dateFormat) == selectedTime.ToString(dateFormat)
                            select new { c, b };
                var temp = query.GroupBy(a => a.c);
                int? bestSellerID = null;
                decimal? bestSellerPrice = 0;
                bestSellerQuantity = 0;
                foreach (var fns in temp ) {
                    decimal? income = 0;
                    int? quantity = 0;
                    foreach(var order in fns)
                    {
                        income += order.b.TotalPrice;
                        quantity += order.b.Quantity;
                        bestSellerID = order.c.ID;
                    }
                    if (bestSellerPrice < income)
                    {
                        bestSellerPrice = income;
                        bestSellerQuantity = (int)quantity;
                        break;
                    }
                }
                bestSeller = Global.FoodsAndServicesList.Single(x => x.ID == bestSellerID);
            }
            catch { 
                bestSeller = null; bestSellerQuantity = null;
            }

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
                var roomBills = from a in Global.BillsList
                                join rr in Global.ReservationsList on a.ID equals rr.IDBillDetail
                                join c in Global.RoomsList on rr.IDRoom equals c.ID
                                join rt in Global.Types on c.IDRoomType equals rt.ID
                                where a.Status == "Completed"
                                where ((DateTime)a.BillDate).ToString(dateFormat) == selectedTime.ToString(dateFormat)
                                select new { rr, rt };
                foreach (var r in roomBills)
                {
                    int nights = (int)r.rr.CheckOutTime.Value.Subtract(r.rr.CheckInTime.Value).TotalDays;
                    roomIncome += r.rt.Price * (nights > 0 ? nights : 1);
                }
                // Food monthly income
                IEnumerable<Order> foodBills = from a in Global.BillsList
                                               join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                               join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                               where a.Status == "Completed"
                                               where ((DateTime)a.BillDate).ToString(dateFormat) == selectedTime.ToString(dateFormat)
                                               where c.Type == "Food"
                                               select b;
                foodIncome = foodBills.Select(x => x.TotalPrice).Sum();
                // Service monthly income
                IEnumerable<Order> serviceBills = from a in Global.BillsList
                                                  join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                  join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                  where c.Type == "Service"
                                                  where a.Status == "Completed"
                                                  where ((DateTime)a.BillDate).ToString(dateFormat) == selectedTime.ToString(dateFormat)
                                                  select b;
                serviceIncome = serviceBills.Select(x => x.TotalPrice).Sum();

                ////    CARTESAN STATTISTIC     ////
                ///Imcome for room quarter 1 
                var roomBillsQ1 = from a in Global.BillsList
                                  join rr in Global.ReservationsList on a.ID equals rr.IDBillDetail
                                  join c in Global.RoomsList on rr.IDRoom equals c.ID
                                  join rt in Global.Types on c.IDRoomType equals rt.ID
                                  where a.Status == "Completed"
                                  where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                  where quart1.Contains(((DateTime)rr.CheckOutTime).ToString("MM"))
                                  select new { rr, rt };
                foreach (var rr in roomBillsQ1)
                {
                    int nights = (int)rr.rr.CheckOutTime.Value.Subtract(rr.rr.CheckInTime.Value).TotalDays;
                    roomIncomeQ1 += rr.rt.Price * (nights > 0 ? nights : 1);
                }
                ///Imcome for room quarter 2 
                var roomBillsQ2 = from a in Global.BillsList
                                  join rr in Global.ReservationsList on a.ID equals rr.IDBillDetail
                                  join c in Global.RoomsList on rr.IDRoom equals c.ID
                                  join rt in Global.Types on c.IDRoomType equals rt.ID
                                  where a.Status == "Completed"
                                  where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                  where quart2.Contains(((DateTime)rr.CheckOutTime).ToString("MM"))
                                  select new { rr, rt };
                foreach (var rr in roomBillsQ2)
                {
                    int nights = (int)rr.rr.CheckOutTime.Value.Subtract(rr.rr.CheckInTime.Value).TotalDays;
                    roomIncomeQ2 += rr.rt.Price * (nights > 0 ? nights : 1);
                }
                ///Imcome for room quarter 3
                var roomBillsQ3 = from a in Global.BillsList
                                  join rr in Global.ReservationsList on a.ID equals rr.IDBillDetail
                                  join c in Global.RoomsList on rr.IDRoom equals c.ID
                                  join rt in Global.Types on c.IDRoomType equals rt.ID
                                  where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                  where a.Status == "Completed"
                                  where quart3.Contains(((DateTime)rr.CheckOutTime).ToString("MM"))
                                  select new { rr, rt };
                foreach (var rr in roomBillsQ3)
                {
                    int nights = (int)rr.rr.CheckOutTime.Value.Subtract(rr.rr.CheckInTime.Value).TotalDays;
                    roomIncomeQ3 += rr.rt.Price * (nights > 0 ? nights : 1);
                }
                ///Imcome for room quarter 4
                var roomBillsQ4 = from a in Global.BillsList
                                  join rr in Global.ReservationsList on a.ID equals rr.IDBillDetail
                                  join c in Global.RoomsList on rr.IDRoom equals c.ID
                                  join rt in Global.Types on c.IDRoomType equals rt.ID
                                  where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                  where a.Status == "Completed"
                                  where quart4.Contains(((DateTime)rr.CheckOutTime).ToString("MM"))
                                  select new { rr, rt };
                foreach (var rr in roomBillsQ4)
                {
                    int nights = (int)rr.rr.CheckOutTime.Value.Subtract(rr.rr.CheckInTime.Value).TotalDays;
                    roomIncomeQ4 += rr.rt.Price * (nights > 0 ? nights : 1);
                }
                ///Imcome for FOOD quarter 1        
                IEnumerable<Order> foodBillsQ1 = from a in Global.BillsList
                                                 join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                 join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                 where quart1.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                 where a.Status == "Completed"
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
                                                 where a.Status == "Completed"
                                                 where c.Type == "Food"
                                                 select b;
                foodIncomeQ2 = foodBillsQ2.Select(y => y.TotalPrice).Sum();
                ///Imcome for FOOD quarter 3     
                IEnumerable<Order> foodBillsQ3 = from a in Global.BillsList
                                                 join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                 join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                 where quart3.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                 where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                 where a.Status == "Completed"
                                                 where c.Type == "Food"
                                                 select b;
                foodIncomeQ3 = foodBillsQ3.Select(y => y.TotalPrice).Sum();
                ///Imcome for FOOD quarter 4      
                IEnumerable<Order> foodBillsQ4 = from a in Global.BillsList
                                                 join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                 join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                 where quart4.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                 where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                 where a.Status == "Completed"
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
                                                    where a.Status == "Completed"
                                                    select b;
                serviceIncomeQ1 = serviceBillsQ1.Select(x => x.TotalPrice).Sum();
                ///Imcome for SERVICE quarter 2
                IEnumerable<Order> serviceBillsQ2 = from a in Global.BillsList
                                                    join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                    join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                    where c.Type == "Service"
                                                    where quart2.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                    where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                    where a.Status == "Completed"
                                                    select b;
                serviceIncomeQ2 = serviceBillsQ2.Select(x => x.TotalPrice).Sum();
                ///Imcome for SERVICE quarter 3
                IEnumerable<Order> serviceBillsQ3 = from a in Global.BillsList
                                                    join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                    join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                    where c.Type == "Service"
                                                    where quart3.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                    where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                    where a.Status == "Completed"
                                                    select b;
                serviceIncomeQ3 = serviceBillsQ3.Select(x => x.TotalPrice).Sum();
                ///Imcome for SERVICE quarter 4
                IEnumerable<Order> serviceBillsQ4 = from a in Global.BillsList
                                                    join b in Global.OrdersList on a.ID equals b.IDBillDetail
                                                    join c in Global.FoodsAndServicesList on b.IDFoodsAndServices equals c.ID
                                                    where c.Type == "Service"
                                                    where quart4.Contains(((DateTime)a.BillDate).ToString("MM"))
                                                    where ((DateTime)a.BillDate).ToString("yyyy") == selectedTime.ToString("yyyy")
                                                    where a.Status == "Completed"
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
                    Values = new ChartValues<ObservableValue> { new ObservableValue((double)roomIncome) },
                }; 
                SeriesCollectionPie.Add(roomSeries);

                var foodSeries = new PieSeries
                {
                    Title = "Food",
                    Values = new ChartValues<ObservableValue> { new ObservableValue((double)foodIncome) },
                }; 
                SeriesCollectionPie.Add(foodSeries);

                var serviceSeries = new PieSeries
                {
                    Title = "Services",
                    Values = new ChartValues<ObservableValue> { new ObservableValue((double)serviceIncome) },
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

            var culture = CultureInfo.CurrentCulture;
            var mutableNfi = (NumberFormatInfo)culture.NumberFormat.Clone();
            mutableNfi.CurrencySymbol = "";
            XFormatter = new[] { "Quarter 1", "Quarter 2", "Quarter 3", "Quarter 4" };
            YFormatter = value => value.ToString("C0", mutableNfi);
            if (foodIncome == 0 && serviceIncome == 0 && roomIncome == 0)
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