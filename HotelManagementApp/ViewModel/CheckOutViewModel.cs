using HotelManagementApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManagementApp.ViewModel
{
    public class CheckOutViewModel : BaseViewModel
    {
        private ObservableCollection<RoomsReservation> _FilteredList;
        public ObservableCollection<RoomsReservation> FilteredList { get => _FilteredList; set { _FilteredList = value; OnPropertyChanged(); } }
        private int? _Nights;
        public int? Nights { get => _Nights; set { _Nights = value; OnPropertyChanged(); } }
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

        public ICommand CheckOutCommand { get; set; }
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
                var bill = DataProvider.Instance.DB.BillDetails.Where(x => x.ID == SelectedBill.ID).FirstOrDefault();
                bill.BillDate = DateTime.Now;
                bill.Status = "Completed";
                foreach (var item in bill.RoomsReservations)
                {
                    Global.RoomsList.Remove(item.Room);
                    item.Room.Status = "Available";
                    Global.RoomsList.Add(item.Room);
                    Global.OnGoingReservationsList.Remove(item);
                }
                DataProvider.Instance.DB.SaveChanges();
                SelectedReservation = null;
                SelectedBill = null;
            });
        }

        private void LoadFilteredList()
        {
            FilteredList = Global.OnGoingReservationsList;
        }
    }
}
