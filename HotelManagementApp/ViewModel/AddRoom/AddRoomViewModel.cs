using HotelManagementApp.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace HotelManagementApp.ViewModel
{
    public class AddRoomViewModel : BaseViewModel
    {
        private string _RoomNum;
        public string RoomNum { get => _RoomNum; set { _RoomNum = value; OnPropertyChanged(); } }

        private RoomType _RoomType;
        public RoomType RoomType { get => _RoomType; set { _RoomType = value; OnPropertyChanged(); } }

        private string _Status;
        public string Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }

        private string _ImageSource;
        public string ImageSource { get => _ImageSource; set { _ImageSource = value; OnPropertyChanged(); } }

        private BitmapImage _Image;
        public BitmapImage Image { get => _Image; set { _Image = value; OnPropertyChanged(); } }
        private string _SelectedImagePath;

        public ICommand AddRoomCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }

        public AddRoomViewModel()
        {
            AddRoomCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(RoomNum) || RoomType.Name == null || string.IsNullOrEmpty(Status) || RoomType == null)
                {
                    return false;
                }
                var list = DataProvider.Instance.DB.Rooms.Where(x => x.RoomNum == RoomNum);
                if (list == null || list.Count() != 0)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var roomType = DataProvider.Instance.DB.RoomTypes.Where(x => x.ID == RoomType.ID);
                var room = new Room()
                {
                    RoomNum = RoomNum,
                    IDRoomType = RoomType.ID,
                    Status = Status,
                };
                DataProvider.Instance.DB.Rooms.Add(room);
                DataProvider.Instance.DB.SaveChanges();
                addImage(room);
                DataProvider.Instance.DB.SaveChanges();
            });

            SelectImageCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                SelectImage();
            });
        }
        void SelectImage()
        {
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.Multiselect = false;
            OpenFile.Title = "Select Picture(s)";
            OpenFile.Filter = "ALL supported Graphics| *.jpeg; *.jpg;*.png;";
            if (OpenFile.ShowDialog() == true)
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = new FileStream(OpenFile.FileName, FileMode.Open, FileAccess.Read);
                img.EndInit();
                Image = img;
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
    }
}