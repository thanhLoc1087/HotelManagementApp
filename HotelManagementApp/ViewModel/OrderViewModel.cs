using HotelManagementApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
        private string _RoomNum;
        public string RoomNum 
        {
            get => _RoomNum; 
            set 
            {
                _RoomNum = value;
                TargetBillDetail = new BillDetail();
                var billDetail = Global.BillsList.Where(x => x.Status == "On-Going" && x.RoomsReservations.Where(y => y.Room.RoomNum == RoomNum).FirstOrDefault() != null).FirstOrDefault();
                if(billDetail != null)
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
                if (SelectedItem != null)
                {
                    var order = new Order();
                    order.IDFoodsAndServices = SelectedItem.ID;
                    order.Quantity = 1;
                    order.TotalPrice = order.Quantity * SelectedItem.Price;
                    order.Time = DateTime.Now;
                    order.Deleted = false;
                    PendingOrdersList.Add(order);
                }
                OnPropertyChanged();
            }
        }

        public ICommand Selection { get; set; }
        public ICommand OrderCommand { get; set; }
        public OrderViewModel()
        {
            LoadFilteredList();
            PendingOrdersList = new ObservableCollection<Order>();
            OrderCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(RoomNum) || TargetBillDetail == null || PendingOrdersList == null || SelectedItem == null)
                {
                    return false;
                }
                var list = Global.RoomsList.Where(x => x.RoomNum == RoomNum);
                if(list == null || list.Count() == 0)
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
                    Sbill.Orders.Add(item);
                    Sbill.TotalMoney += item.TotalPrice;
                }
                DataProvider.Instance.DB.SaveChanges();
                SelectedItem = null;
                PendingOrdersList.Clear();
            });
        }

        private void LoadFilteredList()
        {
            
        }
    }
}
