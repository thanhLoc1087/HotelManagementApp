using HotelManagementApp.Model;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HotelManagementApp.ViewModel
{
    public class FoodViewModel : BaseViewModel
    {
        private ObservableCollection<FoodsAndService> _FilteredList;
        public ObservableCollection<FoodsAndService> FilteredList { get => _FilteredList; set { _FilteredList = value; OnPropertyChanged(); } }


        private string _Filter;
        public string Filter { get => _Filter; set { _Filter = value; LoadFilteredList(); OnPropertyChanged(); } }
        private string _TypeFilter;
        public string TypeFilter { get => _TypeFilter; set { _TypeFilter = value; LoadFilteredList(); OnPropertyChanged(); } }
        private string _SearchString;
        public string SearchString { get => _SearchString; set { _SearchString = value; LoadFilteredList(); OnPropertyChanged(); } }

        private string _Name;
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }

        private string _Unit;
        public string Unit { get => _Unit; set { _Unit = value; OnPropertyChanged(); } }

        private decimal? _Price;
        public decimal? Price { get => _Price; set { _Price = value; if (Price == 0) Price = null; OnPropertyChanged(); } }

        private string _Type;
        public string Type { get => _Type; set { _Type = value; OnPropertyChanged(); } }

        private string _ImageSource;
        public string ImageSource { get => _ImageSource; set { _ImageSource = value; OnPropertyChanged(); } }

        private string _SelectedImagePath;

        private BitmapImage _Image;
        public BitmapImage Image { get => _Image; set { _Image = value; OnPropertyChanged(); } }

        private FoodsAndService _SelectedItem;
        public FoodsAndService SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                if (SelectedItem != null)
                {
                    Name = SelectedItem.Name;
                    Unit = SelectedItem.Unit;
                    Price = SelectedItem.Price;
                    Type = SelectedItem.Type;
                    ImageSource = SelectedItem.ImageData;
                    LoadImage();
                }
                OnPropertyChanged();
            }
        }

        public ICommand addCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        public ICommand editCommand { get; set; }
        public ICommand deleteCommand { get; set; }
        public FoodViewModel()
        {
            addCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Unit) || Price == 0 || string.IsNullOrEmpty(Type))
                {
                    return false;
                }
                var list = DataProvider.Instance.DB.FoodsAndServices.Where(x => x.Name == Name && x.Deleted == false);
                if (list == null || list.Count() != 0)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var food = new FoodsAndService()
                {
                    Name = Name,
                    Unit = Unit,
                    Price = Price,
                    Type = Type
                };
                DataProvider.Instance.DB.FoodsAndServices.Add(food);
                DataProvider.Instance.DB.SaveChanges();
                addImage(food);
                DataProvider.Instance.DB.SaveChanges();
                UpdateList(food);
                ClearFields();
            });

            SelectImageCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                SelectImage();
            });

            editCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Unit) || Price == 0 || SelectedItem == null || string.IsNullOrEmpty(Type))
                {
                    return false;
                }
                var list = DataProvider.Instance.DB.FoodsAndServices.Where(x => x.Name == Name && x.Deleted == false && x.ID != SelectedItem.ID);
                if (list == null || list.Count() != 0)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var food = DataProvider.Instance.DB.FoodsAndServices.Where(x => x.ID == SelectedItem.ID).FirstOrDefault();
                food.Name = Name;
                food.Unit = Unit;
                food.Price = Price;
                food.Type = Type;
                food.ImageData = null;
                food.ImageData = _SelectedImagePath;
                addImage(food);

                DataProvider.Instance.DB.SaveChanges();
                UpdateList(food);
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
                var food = DataProvider.Instance.DB.FoodsAndServices.Where(x => x.ID == SelectedItem.ID).FirstOrDefault();
                food.Deleted = true;

                UpdateList(food, true);
                DataProvider.Instance.DB.SaveChanges();
                ClearFields();
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
                Image = LoadBitmapImage(OpenFile.FileName);
                _SelectedImagePath = OpenFile.FileName;
            }
            OnPropertyChanged();
        }

        void addImage(FoodsAndService food)
        {
            string destinationDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            if (Image != null)
            {
                food.ImageData = $"\\ImageStorage\\FoodImg\\food{food.ID}.png";
                var destination = destinationDirectory + food.ImageData;
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
                else
                {
                    Image = null;
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
            ObservableCollection<FoodsAndService> list = new ObservableCollection<FoodsAndService>();
            foreach (var item in Global.FoodsAndServicesList)
            {
                if (string.IsNullOrEmpty(Filter) && string.IsNullOrEmpty(SearchString) && string.IsNullOrEmpty(TypeFilter))
                {
                    list = Global.FoodsAndServicesList;
                }
                else if (string.IsNullOrEmpty(Filter) && string.IsNullOrEmpty(SearchString) && !string.IsNullOrEmpty(TypeFilter))
                {
                    if (item.Type == TypeFilter)
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    switch (Filter)
                    {
                        case "ID":
                            if (string.IsNullOrEmpty(SearchString) || (item.ID == Convert.ToInt32(SearchString)) && (item.Type == TypeFilter || string.IsNullOrEmpty(TypeFilter)))
                            {
                                list.Add(item);
                            }
                            break;
                        case "Name":
                            if ((string.IsNullOrEmpty(SearchString) || item.Name.Contains(SearchString)) && (item.Type == TypeFilter || string.IsNullOrEmpty(TypeFilter)))
                            {
                                list.Add(item);
                            }
                            break;
                        case "Unit":
                            if ((string.IsNullOrEmpty(SearchString) || item.Unit.Contains(SearchString)) && (item.Type == TypeFilter || string.IsNullOrEmpty(TypeFilter)))
                            {
                                list.Add(item);
                            }
                            break;
                        case "Price":
                            if ((string.IsNullOrEmpty(SearchString) || item.Price == Convert.ToDecimal(SearchString)) && (item.Type == TypeFilter || string.IsNullOrEmpty(TypeFilter)))
                            {
                                list.Add(item);
                            }
                            break;
                    }
                }
            }
            FilteredList = list;
        }
        private void UpdateList(FoodsAndService a, bool delete = false)
        {
            var food = Global.FoodsAndServicesList.Where(x => x.ID == a.ID).FirstOrDefault();
            if (delete)
            {
                HotelManagementApp.Global.FoodsAndServicesList.Remove((FoodsAndService)food);
            }
            else
            {
                if (food == null)
                {
                    HotelManagementApp.Global.FoodsAndServicesList.Add(a);
                }
                else
                {
                    food = a;
                }
            }
            LoadFilteredList();
        }
        public void ClearFields()
        {
            Name = Unit = Type = null;
            Price = null;
            Image = null;
        }
    }

}