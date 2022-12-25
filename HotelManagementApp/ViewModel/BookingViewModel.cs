using HotelManagementApp.Model;
using HotelManagementApp.View.Reservation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HotelManagementApp.ViewModel
{
    public class BookingViewModel : BaseViewModel
    {
        private ObservableCollection<Room> _FilteredList;
        public ObservableCollection<Room> FilteredList { get => _FilteredList; set { _FilteredList = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _SuggestionsList;
        public ObservableCollection<string> SuggestionsList { get => _SuggestionsList; set { _SuggestionsList = value; OnPropertyChanged(); } }
        private ObservableCollection<RoomsReservation> _PendingReservationsList;
        public ObservableCollection<RoomsReservation> PendingReservationList { get => PendingReservationList; set { PendingReservationList = value; OnPropertyChanged(); } }
        
        private string _RoomNum;
        public string RoomNum { get => _RoomNum; set { _RoomNum = value; OnPropertyChanged(); } }
        private string _CustomerName;
        public string CustomerName { get => _CustomerName; set { _CustomerName = value; OnPropertyChanged(); } }
        private string _Sex;
        public string Sex { get => _Sex; set { _Sex = value; OnPropertyChanged(); } }
        private string _PhoneNum;
        public string PhoneNum { get => _PhoneNum; set { _PhoneNum = value; OnPropertyChanged(); } }
        private string _Email;
        public string Email { get => _Email; set { _Email = value; OnPropertyChanged(); } }
        private string _Nationality;
        public string Nationality { get => _Nationality; set { _Nationality = value; OnPropertyChanged(); } }
        private DateTime? _CheckInDate = null;
        public DateTime? CheckInDate { get => _CheckInDate; set { _CheckInDate = value; OnPropertyChanged(); } }
        private DateTime? _CheckInTime = null;
        public DateTime? CheckInTime { get => _CheckInTime; set { _CheckInTime = value; OnPropertyChanged(); } }
        private DateTime? _CheckOutDate = null;
        public DateTime? CheckOutDate { get => _CheckOutDate; set { _CheckOutDate = value; OnPropertyChanged(); } }
        private DateTime? _CheckOutTime = null;
        public DateTime? CheckOutTime { get => _CheckOutTime; set { _CheckOutTime = value; OnPropertyChanged(); } }
        private string _CCCD;
        public string CCCD
        {
            get => _CCCD;
            set
            {
                _CCCD = value;
                LoadSuggestionsList();
                var customer = Global.CustomersList.Where(x => x.CCCD == CCCD).FirstOrDefault();
                if (customer != null)
                {
                    if (CCCD != null)
                    {
                        CustomerName = customer.Name;
                        Sex = customer.Sex;
                        PhoneNum = customer.PhoneNumber;
                        Email = customer.Email;
                        Nationality = customer.Nationality;
                    }
                    else if (CCCD != null && customer == null)
                    {
                        CustomerName = Sex = PhoneNum = Email = Nationality = null;
                    }
                    else if (CCCD == "") { ClearFields(); }
                }
                OnPropertyChanged();
            }
        }
        private Room _SelectedReservation;
        public Room SelectedReservation
        {
            get => _SelectedReservation;
            set
            {
                _SelectedReservation = value;

                OnPropertyChanged();
            }
        }

        private Room _SelectedRoom;
        public Room SelectedRoom 
        { 
            get => _SelectedRoom;
            set 
            {
                _SelectedRoom = value;
                var temp = PendingReservationList.Where(x => x.Room == value).FirstOrDefault();
                if(_SelectedRoom != null)
                {
                    if(temp == null)
                    {
                        var reservation = new RoomsReservation();
                        reservation.IDRoom = SelectedRoom.ID;
                        reservation.CheckInTime = CheckInDate.Value.Date.Add(CheckInTime.Value.TimeOfDay);
                        reservation.CheckOutTime = CheckOutDate.Value.Date.Add(CheckOutTime.Value.TimeOfDay);
                    }
                    if(_SelectedRoom.Status != "Available")
                    {

                    }
                }
                OnPropertyChanged();
            } 
        }
     
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand ClearAllCommand { get; set; }
        public ICommand AddReservationCommand { get; set; }
        public ICommand CancelReservationCommand { get; set; }
        public ICommand CCCDSelectionChangedCommand { get; set; }
        public ICommand CCCDTextChangedCommand { get; set; }
        private bool windowShowed = false;
        public AddReservationWindow reservationWindow = new AddReservationWindow();

        public BookingViewModel()
        {
            LoadFilteredList();
            ClearAllCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                ClearFields();
            });
        }

       
        private void LoadFilteredList()
        {
            FilteredList = Global.RoomsList;
        }
        private void ClearFields()
        {
            RoomNum = CustomerName = Sex = PhoneNum = Email = CCCD = Nationality = null;
            CheckInDate = CheckOutDate = CheckInTime = CheckOutTime = null;
        }

        private bool Conflict()
        {
            var IncomingCheckInTime = CheckInDate.Value.Date.Add(CheckInTime.Value.TimeOfDay);
            var IncomingCheckOutTime = CheckOutDate.Value.Date.Add(CheckOutTime.Value.TimeOfDay);
            var temp = Global.ReservationsList.Where(x => x.Room.RoomNum == RoomNum && !(x.CheckOutTime <= IncomingCheckInTime || x.CheckInTime >= IncomingCheckInTime));
            if (temp.Count() != 0)
            {
                MessageBox.Show("Phòng đã được đặt trong khoảng thời gian này, vui lòng chọn phòng khác hoặc chỉnh sửa thời gian!", "Phòng đã được đặt!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return true;
            }
            return false;
        }
        private void LoadCustomersList()
        {
            Global.CustomersList = new ObservableCollection<Customer>();
            var customersList = DataProvider.Instance.DB.Customers.Where(x => x.Deleted == false);
            foreach (var item in customersList)
            {
                Global.CustomersList.Add(item);
            }
        }

        private void LoadSuggestionsList()
        {
            if(CCCD != null)
            {
                SuggestionsList = new ObservableCollection<string>();
                foreach (var item in Global.CustomersList)
                {
                    if (item.CCCD.StartsWith(CCCD))
                    {
                        SuggestionsList.Add(item.CCCD);
                    }
                }
            }
        }
        // chạy khi nhấn vào 1 phòng
        private void ShowAddReservationWindow()
        {
            if (!windowShowed)
            {
                reservationWindow.Show();
            }
        }
    }
}
