using HotelManagementApp.Model;
using HotelManagementApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HotelManagementApp.ViewModel
{
    class BillReportViewModel : BaseViewModel
    {
        private int _BookingID;
        public int Booking_Id { get => _BookingID; set { _BookingID = value; OnPropertyChanged(); } }
        private string _CustomerName;
        public string CustomerName { get => _CustomerName; set { _CustomerName = value; OnPropertyChanged(); } }
        private string _PhoneNum;
        public string CustomerPhoneNumber { get => _PhoneNum; set { _PhoneNum = value; OnPropertyChanged(); } }
        private DateTime? _StartTime = null;
        public DateTime? CheckInTime { get => _StartTime; set { _StartTime = value; OnPropertyChanged(); } }
        private DateTime? _DateBooking = null;
        public DateTime? DateBooking { get => _DateBooking; set { _DateBooking = value; OnPropertyChanged(); } }
        private DateTime? _EndTime = null;
        public DateTime? CheckOutTime { get => _EndTime; set { _EndTime = value; OnPropertyChanged(); } }
        private double _GoodTotal;
        public double Good_total { get => _GoodTotal; set { _GoodTotal = value; OnPropertyChanged(); } }
        private double _RoomPrice;
        public double Room_price { get => _RoomPrice; set { _RoomPrice = value; OnPropertyChanged(); } }
        private string _TotalPayMent;
        public string Total_Payment { get => _TotalPayMent; set { _TotalPayMent = value; OnPropertyChanged(); } }


        public BillReportViewModel()
        {

        }
    }
}
