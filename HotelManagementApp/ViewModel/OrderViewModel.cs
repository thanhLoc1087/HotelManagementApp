using HotelManagementApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private Order _SelectedItem;
        public Order SelectedItem
        { 
            get => _SelectedItem;
            set 
            { 
                _SelectedItem = value;
                if (SelectedItem != null)
                {
                    PendingOrdersList.Add(SelectedItem);
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
                if (string.IsNullOrEmpty(RoomNum) || TargetBillDetail == null || PendingOrdersList == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                foreach (var item in PendingOrdersList)
                {
                    TargetBillDetail.Orders.Add(item);
                }

                DataProvider.Instance.DB.SaveChanges();
                SelectedItem = null;
            });
        }

        private void LoadFilteredList()
        {
            
        }
    }
}
