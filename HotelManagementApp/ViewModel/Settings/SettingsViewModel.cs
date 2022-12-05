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
        private string _CurrentPassword;
        public string CurrentPassword { get => _CurrentPassword; set { _CurrentPassword = value; OnPropertyChanged(); } }
        private string _NewPassword;
        public string NewPassword { get => _NewPassword; set { _NewPassword = value; OnPropertyChanged(); } }
        private string _ConfirmPassword;
        public string ConfirmPassword { get => _ConfirmPassword; set { _ConfirmPassword = value; OnPropertyChanged(); } }
        public ICommand LogoutCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        public SettingsViewModel()
        {
            LogoutCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            );

            ChangePasswordCommand = new RelayCommand<object>((p) => 
            {
                var hashedPassword = MD5Hash(Base64Encode(CurrentPassword));
                if(ConfirmPassword != NewPassword)
                {
                    return false;
                }
                //if(hashedPassword != CurrentEmployee.PasswordHash)
                //{
                //    return false;
                //}
                
                return true;
            }, (p) =>
            {
                //var staff = DataProvider.Instance.DB.Staffs.Where(x => x.ID == CurrentEmployee.ID).FirstOrDefault();
                var hashedPassword = MD5Hash(Base64Encode(NewPassword));
                // CurrentEmployee.PasswordHash = hashedPassword;
                DataProvider.Instance.DB.SaveChanges();
            }
            );
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
