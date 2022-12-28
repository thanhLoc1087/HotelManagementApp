using HotelManagementApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HotelManagementApp.ViewModel
{
    internal class BillsViewModel : BaseViewModel
    {
        private ObservableCollection<BillDetail> _FilteredList;
        public ObservableCollection<BillDetail> FilteredList { get => _FilteredList; set { _FilteredList = value; OnPropertyChanged(); } }
        private string _Filter;
        public string Filter { get => _Filter; set { _Filter = value; LoadFilteredList(); OnPropertyChanged(); } }
        private string _StatusFilter;
        public string StatusFilter { get => _StatusFilter; set { _StatusFilter = value; LoadFilteredList(); OnPropertyChanged(); } }
        private string _SearchString;
        public string SearchString { get => _SearchString; set { _SearchString = value; LoadFilteredList(); OnPropertyChanged(); } }
        private BillDetail _SelectedItem;
        public BillDetail SelectedItem { get => _SelectedItem; set { _SelectedItem = value; OnPropertyChanged(); } }
        public BillsViewModel() {
            FilteredList = Global.BillsList;
        }
        void LoadFilteredList()
        {
            ObservableCollection<BillDetail> list = new ObservableCollection<BillDetail>();
            foreach (var item in Global.BillsList)
            {
                if (string.IsNullOrEmpty(Filter) && string.IsNullOrEmpty(SearchString) && string.IsNullOrEmpty(StatusFilter))
                {
                    list = Global.BillsList;
                }
                else if (string.IsNullOrEmpty(Filter) && string.IsNullOrEmpty(SearchString) && !string.IsNullOrEmpty(StatusFilter))
                {
                    if (item.Status == StatusFilter)
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    switch (Filter)
                    {
                        case "Bill ID":
                            if (string.IsNullOrEmpty(SearchString) || (item.ID == Convert.ToInt32(SearchString)) && (item.Status == StatusFilter || string.IsNullOrEmpty(StatusFilter)))
                            {
                                list.Add(item);
                            }
                            break;
                        case "Customer ID":
                            if ((string.IsNullOrEmpty(SearchString) || item.IDCustomer.ToString().Contains(SearchString)) && (item.Status == StatusFilter || string.IsNullOrEmpty(StatusFilter)))
                            {
                                list.Add(item);
                            }
                            break;
                        case "Customer Name":
                            if ((string.IsNullOrEmpty(SearchString) || item.Customer.Name.Contains(SearchString)) && (item.Status == StatusFilter || string.IsNullOrEmpty(StatusFilter)))
                            {
                                list.Add(item);
                            }
                            break;
                        case "Customer CCCD":
                            if ((string.IsNullOrEmpty(SearchString) || item.Customer.CCCD.Contains(SearchString)) && (item.Status == StatusFilter || string.IsNullOrEmpty(StatusFilter)))
                            {
                                list.Add(item);
                            }
                            break;
                        case "Staff ID":
                            if ((string.IsNullOrEmpty(SearchString) || item.IDStaff.ToString().Contains(SearchString)) && (item.Status == StatusFilter || string.IsNullOrEmpty(StatusFilter)))
                            {
                                list.Add(item);
                            }
                            break;
                    }
                }
            }
            FilteredList = list;
        }
    }
}
