using HotelManagementApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace HotelManagementApp.ViewModel
{
    public class OrderViewModel : BaseViewModel
    {
        private ObservableCollection<Order> _PendingOrdersList;
        public ObservableCollection<Order> PendingOrdersList { get => _PendingOrdersList; set { _PendingOrdersList = value; OnPropertyChanged(); } }
        private ObservableCollection<FoodsAndService> _FilteredList;
        public ObservableCollection<FoodsAndService> FilteredList { get => _FilteredList; set { _FilteredList = value; OnPropertyChanged(); } }
        private int? _Quantity;
        public int? Quantity { get => _Quantity; set { _Quantity = value; OnPropertyChanged(); } }
        private decimal? _Total = 0;
        private string _Sort;
        public string Sort { get => _Sort; set { _Sort = value; LoadFilteredList(); OnPropertyChanged(); } }
        private string _TypeFilter;
        public string TypeFilter { get => _TypeFilter; set { _TypeFilter = value; LoadFilteredList(); OnPropertyChanged(); } }
        private string _SearchString;
        public string SearchString { get => _SearchString; set { _SearchString = value; LoadFilteredList(); OnPropertyChanged(); } }
        public decimal? Total { get => _Total; set { _Total = value; OnPropertyChanged(); } }
        private string _RoomNum;
        public string RoomNum
        {
            get => _RoomNum;
            set
            {
                _RoomNum = value;
                TargetBillDetail = new BillDetail();
                var billDetail = Global.BillsList.Where(x => x.Status == "On-Going" && x.RoomsReservations.Where(y => y.Room.RoomNum == RoomNum).FirstOrDefault() != null).FirstOrDefault();
                if (billDetail != null)
                {
                    TargetBillDetail = billDetail;
                }
                OnPropertyChanged();
            }
        }
        private BillDetail _TargetBillDetail;
        public BillDetail TargetBillDetail { get => _TargetBillDetail; set { _TargetBillDetail = value; OnPropertyChanged(); } }
        private FoodsAndService _SelectedItem;
        public FoodsAndService SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                var temp = PendingOrdersList.Where(x => x.FoodsAndService == value).FirstOrDefault();
                if (SelectedItem != null)
                {
                    if (temp == null)
                    {
                        var order = new Order();
                        order.FoodsAndService = _SelectedItem;
                        order.Quantity = 1;
                        order.TotalPrice = order.Quantity * _SelectedItem.Price;
                        order.Time = DateTime.Now;
                        order.Deleted = false;
                        PendingOrdersList.Add(order);
                        Quantity = order.Quantity;
                    }
                    else
                    {
                        var order = PendingOrdersList.Where(x => x == temp).FirstOrDefault();
                        order.Quantity += 1;
                        order.TotalPrice = order.Quantity * _SelectedItem.Price;
                        Quantity = order.Quantity;
                    }
                    Total += _SelectedItem.Price;
                }
                OnPropertyChanged();
            }
        }
        private Order _SelectedOrder;
        public Order SelectedOrder
        {
            get => _SelectedOrder;
            set
            {
                _SelectedOrder = value;
                if (SelectedOrder != null)
                {
                    Quantity = SelectedOrder.Quantity;
                }
            }
        }
        public ICommand EditOrderCommand { get; set; }
        public ICommand OrderCommand { get; set; }
        public ICommand ClearAllCommand { get; set; }
        public ICommand DelOrderBtn { get; set; }
        public ICommand QuantityIncreBtn { get; set; }
        public ICommand QuantityDecreBtn { get; set; }
        public OrderViewModel()
        {
            LoadFilteredList();
            PendingOrdersList = new ObservableCollection<Order>();
            ClearAllCommand = new RelayCommand<object>((p) =>
            {
                if (PendingOrdersList.Count == 0 || PendingOrdersList == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                if (PendingOrdersList != null)
                {
                    PendingOrdersList.Clear();
                    RoomNum = null;
                    Total = 0;
                }
                Quantity = null;
            });
            EditOrderCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedOrder == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                Total -= SelectedOrder.TotalPrice;
                SelectedOrder.Quantity = Quantity;
                SelectedOrder.TotalPrice = SelectedOrder.Quantity * SelectedOrder.FoodsAndService.Price;
                Total += SelectedOrder.TotalPrice;
            });
            OrderCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(RoomNum) || TargetBillDetail == null || PendingOrdersList.Count == 0 || PendingOrdersList == null)
                {
                    return false;
                }
                var list = Global.RoomsList.Where(x => x.RoomNum == RoomNum);
                if (list == null || list.Count() == 0)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var DBbill = DataProvider.Instance.DB.BillDetails.Where(x => x.ID == TargetBillDetail.ID).FirstOrDefault();
                var Sbill = Global.BillsList.Where(x => x.ID == DBbill.ID).FirstOrDefault();
                foreach (var item in PendingOrdersList)
                {
                    //some logic
                    DataProvider.Instance.DB.Orders.Add(item);
                    DBbill.Orders.Add(item);
                    DBbill.TotalMoney += item.TotalPrice;
                    Global.OrdersList.Add(item);
                }
                DataProvider.Instance.DB.SaveChanges();
                RoomNum = null;
                Total = 0;
                Quantity = 0;
                PendingOrdersList.Clear();
            }); 
            DelOrderBtn = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                Total -= ((Order)p).TotalPrice;
                PendingOrdersList.Remove(p as Order);
            });
            QuantityIncreBtn = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                ((Order)p).Quantity++;
                Total -= ((Order)p).TotalPrice;
                ((Order)p).TotalPrice = ((Order)p).Quantity * ((Order)p).FoodsAndService.Price;
                Total += ((Order)p).TotalPrice;
            }); 
            QuantityDecreBtn = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (((Order)p).Quantity <= 1) return;
                ((Order)p).Quantity--;
                Total -= ((Order)p).TotalPrice;
                ((Order)p).TotalPrice = ((Order)p).Quantity * ((Order)p).FoodsAndService.Price;
                Total += ((Order)p).TotalPrice;
            }); 
        }

        private void LoadFilteredList()
        {
            ObservableCollection<FoodsAndService> list = new ObservableCollection<FoodsAndService>();

            foreach (var item in Global.FoodsAndServicesList)
            {
                if (string.IsNullOrEmpty(Sort) && string.IsNullOrEmpty(SearchString) && string.IsNullOrEmpty(TypeFilter))
                {
                    list = Global.FoodsAndServicesList;
                }
                else if (string.IsNullOrEmpty(Sort) && string.IsNullOrEmpty(SearchString) && !string.IsNullOrEmpty(TypeFilter))
                {
                    if (item.Type == TypeFilter)
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    if ((string.IsNullOrEmpty(SearchString) || item.Name.Contains(SearchString)) && (item.Type == TypeFilter || string.IsNullOrEmpty(TypeFilter)))
                    {
                        list.Add(item);
                    }
                }
            }
            if (Sort == "Descending")
            {
                ObservableCollection<FoodsAndService> temp;
                temp = new ObservableCollection<FoodsAndService>(list.OrderByDescending(x => x.Price));
                list.Clear();
                foreach (var item in temp) list.Add(item);
            }
            else if (Sort == "Ascending")
            {
                ObservableCollection<FoodsAndService> temp;
                temp = new ObservableCollection<FoodsAndService>(list.OrderBy(x => x.Price));
                list.Clear();
                foreach (var item in temp) list.Add(item);
            }
            FilteredList = list;
        }

        
    }
}
