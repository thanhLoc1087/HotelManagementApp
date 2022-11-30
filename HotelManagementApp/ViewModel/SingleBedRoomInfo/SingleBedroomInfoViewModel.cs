using HotelManagementApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xaml;
using System.Xml.Linq;

namespace HotelManagementApp.ViewModel
{
    public class SingleBedroomInfoViewModel : BaseViewModel
    {
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
        public string CCCD { get => _CCCD; set { _CCCD = value; OnPropertyChanged(); } }
        private string _Nationality;
        public string Nationality { get => _Nationality; set { _Nationality = value; OnPropertyChanged(); } }
        private DateTime? _CheckInDate = null;
        public DateTime? CheckInDate { get => _CheckInDate; set { _CheckInDate = value; OnPropertyChanged(); } }
        private DateTime? _CheckInTime = null;
        public DateTime? CheckInTime { get => _CheckInTime; set { _CheckInTime = value; OnPropertyChanged(); } }
        private DateTime? _CheckOutDate = null;
        public DateTime? CheckOutDate { get => _CheckOutDate; set { _CheckOutDate = value; OnPropertyChanged(); } }
        private DateTime? _CheckOutTime = null ;
        public DateTime? CheckOutTime { get => _CheckOutTime; set { _CheckOutTime = value; OnPropertyChanged(); } }
        private DateTime? _BillDate;
        public DateTime? BillDate { get => _BillDate; set { _BillDate = value; OnPropertyChanged(); } }
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public SingleBedroomInfoViewModel()
        {
            AddCommand  = new RelayCommand<object>((p) =>
            {
                var room = DataProvider.Instance.DB.Rooms.Where(x => x.RoomNum == RoomNum).FirstOrDefault();
                if (string.IsNullOrEmpty(RoomNum) || string.IsNullOrEmpty(CustomerName) || string.IsNullOrEmpty(PhoneNum) || string.IsNullOrEmpty(CCCD) || CheckInDate == null || CheckInTime == null || CheckOutDate == null || CheckOutDate == null)
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

                customer = DataProvider.Instance.DB.Customers.Where(x => x.CCCD == customer.CCCD).FirstOrDefault();
                // Create & save new bill detail
                var billDetail = new BillDetail()
                {
                    IDStaff = 1,
                    IDCustomer = customer.ID,
                    BillDate = BillDate
                };

                DataProvider.Instance.DB.BillDetails.Add(billDetail);
                DataProvider.Instance.DB.SaveChanges();
                billDetail = DataProvider.Instance.DB.BillDetails.Where(x => x.IDCustomer == customer.ID && x.BillDate == BillDate).FirstOrDefault();

                // Create & save new room reservation
                var room = DataProvider.Instance.DB.Rooms.Where(x => x.RoomNum == RoomNum).FirstOrDefault();

                var roomReservation = new RoomsReservation()
                {
                    IDBillDetail = billDetail.ID,
                    IDRoom = room.ID,
                    CheckInTime = CheckInDate.Value.Date.Add(CheckInTime.Value.TimeOfDay),
                    CheckOutTime = CheckOutDate.Value.Date.Add(CheckOutTime.Value.TimeOfDay),
                };
                DataProvider.Instance.DB.RoomsReservations.Add(roomReservation);
                DataProvider.Instance.DB.SaveChanges();

                //DataProvider.Instance.DB.RoomsReservations.Add(roomReservation);
                DataProvider.Instance.DB.SaveChanges();
                //LoadStaffsList();
            });

            DeleteCommand  = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {

            });

            ClearCommand  = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                RoomNum = CustomerName = Sex = PhoneNum = Email = CCCD = Nationality = null;
                CheckInDate = CheckOutDate = CheckInTime = CheckOutTime = BillDate = null;
            });
        }
    }
}
