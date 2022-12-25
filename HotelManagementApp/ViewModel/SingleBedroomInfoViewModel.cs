using HotelManagementApp.Model;
using HotelManagementApp.View.Reservation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Media;
using System.Xaml;
using System.Xml.Linq;

namespace HotelManagementApp.ViewModel
{
    public class SingleBedroomInfoViewModel : BaseViewModel
    {
        /// List phòng để hiển thị
        private ObservableCollection<Room> _RoomsList;
        public ObservableCollection<Room> RoomsList { get => _RoomsList; set { _RoomsList = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _SuggestionsList;
        public ObservableCollection<string> SuggestionsList { get => _SuggestionsList; set { _SuggestionsList = value; OnPropertyChanged(); } }
        private ObservableCollection<Customer> _CustomersList;
        public  ObservableCollection<Customer> CustomersList { get => _CustomersList; set { _CustomersList = value; OnPropertyChanged(); } }
        private Room _SelectedRoom;
        public Room SelectedRoom { get => _SelectedRoom; set { _SelectedRoom = value; OnPropertyChanged(); } }
        private ObservableCollection<RoomsReservation> _RoomReservationsList;
        public ObservableCollection<RoomsReservation> RoomReservationList { get => _RoomReservationsList; set { _RoomReservationsList = value; OnPropertyChanged(); } }
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
        private string _CCCD;
        public string CCCD { 
            get => _CCCD;
            set 
            {
                _CCCD = value;
                LoadSuggestionsList();
                var customer = CustomersList.Where(x => x.CCCD == CCCD).FirstOrDefault();
                if (CCCD != null && customer != null)
                {
                    CustomerName = customer.Name;
                    Sex = customer.Sex;
                    PhoneNum = customer.PhoneNumber;
                    Email = customer.Email;
                    Nationality = customer.Nationality;
                }
                else
                {
                    CustomerName = Sex = PhoneNum = Email = Nationality = null;
                }
                OnPropertyChanged();
            }
        }
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
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand AddReservationCommand { get; set; }
        public ICommand CancelReservationCommand { get; set; }
        public ICommand CCCDSelectionChangedCommand { get; set; }
        public ICommand CCCDTextChangedCommand { get; set; }
        private bool windowShowed = false;
        private RoomsReservation _SelectedItem;
        public AddReservationWindow reservationWindow = new AddReservationWindow();
        public RoomsReservation SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                if (SelectedItem != null)
                {
                    RoomNum = SelectedItem.Room.RoomNum;
                    CustomerName = SelectedItem.BillDetail.Customer.Name;
                    Sex = SelectedItem.BillDetail.Customer.Sex;
                    PhoneNum = SelectedItem.BillDetail.Customer.PhoneNumber;
                    Email = SelectedItem.BillDetail.Customer.Email;
                    CCCD = SelectedItem.BillDetail.Customer.CCCD;
                    Nationality = SelectedItem.BillDetail.Customer.Nationality;
                    string temp;
                    temp = SelectedItem.CheckInTime.Value.ToString("HH:mm");
                    CheckInTime = DateTime.Parse(temp);
                    temp = SelectedItem.CheckInTime.Value.ToShortDateString();
                    CheckInDate = DateTime.Parse(temp);
                    temp = SelectedItem.CheckOutTime.Value.ToString("HH:mm");
                    CheckOutTime = DateTime.Parse(temp);
                    temp = SelectedItem.CheckOutTime.Value.ToShortDateString();
                    CheckOutDate = DateTime.Parse(temp);
                }
                OnPropertyChanged();
            }
        }

