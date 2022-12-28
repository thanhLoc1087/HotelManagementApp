using HotelManagementApp.Model;
using HotelManagementApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HotelManagementApp.ViewModel
{
    public class SettingsViewModel : BaseViewModel
    {
        private bool IsLoggedOut = false;
        private string _CurrentPassword;
        public string CurrentPassword { get => _CurrentPassword; set { _CurrentPassword = value; OnPropertyChanged(); } }
        private string _NewPassword;
        public string NewPassword { get => _NewPassword; set { _NewPassword = value; OnPropertyChanged(); } }
        private string _ConfirmPassword;
        public string ConfirmPassword { get => _ConfirmPassword; set { _ConfirmPassword = value; OnPropertyChanged(); } }
        private string _hotelName = Const.hotelName;
        public string HotelName { get => _hotelName; set { _hotelName = value; OnPropertyChanged(); } }
        private string _hotelMoto = Const.hotelMoto;
        public string HotelMoto { get => _hotelMoto; set { _hotelMoto = value; OnPropertyChanged(); } }
        private string _hotelPhone = Const.hotelPhone;
        public string HotelPhone { get => _hotelPhone; set { _hotelPhone = value; OnPropertyChanged(); } }
        private string _hotelMail = Const.hotelMail;
        public string HotelEmail { get => _hotelMail; set { _hotelMail = value; OnPropertyChanged(); } }
        private string _hotelAddress = Const.hotelAddress;
        public string HotelAddress { get => _hotelAddress; set { _hotelAddress = value; OnPropertyChanged(); } }
        private string _loginFailed = Const.loginMsg;
        public string LoginFailed { get => _loginFailed; set { _loginFailed = value; OnPropertyChanged(); } }
        private string _chartFailed = Const.statErrorMsg;
        public string ChartFailed { get => _chartFailed; set { _chartFailed = value; OnPropertyChanged(); } }
        private string _successMsg;
        public string SuccessMsg { get => _successMsg; set { _successMsg = value; OnPropertyChanged(); } }
        public ICommand LogoutCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }
        public ICommand SaveChanges { get; set; }
        public ICommand CancelChanges { get; set; }

        public SettingsViewModel()
        {
            LogoutCommand = new RelayCommand<object>((p) => { if (IsLoggedOut) return false; return true; }, (p) =>
            {
                IsLoggedOut = true;
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            );
            
            ChangePasswordCommand = new RelayCommand<object>((p) =>
            {
                var hashedPassword = MD5Hash(Base64Encode(CurrentPassword));
                if (ConfirmPassword != NewPassword)
                {
                    return false;
                }
                if (hashedPassword != Const.ActiveAccount.PasswordHash)
                {
                    return false;
                }

                return true;
            }, (p) =>
            {
                var account = DataProvider.Instance.DB.Accounts.Where(x => x.IDStaff == Const.ActiveAccount.IDStaff).FirstOrDefault();
                var hashedPassword = MD5Hash(Base64Encode(NewPassword));
                account.PasswordHash = hashedPassword;
                DataProvider.Instance.DB.SaveChanges();
                CurrentPassword = NewPassword = ConfirmPassword = null;
            }
            );
            SaveChanges = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(HotelAddress) ||
                string.IsNullOrEmpty(HotelEmail) ||
                string.IsNullOrEmpty(HotelMoto) ||
                string.IsNullOrEmpty(HotelName) ||
                string.IsNullOrEmpty(HotelPhone) ||
                string.IsNullOrEmpty(LoginFailed) ||
                string.IsNullOrEmpty(ChartFailed)) 
                {
                    SuccessMsg = "All fields must not be empty.";
                    return false;
                }
                if (
                HotelAddress == Const.hotelName &&
                HotelMoto == Const.hotelMoto &&
                HotelPhone == Const.hotelPhone &&
                HotelEmail == Const.hotelMail &&
                HotelAddress == Const.hotelAddress &&
                LoginFailed == Const.loginMsg &&
                ChartFailed == Const.statErrorMsg)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                Const.hotelName = HotelAddress;
                Const.hotelMoto = HotelMoto;
                Const.hotelPhone = HotelPhone;
                Const.hotelMail = HotelEmail;
                Const.hotelAddress = HotelAddress;
                Const.loginMsg = LoginFailed;
                Const.statErrorMsg = ChartFailed;
                SuccessMsg = "Successfully save changes!";
            });
            CancelChanges = new RelayCommand<object>((p) =>
            {
                if (
                HotelAddress == Const.hotelName ||
                HotelMoto == Const.hotelMoto ||
                HotelPhone == Const.hotelPhone ||
                HotelEmail == Const.hotelMail ||
                HotelAddress == Const.hotelAddress ||
                LoginFailed == Const.loginMsg ||
                ChartFailed == Const.statErrorMsg)
                {
                    return false;
                }
                return true;
            }, (p) =>
            {
                HotelAddress = Const.hotelName;
                HotelMoto = Const.hotelMoto;
                HotelPhone = Const.hotelPhone;
                HotelEmail = Const.hotelMail;
                HotelAddress = Const.hotelAddress;
                LoginFailed = Const.loginMsg;
                ChartFailed = Const.statErrorMsg;
                SuccessMsg = "";
            });
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
    }
}