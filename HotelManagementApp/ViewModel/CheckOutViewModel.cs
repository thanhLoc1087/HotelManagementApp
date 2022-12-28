using HotelManagementApp.Model;
using HotelManagementApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManagementApp.ViewModel
{
    public class CheckOutViewModel : BaseViewModel
    {
        private ObservableCollection<RoomsReservation> _FilteredList;
        public ObservableCollection<RoomsReservation> FilteredList { get => _FilteredList; set { _FilteredList = value; OnPropertyChanged(); } }
        private ObservableCollection<RoomsReservation> _roomRevsCheckOut;
        public ObservableCollection<RoomsReservation> RoomRevsCheckOut { get => _roomRevsCheckOut; set { _roomRevsCheckOut = value; OnPropertyChanged(); } }
        private int? _Nights;
        public int? Nights { get => _Nights; set { _Nights = value; OnPropertyChanged(); } }
        private string _checkOutDate;
        public string CheckOutDate { get => _checkOutDate; set { _checkOutDate = value; OnPropertyChanged(); } }
        private string _checkInDate;
        public string CheckInDate { get => _checkInDate; set { _checkInDate = value; OnPropertyChanged(); } }
        private decimal? _FnSTotal;
        public decimal? FnSTotal { get => _FnSTotal; set { _FnSTotal = value; OnPropertyChanged(); } }
        private decimal? _RoomsTotal;
        public decimal? RoomsTotal { get => _RoomsTotal; set { _RoomsTotal = value; OnPropertyChanged(); } }
        private bool _billConfirmed = false;
        public bool BillConfirmed { get => _billConfirmed; set { _billConfirmed = value; OnPropertyChanged(); } }
        private PaymentWindow paymentWindow;
        public PaymentWindow PaymentWindow { get => paymentWindow; set { paymentWindow = value; OnPropertyChanged(); } }
        private RoomsReservation _SelectedReservation;
        public RoomsReservation SelectedReservation
        {
            get => _SelectedReservation;
            set
            {
                _SelectedReservation = value;
                if(value != null)
                {
                    SelectedBill = Global.BillsList.Where(x => x.ID == _SelectedReservation.IDBillDetail).FirstOrDefault();
                    Nights = (int)_SelectedReservation.CheckOutTime.Value.Subtract(_SelectedReservation.CheckInTime.Value).TotalDays;
                }
                OnPropertyChanged();
            }
        }

        private BillDetail _SelectedBill;
        public BillDetail SelectedBill
        {
            get => _SelectedBill;
            set
            {
                _SelectedBill = value;

                OnPropertyChanged();
            }
        }
        private BillDetail _bill;
        public BillDetail Bill
        {
            get => _bill;
            set
            {
                _bill = value;
                OnPropertyChanged();
            }
        } 

        public ICommand CheckOutCommand { get; set; }
        public ICommand ConfirmBillBtn { get; set; }
        public CheckOutViewModel()
        {
            LoadFilteredList();
            CheckOutCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedBill == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                Bill = DataProvider.Instance.DB.BillDetails.Where(x => x.ID == SelectedBill.ID).FirstOrDefault();
                PaymentWindow = new PaymentWindow();
                Bill.BillDate = DateTime.Now;
                CheckInDate = ((DateTime)Bill.RoomsReservations.FirstOrDefault().CheckInTime).ToString("G");
                CheckOutDate = ((DateTime)Bill.RoomsReservations.FirstOrDefault().CheckOutTime).ToString("G");
                RoomsTotal = Bill.RoomsReservations.Select(x => x.Room.RoomType.Price * Nights).Sum();
                FnSTotal = Bill.Orders.Select(x=>x.TotalPrice).Sum();
                PaymentWindow.ShowDialog();
            });
            ConfirmBillBtn = new RelayCommand<object>((p) => true, (p) =>
            {
                Bill.Status = "Completed";
                foreach (var item in Bill.RoomsReservations)
                {
                    Global.RoomsList.Remove(item.Room);
                    item.Room.Status = "Available";
                    Global.RoomsList.Add(item.Room);
                }
                DataProvider.Instance.DB.SaveChanges();
                SelectedReservation = null;
                SelectedBill = null;
                PaymentWindow.Close();
            });
        }

        private void LoadFilteredList()
        {
            ObservableCollection<RoomsReservation> list = new ObservableCollection<RoomsReservation>();
            foreach(var item in Global.ReservationsList)
            {
                if(item.BillDetail.Status == "On-Going")
                {
                    list.Add(item);
                }
            }
            FilteredList = list;
        }
    }
}
