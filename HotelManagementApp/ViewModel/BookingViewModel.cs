﻿using HotelManagementApp.Model;
using HotelManagementApp.View.Reservation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Schema;

namespace HotelManagementApp.ViewModel
{
    public class BookingViewModel : BaseViewModel
    {
        private ObservableCollection<Room> _FilteredList;
        public ObservableCollection<Room> FilteredList { get => _FilteredList; set { _FilteredList = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _SuggestionsList;
        public ObservableCollection<string> SuggestionsList { get => _SuggestionsList; set { _SuggestionsList = value; OnPropertyChanged(); } }
        private ObservableCollection<RoomsReservation> _PendingReservationsList;
        public ObservableCollection<RoomsReservation> PendingReservationsList { get => _PendingReservationsList; set { _PendingReservationsList = value; OnPropertyChanged(); } }
        private string _SearchString;
        public string SearchString { get => _SearchString; set { _SearchString = value; LoadFilteredList(); OnPropertyChanged(); } }
        private string _Sort;
        public string Sort { get => _Sort; set { _Sort = value; LoadFilteredList(); OnPropertyChanged(); } }
        private RoomType _TypeFilter;
        public RoomType TypeFilter { get => _TypeFilter; set { _TypeFilter = value; LoadFilteredList(); OnPropertyChanged(); } }
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
        public DateTime? CheckInDate { get => _CheckInDate; set { _CheckInDate = value; UpdateTotal(); LoadFilteredList(); OnPropertyChanged(); } }
        private DateTime? _CheckInTime = null;
        public DateTime? CheckInTime { get => _CheckInTime; set { _CheckInTime = value; UpdateTotal(); LoadFilteredList(); OnPropertyChanged(); } }
        private DateTime? _CheckOutDate = null;
        public DateTime? CheckOutDate { get => _CheckOutDate; set { _CheckOutDate = value; UpdateTotal(); LoadFilteredList(); OnPropertyChanged(); } }
        private DateTime? _CheckOutTime = null;
        public DateTime? CheckOutTime { get => _CheckOutTime; set { _CheckOutTime = value; UpdateTotal(); LoadFilteredList(); OnPropertyChanged(); } }
        private decimal? _Total = 0;
        public decimal? Total {get => _Total; set { _Total = value; OnPropertyChanged(); } }
        private string _CCCD = "";
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
                var temp = PendingReservationsList.Where(x => x.Room == value).FirstOrDefault();
                if(CheckInTime != null && CheckInDate != null && CheckOutDate != null && CheckOutTime != null)
                {
                    if (_SelectedRoom != null && (_SelectedRoom.Status == "Booked" || _SelectedRoom.Status == "Available"))
                    {
                        if (temp == null)
                        {
                            var reservation = new RoomsReservation();
                            reservation.Room = SelectedRoom;
                            reservation.Deleted = false;
                            PendingReservationsList.Add(reservation);
                            UpdateTotal();
                        }
                    }
                }
                
                OnPropertyChanged();
            } 
        }
        AddReservationWindow reservationWindow = new AddReservationWindow();


        public ICommand BookingCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand ClearAllCommand { get; set; }
        public ICommand AddReservationCommand { get; set; }
        public ICommand CancelReservationCommand { get; set; }
        public ICommand CCCDSelectionChangedCommand { get; set; }
        public ICommand CCCDTextChangedCommand { get; set; }
        public ICommand RemoveBtn { get; set; }
        private bool windowShowed = false;

        public BookingViewModel()
        {
            LoadFilteredList();
            PendingReservationsList = new ObservableCollection<RoomsReservation>();
            reservationWindow = new AddReservationWindow() { DataContext = this };
            ClearAllCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                ClearFields();
                Total = 0;
                PendingReservationsList.Clear();
            });
            BookingCommand = new RelayCommand<object>((p) =>
            {

                if (PendingReservationsList == null || PendingReservationsList.Count() ==0 || string.IsNullOrEmpty(CustomerName) || string.IsNullOrEmpty(PhoneNum) || string.IsNullOrEmpty(CCCD) || CheckInDate == null || CheckInTime == null || CheckOutTime == null || CheckOutDate == null)
                {
                    return false;
                }
                if (CheckInDate > CheckOutDate)
                {
                    return false;
                }
                if (CheckInDate == CheckOutDate && CheckInTime >= CheckOutTime)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var customer = new Customer();
                // If customer doesn't exist, create & save new customer, else update the existing one
                if (DataProvider.Instance.DB.Customers.Where(x => x.CCCD == CCCD).Count() == 0 || DataProvider.Instance.DB.Customers.Where(x => x.CCCD == CCCD) == null)
                {
                    customer = new Customer()
                    {
                        Name = CustomerName,
                        Sex = Sex,
                        CCCD = CCCD,
                        PhoneNumber = PhoneNum,
                        Email = Email,
                        Nationality = Nationality,
                    };
                    DataProvider.Instance.DB.Customers.Add(customer);
                    Global.CustomersList.Add(customer);
                    DataProvider.Instance.DB.SaveChanges();
                    ClearFields();
                    Total = 0;
                    PendingReservationsList.Clear();
                }
                else
                {
                    customer = DataProvider.Instance.DB.Customers.Where(x => x.CCCD == CCCD).FirstOrDefault();
                }

                // Create & save new bill detail
                var billDetail = new BillDetail()
                {
                    Staff = Const.ActiveAccount.Staff,
                    Customer = customer,
                    Deleted = false,
                    Status = "On-Going",
                    TotalMoney = Total,
                    BillDate = CheckInDate,
                };

                // Create & save new room reservation
                foreach (var item in PendingReservationsList)
                {
                    var room = DataProvider.Instance.DB.Rooms.Where(x => x.ID == item.Room.ID);
                    Global.RoomsList.Remove(item.Room);
                    item.Room.Status = "Booked";
                    Global.RoomsList.Add(item.Room);
                    item.BillDetail = billDetail;
                    item.CheckInTime = CheckInDate.Value.Date.Add(CheckInTime.Value.TimeOfDay);
                    item.CheckOutTime = CheckOutDate.Value.Date.Add(CheckOutTime.Value.TimeOfDay);
                    billDetail.TotalMoney = Total;
                    DataProvider.Instance.DB.RoomsReservations.Add(item);
                    Global.ReservationsList.Add(item);
                    Global.OnGoingReservationsList.Add(item);
                    Global.OnStaticPropertyChanged("ReservationsList");
                }
                Global.BillsList.Add(billDetail);
                DataProvider.Instance.DB.BillDetails.Add(billDetail);
                DataProvider.Instance.DB.SaveChanges();

                Total = 0;
                ClearFields();
                PendingReservationsList.Clear();
            });

