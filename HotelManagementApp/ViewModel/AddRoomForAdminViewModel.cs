using HotelManagementApp.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace HotelManagementApp.ViewModel
{
    public class AddRoomForAdminViewModel : BaseViewModel
    {
        private ObservableCollection<Room> _RoomsList;
        public ObservableCollection<Room> RoomsList { get => _RoomsList; set { _RoomsList = value; OnPropertyChanged(); } }
        private ObservableCollection<Room> _FilteredList;
        public ObservableCollection<Room> FilteredList { get => _FilteredList; set { _FilteredList = value; OnPropertyChanged(); } }

        private int _FilterNum;
        public int FilterNum { get => _FilterNum; set { _FilterNum = value; LoadFilteredList(); OnPropertyChanged(); } }
        private string _Filter;
        public string Filter { get => _Filter; set { _Filter = value; LoadFilteredList(); OnPropertyChanged(); } }
        private string _TypeFilter;
        public string TypeFilter { get => _TypeFilter; set { _TypeFilter = value; LoadFilteredList(); OnPropertyChanged(); } }
        private string _SearchString;
        public string SearchString { get => _SearchString; set { _SearchString = value; LoadFilteredList(); OnPropertyChanged(); } }
        private string _RoomNum;
        public string RoomNum { get => _RoomNum; set { _RoomNum = value; OnPropertyChanged(); } }
        private string _Type;
        public string Type { get => _Type; set { _Type = value; OnPropertyChanged(); } }
        private string _Status;
        public string Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }
        private string _ImageSource;
        public string ImageSource { get => _ImageSource; set { _ImageSource = value; OnPropertyChanged(); } }
        private Room _SelectedItem;
        public Room SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                if (SelectedItem != null)
                {
                    RoomNum = SelectedItem.RoomNum;
                    //RoomType = SelectedItem.RoomType;
                    Status = SelectedItem.Status;
                    ImageSource = SelectedItem.ImageData;
                    LoadImage();
                }
                OnPropertyChanged();
            }
        }
        private string _SelectedImagePath;
        private BitmapImage _Image;
        public BitmapImage Image { get => _Image; set { _Image = value; OnPropertyChanged(); } }
        public ICommand addCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        public ICommand editCommand { get; set; }
        public ICommand deleteCommand { get; set; }

        public AddRoomForAdminViewModel()
        {
            LoadRoomsList();
            LoadFilteredList();
            addCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(RoomNum) || string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(Status))
                {
                    return false;
                }
                var list = DataProvider.Instance.DB.Rooms.Where(x => x.RoomNum== RoomNum);
                if (list == null || list.Count() != 0)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var room = new Room()
                {
                    RoomNum = RoomNum,
                    //RoomType = Type,
                    Status = Status,
                };
                DataProvider.Instance.DB.Rooms.Add(room);
                DataProvider.Instance.DB.SaveChanges();
                addImage(room);
                DataProvider.Instance.DB.SaveChanges();
                LoadRoomsList();
                ClearFields();
            });

            SelectImageCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                SelectImage();
            });

            editCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(RoomNum) || string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(Status) || SelectedItem == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var room = DataProvider.Instance.DB.Rooms.Where(x => x.ID == SelectedItem.ID).FirstOrDefault();
                room.RoomNum = RoomNum;
                // room.RoomType = Type;
                room.Status = Status;
                room.ImageData = _SelectedImagePath;
                addImage(room);

                DataProvider.Instance.DB.SaveChanges();

                OnPropertyChanged();
                LoadRoomsList();
                ClearFields();
            });
            deleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var room = DataProvider.Instance.DB.Rooms.Where(x => x.ID == SelectedItem.ID).FirstOrDefault();
                room.Deleted = true;

                DataProvider.Instance.DB.SaveChanges();

                OnPropertyChanged();
                LoadRoomsList();
                ClearFields();
            });

        }
        void LoadRoomsList()
        {
            RoomsList = new ObservableCollection<Room>();
            var roomsList = DataProvider.Instance.DB.Rooms.Where(x => x.Deleted == false);
            foreach (var item in RoomsList)
            {
                RoomsList.Add(item);
            }
        }

        void SelectImage()
        {
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.Multiselect = false;
            OpenFile.Title = "Select Picture(s)";
            OpenFile.Filter = "ALL supported Graphics| *.jpeg; *.jpg;*.png;";
            if (OpenFile.ShowDialog() == true)
            {
                Image = LoadBitmapImage(OpenFile.FileName);
                _SelectedImagePath = OpenFile.FileName;
            }
            OnPropertyChanged();
        }

        void addImage(Room room)
        {
            string destinationDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            if (Image != null)
            {
                room.ImageData = $"\\ImageStorage\\RoomImg\\room{room.RoomNum}.png";
                var destination = destinationDirectory + room.ImageData;
                if (_SelectedImagePath == null)
                    return;
                File.Copy(_SelectedImagePath, destination, true);
            }
            else
            {
                ImageSource = null;
            }
            OnPropertyChanged();
        }
        private void LoadImage()
        {
            if (ImageSource != null)
            {
                string destinationDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + ImageSource;
                if (File.Exists(destinationDirectory))
                {
                    Image = LoadBitmapImage(destinationDirectory);
                }
            }
            else
            {
                Image = null;
            }
        }
        public static BitmapImage LoadBitmapImage(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }
        private void LoadFilteredList()
        {

        //    ObservableCollection<Room> list = new ObservableCollection<Room>();
        //    foreach (var item in RoomsList)
        //    {
        //        if (string.IsNullOrEmpty(Filter) && string.IsNullOrEmpty(SearchString) && string.IsNullOrEmpty(TypeFilter))
        //        {
        //            list = RoomsList;
        //        }
        //        else if (string.IsNullOrEmpty(Filter) && string.IsNullOrEmpty(SearchString) && !string.IsNullOrEmpty(TypeFilter))
        //        {
        //            //if (item.RoomType == TypeFilter)
        //            //{
        //            //    list.Add(item);
        //            //}
        //        }
        //        else
        //        {
        //            switch (Filter)
        //            {
        //                case "ID":
        //                    if (string.IsNullOrEmpty(SearchString) || (item.ID == Convert.ToInt32(SearchString)) && (item.Role == TypeFilter || string.IsNullOrEmpty(TypeFilter)))
        //                    {
        //                        list.Add(item);
        //                    }
        //                    break;
        //                case "Room Num":
        //                    if ((item.RoomNum == SearchString || string.IsNullOrEmpty(SearchString)) && (item.RoomNum == TypeFilter || string.IsNullOrEmpty(TypeFilter)))
        //                    {
        //                        list.Add(item);
        //                    }
        //                    break;
        //                case "Room Type":
        //                    if ((item.== SearchString || string.IsNullOrEmpty(SearchString)) && (item.Role == TypeFilter || string.IsNullOrEmpty(TypeFilter)))
        //                    {
        //                        list.Add(item);
        //                    }
        //                    break;
        //                case "Status":
        //                    if ((string.IsNullOrEmpty(SearchString) ||item.CCCD == SearchString) && (item.Role == TypeFilter || string.IsNullOrEmpty(TypeFilter)))
        //                    {
        //                        list.Add(item);
        //                    }
        //                    break;
        //                case "Phone":
        //                    if ((string.IsNullOrEmpty(SearchString) || item.PhoneNumber == SearchString) && (item.Role == TypeFilter || string.IsNullOrEmpty(TypeFilter)))
        //                    {
        //                        list.Add(item);
        //                    }
        //                    break;
        //            }
        //        }
        //    }
        //    FilteredList = list;
        }
        public void ClearFields()
        {
            RoomNum = Type = Status = null;
            Image = null;
        }
    }
}
