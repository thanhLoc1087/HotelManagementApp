
using HotelManagementApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApp.ViewModel
{
    public class ServiceViewModel : BaseViewModel
    {
        private FoodsAndService _Service;
        public FoodsAndService Service { get => _Service; set { _Service = value; OnPropertyChanged(); } }

    }
}
