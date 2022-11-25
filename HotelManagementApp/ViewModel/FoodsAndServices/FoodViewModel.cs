using HotelManagementApp.Model;
using HotelManagementApp.View;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace HotelManagementApp.ViewModel
{
    public class FoodViewModel : BaseViewModel
    {
        private ObservableCollection<FoodsAndService> _FoodsAndServicesList;
        public ObservableCollection<FoodsAndService> FoodsAndServices { get => _FoodsAndServicesList; set { _FoodsAndServicesList = value; OnPropertyChanged(); } }

        private string _Name;
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }
        private string _Unit;
        public string Unit { get => _Unit; set { _Unit = value; OnPropertyChanged(); } }

        private decimal? _Price;
        public decimal? Price { get => _Price; set { _Price = value; OnPropertyChanged(); } }

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
                    ImageSource = SelectedItem.ImageData;
                    if (ImageSource != null)
                    {
                        BitmapImage newBitmapImage = new BitmapImage();
                        if (File.Exists(ImageSource))
                        {
                            newBitmapImage.BeginInit();

                            newBitmapImage.StreamSource = new FileStream(ImageSource, FileMode.Open, FileAccess.Read);
                            newBitmapImage.EndInit();
                            Image = newBitmapImage;
                        }
                    }
                    else
                    {
                        Image = null;
                    }

                }
                OnPropertyChanged();
            }
        }

        public ICommand addCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        public ICommand editCommand { get; set; }
        public FoodViewModel()
        {
            LoadFoodList();
            addCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Unit) || Price == 0)
                {
                    return false;
                }
                var list = DataProvider.Instance.DB.Staffs.Where(x => x.Name == Name);
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
                    ImageData = (Image == null) ? null : _SelectedImagePath
                };
                DataProvider.Instance.DB.FoodsAndServices.Add(food);
                DataProvider.Instance.DB.SaveChanges();
                var newFood = DataProvider.Instance.DB.FoodsAndServices.Where(x => x.ID == food.ID).FirstOrDefault();
                addImage(newFood);
                LoadFoodList();
            });

            SelectImageCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                SelectImage();
            });

            editCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Unit) || Price == 0 || SelectedItem == null)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                var food = DataProvider.Instance.DB.FoodsAndServices.Where(x => x.ID == SelectedItem.ID).SingleOrDefault();
                food.Name = Name;
                food.Unit = Unit;
                food.Price = Price;
                food.ImageData = _SelectedImagePath;
                addImage(food);

                DataProvider.Instance.DB.SaveChanges();

                OnPropertyChanged();
                LoadFoodList();
            });
        }
        void LoadFoodList()
        {
            FoodsAndServices = new ObservableCollection<FoodsAndService>();
            var foodList = DataProvider.Instance.DB.Staffs;
            foreach (var item in FoodsAndServices)
            {
                FoodsAndServices.Add(item);
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
                Image = img;
                _SelectedImagePath = OpenFile.FileName;
            }
            OnPropertyChanged();
        }
        void addImage(FoodsAndService food)
        {
            string destinationDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            if (Image != null)
            {
                ImageSource = destinationDirectory + $"\\ImageStorage\\StaffImg\\staff{food.ID}.png";
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