            RemoveBtn = new RelayCommand<object>((p) => true, (p) =>
            {
                PendingReservationsList.Remove((RoomsReservation)p);
                UpdateTotal();
            });
        }
       
        void LoadFilteredList()
        {
            FilteredList = Global.RoomsList;
            ObservableCollection<Room> list = new ObservableCollection<Room>();
            ObservableCollection<Room> availables = new ObservableCollection<Room>();
            if (CheckInTime != null && CheckInDate != null && CheckOutDate != null && CheckOutTime != null)
            {
                availables = LoadAvailableRoomsList();
            }
            foreach (var item in Global.RoomsList)
            {
                if (string.IsNullOrEmpty(Sort) && string.IsNullOrEmpty(SearchString) && (TypeFilter == null || TypeFilter.Name ==null))
                {
                    if (availables.Count() == 0)
                    {
                        list = Global.RoomsList;
                    }
                    else
                    {
                        list = availables;
                    }
                }
                else if (string.IsNullOrEmpty(Sort) && string.IsNullOrEmpty(SearchString) && (TypeFilter != null))
                {
                    if (item.RoomType.Name == TypeFilter.Name)
                    {
                        list.Add(item);
                    }
                }
                else if ((string.IsNullOrEmpty(SearchString) || item.RoomNum.Contains(SearchString)) && (TypeFilter == null || TypeFilter.Name == null || item.RoomType.Name == TypeFilter.Name))
                {
                    list.Add(item);
                }
            }
            if (Sort == "Descending")
            {
                ObservableCollection<Room> temp;
                temp = new ObservableCollection<Room>(list.OrderByDescending(x => x.RoomType.Price));
                list.Clear();
                foreach (var item in temp) list.Add(item);
            }
            else if (Sort == "Ascending")
            {
                ObservableCollection<Room> temp;
                temp = new ObservableCollection<Room>(list.OrderBy(x => x.RoomType.Price));
                list.Clear();
                foreach (var item in temp) list.Add(item);
            }
            FilteredList = list;
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

        private ObservableCollection<Room> LoadAvailableRoomsList()
        {
            ObservableCollection<Room> list = new ObservableCollection<Room>();
            var IncomingCheckInTime = CheckInDate.Value.Date.Add(CheckInTime.Value.TimeOfDay);
            var IncomingCheckOutTime = CheckOutDate.Value.Date.Add(CheckOutTime.Value.TimeOfDay);
            foreach (var item in Global.RoomsList)
            {
                var temp = item.RoomsReservations.Where(x => !(x.CheckOutTime <= IncomingCheckInTime || x.CheckInTime >= IncomingCheckOutTime) && x.Deleted == false && x.BillDetail.Status == "On-Going");
                if (temp == null || temp.Count() == 0)
                {
                    list.Add(item);                        
                }
            }
            return list;
        }
        private void UpdateTotal()
        {
            if (CheckInDate == null || CheckInTime == null || CheckOutDate == null || CheckOutTime == null)
            {
                return;
            }
            Total = 0;
            foreach(var item in PendingReservationsList)
            {
                var IncomingCheckInTime = CheckInDate.Value.Date.Add(CheckInTime.Value.TimeOfDay);
                var IncomingCheckOutTime = CheckOutDate.Value.Date.Add(CheckOutTime.Value.TimeOfDay);
                var timespan = IncomingCheckOutTime.Subtract(IncomingCheckInTime).TotalDays;
                if (timespan <= 1)
                {
                    Total += item.Room.RoomType.Price;
                }
                else
                {
                    Total += item.Room.RoomType.Price * (int)timespan;
                }
            }
        }
    }
}
