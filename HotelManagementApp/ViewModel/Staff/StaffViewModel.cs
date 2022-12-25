using HotelManagementApp.Model;
using HotelManagementApp.View;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace HotelManagementApp.ViewModel
{
    public class StaffViewModel : BaseViewModel
    {
        private ObservableCollection<Staff> _FilteredList;
        public ObservableCollection<Staff> FilteredList { get => _FilteredList; set { _FilteredList = value; OnPropertyChanged(); } }


        private string _Filter;
        public string Filter { get => _Filter; set { _Filter = value; LoadFilteredList(); OnPropertyChanged(); } }
        private string _TypeFilter;
        public string TypeFilter { get => _TypeFilter; set { _TypeFilter = value; LoadFilteredList(); OnPropertyChanged(); } }
        private string _SearchString;
        public string SearchString { get => _SearchString; set { _SearchString = value; LoadFilteredList(); OnPropertyChanged(); } }

        private string _Name;
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }
        private string _Sex;
        public string Sex { get => _Sex; set { _Sex = value; OnPropertyChanged(); } }

        private string _CCCD;
        public string CCCD { get => _CCCD; set { _CCCD = value; OnPropertyChanged(); } }

        private string _PhoneNum;
        public string PhoneNum { get => _PhoneNum; set { _PhoneNum = value; OnPropertyChanged(); } }

        private string _Role;
        public string Role { get => _Role; set { _Role = value; OnPropertyChanged(); } }
        private string _ImageSource;
        public string ImageSource { get => _ImageSource; set { _ImageSource = value; OnPropertyChanged(); } }
        private string _Username;
        public string Username { get => _Username; set { _Username = value; OnPropertyChanged(); } }
        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }

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
                    var account = DataProvider.Instance.DB.Accounts.Where(x => x.IDStaff == SelectedItem.ID).FirstOrDefault();
                    if (account != null)
                    {
                        Username = account.Username;
                    }
                    else
                        Username = null;
                    LoadImage();
                }
                OnPropertyChanged();
            }
        }
        public ICommand addCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        public ICommand editCommand { get; set; }
        public ICommand deleteCommand { get; set; }
        public StaffViewModel()
        {
            addCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Sex) || string.IsNullOrEmpty(CCCD) || string.IsNullOrEmpty(PhoneNum) || Role == null || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    return false;
                }
                var list = DataProvider.Instance.DB.Staffs.Where(x => x.CCCD == CCCD && x.Deleted == false);
                if (list == null || list.Count() != 0)
                {
                    return false;
                }
                var accList = DataProvider.Instance.DB.Accounts.Where(x => x.Username == Username);
                if (accList == null || accList.Count() != 0)
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
                };
                DataProvider.Instance.DB.Staffs.Add(staff);
                DataProvider.Instance.DB.SaveChanges();
                var account = new Account()
                {
                    IDStaff = staff.ID,
                    Username = Username,
                    PasswordHash = MD5Hash(Base64Encode(Password)),
                };
                DataProvider.Instance.DB.Accounts.Add(account);
                addImage(staff);
                DataProvider.Instance.DB.SaveChanges();
                UpdateList(staff);
                ClearFields();
                SelectedItem = null;
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
                var list = DataProvider.Instance.DB.Staffs.Where(x => x.CCCD == CCCD && x.Deleted == false && x.ID != SelectedItem.ID);
                if (list == null || list.Count() != 0)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var staff = DataProvider.Instance.DB.Staffs.Where(x => x.ID == SelectedItem.ID).FirstOrDefault();
                staff.Name = Name;
                staff.Sex = Sex;
                staff.CCCD = CCCD;
                staff.ImageData = null;
                staff.ImageData = _SelectedImagePath;
                staff.Role = Role;
                addImage(staff);

                DataProvider.Instance.DB.SaveChanges();

                OnPropertyChanged();
                UpdateList(staff);
                ClearFields();
                SelectedItem = null;
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
                var staff = DataProvider.Instance.DB.Staffs.Where(x => x.ID == SelectedItem.ID).FirstOrDefault();
                staff.Deleted = true;

                UpdateList(staff, true);
                DataProvider.Instance.DB.SaveChanges();

                ClearFields();
                SelectedItem = null;
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
                EmployeeImage = LoadBitmapImage(OpenFile.FileName);
                _SelectedImagePath = OpenFile.FileName;
            }
            OnPropertyChanged();
        }

        void addImage(Staff staff)
        {
            string destinationDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            if (EmployeeImage != null)
            {
                staff.ImageData = $"\\ImageStorage\\StaffImg\\staff{staff.ID}.png";
                var destination = destinationDirectory + staff.ImageData;
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
                    EmployeeImage = LoadBitmapImage(destinationDirectory);
                }
            }
            else
            {
                EmployeeImage = null;
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
        private string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        private string Base64Encode(string input)
        {
            var textBytes = Encoding.UTF8.GetBytes(input);
            var base64String = Convert.ToBase64String(textBytes);
            return base64String;
        }

        private void LoadFilteredList()
        {

            ObservableCollection<Staff> list = new ObservableCollection<Staff>();
            foreach (var item in Global.StaffsList)
            {
                if (string.IsNullOrEmpty(Filter) && string.IsNullOrEmpty(SearchString) && string.IsNullOrEmpty(TypeFilter))
                {
                    list = Global.StaffsList;
                }
                else if (string.IsNullOrEmpty(Filter) && string.IsNullOrEmpty(SearchString) && !string.IsNullOrEmpty(TypeFilter))
                {
                    if (item.Role == TypeFilter)
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    switch (Filter)
                    {
                        case "ID":
                            if (string.IsNullOrEmpty(SearchString) || (item.ID == Convert.ToInt32(SearchString)) && (item.Role == TypeFilter || string.IsNullOrEmpty(TypeFilter)))
                            {
                                list.Add(item);
                            }
                            break;
                        case "Name":
                            if ((string.IsNullOrEmpty(SearchString) || item.Name.Contains(SearchString)) && (item.Role == TypeFilter || string.IsNullOrEmpty(TypeFilter)))
                            {
                                list.Add(item);
                            }
                            break;
                        case "Sex":
                            if ((string.IsNullOrEmpty(SearchString) || item.Sex.Contains(SearchString)) && (item.Role == TypeFilter || string.IsNullOrEmpty(TypeFilter)))
                            {
                                list.Add(item);
                            }
                            break;
                        case "CCCD":
                            if ((string.IsNullOrEmpty(SearchString) || item.CCCD.Contains(SearchString)) && (item.Role == TypeFilter || string.IsNullOrEmpty(TypeFilter)))
                            {
                                list.Add(item);
                            }
                            break;
                        case "Phone":
                            if ((string.IsNullOrEmpty(SearchString) || item.PhoneNumber.Contains(SearchString)) && (item.Role == TypeFilter || string.IsNullOrEmpty(TypeFilter)))
                            {
                                list.Add(item);
                            }
                            break;
                    }
                }
            }
            FilteredList = list;
        }
        private void UpdateList(Staff a, bool delete = false)
        {
            var staff = Global.StaffsList.Where(x => x.ID == a.ID).FirstOrDefault();
            if (delete)
            {
                HotelManagementApp.Global.StaffsList.Remove((Staff)staff);
            }
            else
            {
                if (staff == null)
                {
                    HotelManagementApp.Global.StaffsList.Add(a);
                }
                else
                {
                    staff = a;
                }
            }
            LoadFilteredList();
        }
        public void ClearFields()
        {
            Name = Sex = CCCD = PhoneNum = Role = Username = Password = null;
            EmployeeImage = null;
        }
    }
}