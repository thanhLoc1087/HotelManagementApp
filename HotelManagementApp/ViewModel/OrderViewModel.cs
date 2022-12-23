using HotelManagementApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace HotelManagementApp.ViewModel
{
    public class OrderViewModel : BaseViewModel
    {
        private ObservableCollection<FoodsAndService> _FoodsAndServicesList;
        public ObservableCollection<FoodsAndService> FoodsAndServicesList { get => _FoodsAndServicesList; set { _FoodsAndServicesList = value; OnPropertyChanged(); } }
        private ObservableCollection<FoodsAndService> _OrdersList;
        public ObservableCollection<FoodsAndService> OrdersList { get => _OrdersList; set { _OrdersList = value; OnPropertyChanged(); } }
        private ObservableCollection<FoodsAndService> _FilteredList;
        public ObservableCollection<FoodsAndService> FilteredList { get => _FilteredList; set { _FilteredList = value; OnPropertyChanged(); } }
        private FoodsAndService _SelectedItem;
        public FoodsAndService SelectedItem
        { 
            get => _SelectedItem;
            set 
            { 
                _SelectedItem = value;
                if (SelectedItem != null)
                {
                    OrdersList.Add(SelectedItem);
                }
                OnPropertyChanged();
            } 
        }

        public OrderViewModel()
        {
            LoadFoodList();
            OrdersList = new ObservableCollection<FoodsAndService>();
        }
        private void LoadFoodList()
        {
            FoodsAndServicesList = new ObservableCollection<FoodsAndService>();
            var foodList = DataProvider.Instance.DB.FoodsAndServices.Where(x => x.Deleted == false);
            foreach (var item in foodList)
            {
                FoodsAndServicesList.Add(item);
            }
            LoadFilteredList();
        }

        private void LoadFilteredList()
        { }
    }
}
