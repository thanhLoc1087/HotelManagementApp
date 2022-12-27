using HotelManagementApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApp.ViewModel
{
    public class CheckOutViewModel : BaseViewModel
    {
        private ObservableCollection<RoomsReservation> _FilteredList;
        public ObservableCollection<RoomsReservation> FilteredList { get => _FilteredList; set { _FilteredList = value; OnPropertyChanged(); } }
        private RoomsReservation _SelectedItem;
        public RoomsReservation SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;

                OnPropertyChanged();
            }
        }
        public CheckOutViewModel()
        {
            LoadFilteredList();
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
