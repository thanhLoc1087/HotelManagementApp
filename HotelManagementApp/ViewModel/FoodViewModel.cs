//using HotelManagementApp.Model;
//using HotelManagementApp.View;
//using Microsoft.Win32;
//using System.Collections.ObjectModel;
//using System.IO;
//using System.Linq;
//using System.Security;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Xml.Serialization;

//namespace HotelManagementApp.ViewModel
//{
//    public class FoodViewModel : BaseViewModel
//    {
//        private ObservableCollection<FoodsAndService> _FoodsAndServicesList;
//        public ObservableCollection<FoodsAndService> FoodsAndServices { get => _FoodsAndServicesList; set { _FoodsAndServicesList = value; OnPropertyChanged(); } }

//        private string _Name;
//        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }
//        private string _Unit;
//        public string Unit { get => _Unit; set { _Unit = value; OnPropertyChanged(); } }

//        private decimal? _Price;
//        public decimal? Price { get => _Price; set { _Price = value; OnPropertyChanged(); } }


//        private BitmapImage _FoodImage;
//        public BitmapImage FoodImage { get => _FoodImage; set { _FoodImage = value; OnPropertyChanged(); } }



//        private FoodsAndService _SelectedItem;
//        public  FoodsAndService SelectedItem
//        {
//            get => _SelectedItem;
//            set
//            {
//                _SelectedItem = value;
//                if (SelectedItem != null)
//                {
//                    Name = SelectedItem.Name;
//                    Unit = SelectedItem.Unit;
//                    Price = SelectedItem.Price;
//                    //if (SelectedItem.ImageData != null)
//                    //{
//                    //    System.IO.MemoryStream ms = new System.IO.MemoryStream(SelectedItem.ImageData);
//                    //    ms.Seek(0, System.IO.SeekOrigin.Begin);
//                    //    BitmapImage newBitmapImage = new BitmapImage();

//                    //    newBitmapImage.BeginInit();

//                    //    newBitmapImage.StreamSource = ms;
//                    //    newBitmapImage.EndInit();
//                    //    FoodImage = newBitmapImage;
//                    //}
//                    //else
//                    //{
//                    //    FoodImage = null;
//                    //}

//                }
//                OnPropertyChanged();
//            }
//        }

//        public ICommand addCommand { get; set; }
//        public ICommand SelectImageCommand { get; set; }
//        public ICommand editCommand { get; set; }
//        public FoodViewModel()
//        {
//            LoadFoodList();
//            addCommand = new RelayCommand<object>((p) =>
//            {
//                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Unit) || Price == 0)
//                {
//                    return false;
//                }
//                var list = DataProvider.Instance.DB.Staffs.Where(x => x.Name == Name);
//                if (list == null || list.Count() != 0)
//                {
//                    return false;
//                }
//                return true;
//            }, (p) =>
//            {
//                var food = new FoodsAndService()
//                {
//                    Name = Name,
//                    Unit = Unit,
//                    Price = Price,
//                    //ImageData = (FoodImage == null) ? null : convertImageToBytes(FoodImage)
//                };
//                DataProvider.Instance.DB.FoodsAndServices.Add(food);
//                DataProvider.Instance.DB.SaveChanges();

//                FoodsAndServices.Add(food);
//            });

//            SelectImageCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
//            {
//                addImage();
//            });

//            editCommand = new RelayCommand<object>((p) =>
//            {
//                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Unit) || Price == 0 || SelectedItem == null)
//                {
//                    return false;
//                }
//                return true;
//            }, (p) =>
//            {
//                var food = DataProvider.Instance.DB.Foods.Where(x => x.ID == SelectedItem.ID).SingleOrDefault();
//                food.Name = Name;
//                food.Unit = Unit;
//                food.Price = Price;
//                //food.ImageData = (FoodImage== null) ? null : convertImageToBytes(FoodImage);

//                DataProvider.Instance.DB.SaveChanges();

//                OnPropertyChanged();
//                LoadFoodList();
//            });
//        }
//        void LoadFoodList()
//        {
//            FoodsAndServices = new ObservableCollection<Food>();
//            var foodList = DataProvider.Instance.DB.Staffs;
//            foreach (var item in FoodsAndServices)
//            {
//                FoodsAndServices.Add(item);
//            }
//        }

//        void addImage()
//        {
//            OpenFileDialog OpenFile = new OpenFileDialog();
//            OpenFile.Multiselect = false;
//            OpenFile.Title = "Select Picture(s)";
//            OpenFile.Filter = "ALL supported Graphics| *.jpeg; *.jpg;*.png;";
//            if (OpenFile.ShowDialog() == true)
//            {
//                BitmapImage img = new BitmapImage();
//                img.BeginInit();
//                img.StreamSource = new FileStream(OpenFile.FileName, FileMode.Open, FileAccess.Read);
//                img.EndInit();
//                FoodImage = img;
//            }
//            OnPropertyChanged();
//        }
//        byte[] convertImageToBytes(BitmapImage img)
//        {
//            byte[] imgData = new byte[img.StreamSource.Length];
//            img.StreamSource.Seek(0, SeekOrigin.Begin);
//            img.StreamSource.Read(imgData, 0, imgData.Length);
//            return imgData;
//        }
//    }

//}
