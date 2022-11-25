using HotelManagementApp.Model;
using HotelManagementApp.View;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace HotelManagementApp.ViewModel
{
    public class StaffViewModel : BaseViewModel
    {
        private ObservableCollection<Staff> _StaffsList;
        public ObservableCollection<Staff> StaffsList { get => _StaffsList; set { _StaffsList = value; OnPropertyChanged(); } }

        private string _Name;
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }
        private string _Sex;
        public string Sex { get => _Sex; set { _Sex = value; OnPropertyChanged(); } }

        private string _CCCD;
        public string CCCD { get => _CCCD; set { _CCCD = value; OnPropertyChanged(); } }

        private string _PhoneNum;
        public string PhoneNum { get => _PhoneNum; set { _PhoneNum = value; OnPropertyChanged(); } }

        private string  _Role;
        public string Role { get => _Role; set { _Role = value; OnPropertyChanged(); } }
        private string _ImageSource;
        public string ImageSource { get => _ImageSource; set { _ImageSource = value; OnPropertyChanged(); } }

        private BitmapImage _EmployeeImage;
        public BitmapImage EmployeeImage { get => _EmployeeImage; set { _EmployeeImage = value; OnPropertyChanged(); } }

        private string _SelectedImagePath;


        private Staff _SelectedItem;
        public Staff SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                if (SelectedItem != null)
                {
                    Name = SelectedItem.Name;
                    Sex = SelectedItem.Sex;
                    CCCD = SelectedItem.CCCD;
                    PhoneNum = SelectedItem.PhoneNumber;
                    Role = SelectedItem.Role;
                    ImageSource = SelectedItem.ImageData;
                    if (ImageSource != null)
                    {
                        BitmapImage newBitmapImage = new BitmapImage();
                        if(File.Exists(ImageSource))
                        {
                            newBitmapImage.BeginInit();

                            newBitmapImage.StreamSource = new FileStream(ImageSource, FileMode.Open, FileAccess.Read);
                            newBitmapImage.EndInit();
                            EmployeeImage = newBitmapImage;
                        }    
                    }
                    else
                    {
                        EmployeeImage = null;
                    }

                }
                OnPropertyChanged();
            }
        }

        public ICommand addCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        public ICommand editCommand { get; set; }
        public StaffViewModel()
        {
            LoadStaffsList();
            addCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Sex) || string.IsNullOrEmpty(CCCD) || string.IsNullOrEmpty(PhoneNum) || Role == null)
                {
                    return false;
                }
                var list = DataProvider.Instance.DB.Staffs.Where(x => x.CCCD == CCCD);
                if (list == null || list.Count() != 0)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var staff = new Staff()
                {
                    Name = Name,
                    CCCD = CCCD,
                    Sex = Sex,
                    PhoneNumber = PhoneNum,
                    Role = Role,
                    ImageData = (EmployeeImage == null) ? null : _SelectedImagePath
                };
                DataProvider.Instance.DB.Staffs.Add(staff);
                DataProvider.Instance.DB.SaveChanges();
                var newStaff = DataProvider.Instance.DB.Staffs.Where(x => x.ID == staff.ID).FirstOrDefault();
                addImage(newStaff);
                LoadStaffsList();
            });

            SelectImageCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                SelectImage();
            });

            editCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Sex) || string.IsNullOrEmpty(CCCD) || string.IsNullOrEmpty(PhoneNum) || Role == null || SelectedItem == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var staff = DataProvider.Instance.DB.Staffs.Where(x => x.ID == SelectedItem.ID).SingleOrDefault();
                staff.Name = Name;
                staff.Sex = Sex;
                staff.CCCD = CCCD;
                staff.ImageData = _SelectedImagePath;
                staff.Role = Role;
                addImage(staff);

                DataProvider.Instance.DB.SaveChanges();

                OnPropertyChanged();
                LoadStaffsList();
            });
        }
        void LoadStaffsList()
        {
            StaffsList = new ObservableCollection<Staff>();
            var staffsList = DataProvider.Instance.DB.Staffs;
            foreach (var item in staffsList)
            {
                StaffsList.Add(item);
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
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = new FileStream(OpenFile.FileName, FileMode.Open, FileAccess.Read);
                img.EndInit();
                EmployeeImage = img;
                _SelectedImagePath = OpenFile.FileName;
            }
            OnPropertyChanged();
        }

        void addImage(Staff staff)
        {
            string destinationDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            if(EmployeeImage != null)
            {
                ImageSource = destinationDirectory + $"\\ImageStorage\\StaffImg\\staff{staff.ID}.png";
                File.Copy(_SelectedImagePath, ImageSource);
            }
            else
            {
                ImageSource = null;
            }

            OnPropertyChanged();
        }
    }       
}

