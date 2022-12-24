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
        private ObservableCollection<FoodsAndService> _TempOrdersList;
        public ObservableCollection<FoodsAndService> TempOrdersList { get => _TempOrdersList; set { _TempOrdersList = value; OnPropertyChanged(); } }
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
                    TempOrdersList.Add(SelectedItem);
                }
                OnPropertyChanged();
            } 
        }

        public OrderViewModel()
        {
            TempOrdersList = new ObservableCollection<FoodsAndService>();
        }

        private void LoadFilteredList()
        { }
    }
}
