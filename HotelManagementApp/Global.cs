using HotelManagementApp.Model;
using HotelManagementApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApp
{
    public class Global : BaseViewModel
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void OnStaticPropertyChanged([CallerMemberName] string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
        private static ObservableCollection<RoomType> _Types;
        public static ObservableCollection<RoomType> Types
        {
            get
            {
                if (_Types == null)
                {
                    LoadRoomTypesList();
                }
                return _Types;
            }
            set
            {
                _Types = value;
                OnStaticPropertyChanged(nameof(Types));
            }
        }

        private static ObservableCollection<BillDetail> _BillsList;
        public static ObservableCollection<BillDetail> BillsList
        {
            get
            {
                if (_BillsList == null)
                {
                    LoadBillsList();
                }
                return _BillsList;
            }
            set
            {
                _BillsList = value;
                OnStaticPropertyChanged(nameof(BillsList));
            }
        }
        private static ObservableCollection<Customer> _CustomersList;
        public static ObservableCollection<Customer> CustomersList
        {
            get
            {
                if (_CustomersList == null)
                {
                    LoadCustomersList();
                }
                return _CustomersList;
            }
            set
            {
                _CustomersList = value;
                OnStaticPropertyChanged(nameof(CustomersList));
            }
        }
        private static ObservableCollection<FoodsAndService> _FoodsAndServicesList;
        public static ObservableCollection<FoodsAndService> FoodsAndServicesList
        {
            get
            {
                if (_FoodsAndServicesList == null)
                {
                    LoadFoodsAndServicesList();
                }
                return _FoodsAndServicesList;
            }
            set
            {
                _FoodsAndServicesList = value;
                OnStaticPropertyChanged(nameof(FoodsAndServicesList));
            }
        }
        private static ObservableCollection<Order> _OrdersList;
        public static ObservableCollection<Order> OrdersList
        {
            get
            {
                if (_OrdersList == null)
                {
                    LoadOrdersList();
                }
                return _OrdersList;
            }
            set
            {
                _OrdersList = value;
                OnStaticPropertyChanged(nameof(OrdersList));
            }
        }
        private static ObservableCollection<Room> _RoomsList;
        public static ObservableCollection<Room> RoomsList
        {
            get
            {
                if (_RoomsList == null)
                {
                    LoadRoomsList();
                }
                return _RoomsList;
            }
            set
            {
                _RoomsList = value;
                OnStaticPropertyChanged(nameof(RoomsList));
            }
        }
        private static ObservableCollection<Staff> _StaffsList;
        public static ObservableCollection<Staff> StaffsList
        {
            get
            {
                if (_StaffsList == null)
                {
                    LoadStaffsList();
                }
                return _StaffsList;
            }
            set
            {
                _StaffsList = value;
                OnStaticPropertyChanged(nameof(StaffsList));
            }
        }
        private static ObservableCollection<RoomsReservation> _ReservationsList;
        public static ObservableCollection<RoomsReservation> ReservationsList
        {
            get
            {
                if (_ReservationsList == null)
                {
                    LoadReservationsList();
                }
                return _ReservationsList;
            }
            set
            {
                _ReservationsList = value;
                OnStaticPropertyChanged(nameof(ReservationsList));
            }
        }
        private static void LoadRoomTypesList()
        {
            _Types = new ObservableCollection<RoomType>();
            _Types.Add(new RoomType() { Name = null });
            var list = DataProvider.Instance.DB.RoomTypes.Where(x => x.Deleted == false);
            foreach (var item in list)
            {
                _Types.Add(item);
            }
        }
        private static void LoadBillsList()
        {
            _BillsList = new ObservableCollection<BillDetail>();
            var list = DataProvider.Instance.DB.BillDetails.Where(x => x.Deleted == false);
            foreach (var item in list)
            {
                _BillsList.Add(item);
            }
        }

        private static void LoadCustomersList()
        {
            _CustomersList = new ObservableCollection<Customer>();
            var list = DataProvider.Instance.DB.Customers.Where(x => x.Deleted == false);
            foreach (var item in list)
            {
                _CustomersList.Add(item);
            }
        }
        private static void LoadFoodsAndServicesList()
        {
            _FoodsAndServicesList = new ObservableCollection<FoodsAndService>();
            var list = DataProvider.Instance.DB.FoodsAndServices.Where(x => x.Deleted == false);
            foreach (var item in list)
            {
                _FoodsAndServicesList.Add(item);
            }
        }
        private static void LoadOrdersList()
        {
            _OrdersList = new ObservableCollection<Order>();
            var list = DataProvider.Instance.DB.Orders.Where(x => x.Deleted == false);
            foreach (var item in list)
            {
                _OrdersList.Add(item);
            }
        }
        private static void LoadRoomsList()
        {
            _RoomsList = new ObservableCollection<Room>();
            var list = DataProvider.Instance.DB.Rooms.Where(x => x.Deleted == false);
            foreach (var item in list)
            {
                _RoomsList.Add(item);
            }
        }
        private static void LoadStaffsList()
        {
            _StaffsList = new ObservableCollection<Staff>();
            var list = DataProvider.Instance.DB.Staffs.Where(x => x.Deleted == false);
            foreach (var item in list)
            {
                _StaffsList.Add(item);
            }
        }
        private static void LoadReservationsList()
        {
            _ReservationsList = new ObservableCollection<RoomsReservation>();
            var list = DataProvider.Instance.DB.RoomsReservations.Where(x => x.Deleted == false);
            foreach (var item in list)
            {
                _ReservationsList.Add(item);
            }
        }
    }
}