        public SingleBedroomInfoViewModel()
        {
            LoadRoomReservationsList();
            LoadCustomersList();
            AddCommand = new RelayCommand<object>((p) =>
            {
                var room = DataProvider.Instance.DB.Rooms.Where(x => x.RoomNum == RoomNum).FirstOrDefault();
                if (string.IsNullOrEmpty(RoomNum) || string.IsNullOrEmpty(CustomerName) || string.IsNullOrEmpty(PhoneNum) || string.IsNullOrEmpty(CCCD) || CheckInDate == null || CheckInTime == null || CheckInDate == null || CheckOutDate == null)
                {
                    return false;
                }
                if (CheckInDate > CheckOutDate)
                {
                    return false;
                }
                if (CheckInDate == CheckOutDate && CheckInTime > CheckOutTime)
                {
                    return false;
                }

                if (room == null)
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
                    DataProvider.Instance.DB.SaveChanges();
                }
                else
                {
                    customer = DataProvider.Instance.DB.Customers.Where(x => x.CCCD == CCCD).FirstOrDefault();
                }

                // Create & save new bill detail
                var billDetail = new BillDetail()
                {
                    IDStaff = (int)Const.ActiveAccount.IDStaff,
                    IDCustomer = customer.ID,
                    Status = "On-Going",
                };

                DataProvider.Instance.DB.BillDetails.Add(billDetail);
                DataProvider.Instance.DB.SaveChanges();

                // Create & save new room reservation
                foreach (var item in RoomReservationList)
                {
                    item.BillDetail = billDetail;
                    DataProvider.Instance.DB.RoomsReservations.Add(item);
                }
                DataProvider.Instance.DB.SaveChanges();
                //LoadRoomReservationsList();
                //ClearFields();
                SelectedItem = null;
            });
            EditCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(RoomNum) || string.IsNullOrEmpty(CustomerName) || string.IsNullOrEmpty(PhoneNum) || string.IsNullOrEmpty(CCCD) || CheckInDate == null || CheckInTime == null || CheckOutDate == null || CheckOutDate == null || SelectedItem == null)
                {
                    return false;
                }
                if (CheckInDate > CheckOutDate || CheckInTime >= CheckOutTime)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var RoomReservation = DataProvider.Instance.DB.RoomsReservations.Where(x => x.ID == SelectedItem.ID).FirstOrDefault();
                var room = DataProvider.Instance.DB.Rooms.Where(x => x.RoomNum == RoomNum).FirstOrDefault();
                RoomReservation.IDRoom = room.ID;
                RoomReservation.CheckInTime = CheckInDate.Value.Date.Add(CheckInTime.Value.TimeOfDay);
                RoomReservation.CheckOutTime = CheckOutDate.Value.Date.Add(CheckOutTime.Value.TimeOfDay);
                RoomReservation.BillDetail.Customer.Name = CustomerName;
                RoomReservation.BillDetail.Customer.Sex = Sex;
                RoomReservation.BillDetail.Customer.CCCD = CCCD;
                RoomReservation.BillDetail.Customer.PhoneNumber = PhoneNum;
                RoomReservation.BillDetail.Customer.Email = Email;
                RoomReservation.BillDetail.Customer.Nationality = Nationality;

                DataProvider.Instance.DB.SaveChanges();

                OnPropertyChanged();
                LoadRoomReservationsList();
                ClearFields();
                SelectedItem = null;
            });


            DeleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                SelectedItem = null;
            });

            ClearCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                ClearFields();
                ShowAddReservationWindow();
            });

            CCCDSelectionChangedCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                LoadSuggestionsList();
            });
            CCCDTextChangedCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                LoadSuggestionsList();
                var customer = CustomersList.Where(x => x.CCCD == CCCD).FirstOrDefault();
                if (CCCD != null && customer != null)
                {
                    CustomerName = customer.Name;
                    Sex = customer.Sex;
                    PhoneNum = customer.PhoneNumber;
                    Email = customer.Email;
                    Nationality = customer.Nationality;
                }
                else
                {
                    CustomerName = Sex = PhoneNum = Email = Nationality = null;
                }
            });

            AddReservationCommand = new RelayCommand<object>((p) =>
            {
                var room = DataProvider.Instance.DB.Rooms.Where(x => x.ID == SelectedRoom.ID).FirstOrDefault();
                if (CheckInDate == null || CheckInTime == null || CheckInDate == null || CheckOutDate == null)
                {
                    return false;
                }
                if (CheckInDate > CheckOutDate)
                {
                    return false;
                }
                if (CheckInDate == CheckOutDate && CheckInTime > CheckOutTime)
                {
                    return false;
                }

                if (room == null)
                {
                    return false;
                }

                return true;
            }, (p) =>
            {
                if (!Conflict())
                {
                    var room = SelectedRoom;
                    var roomReservation = new RoomsReservation()
                    {
                        IDRoom = room.ID,
                        CheckInTime = CheckInDate.Value.Date.Add(CheckInTime.Value.TimeOfDay),
                        CheckOutTime = CheckOutDate.Value.Date.Add(CheckOutTime.Value.TimeOfDay),
                    };
                    RoomReservationList.Add(roomReservation);
                }
            });
            CancelReservationCommand = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                reservationWindow.Close();
            });
        }

        private void LoadRoomReservationsList()
        {
            RoomReservationList = new ObservableCollection<RoomsReservation>();
            var ReservationList = DataProvider.Instance.DB.RoomsReservations.Where(x => x.BillDetail.Status == "On-Going");
            foreach (var item in ReservationList)
            {
                RoomReservationList.Add(item);
            }
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
            var temp = RoomReservationList.Where(x => x.Room.RoomNum == RoomNum && !(x.CheckOutTime <= IncomingCheckInTime || x.CheckInTime >= IncomingCheckInTime));
            if (temp.Count() != 0)
            {
                MessageBox.Show("Phòng đã được đặt trong khoảng thời gian này, vui lòng chọn phòng khác hoặc chỉnh sửa thời gian!", "Phòng đã được đặt!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return true;
            }
            return false;
        }
        private void LoadCustomersList()
        {
            CustomersList = new ObservableCollection<Customer>();
            var customersList = DataProvider.Instance.DB.Customers.Where(x => x.Deleted == false);
            foreach(var item in customersList)
            {
                CustomersList.Add(item);
            }
        }

        private void LoadSuggestionsList()
        {
            SuggestionsList = new ObservableCollection<string>();
            foreach(var item in CustomersList)
            {
                if(item.CCCD.StartsWith(CCCD))
                {
                    SuggestionsList.Add(item.CCCD);
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