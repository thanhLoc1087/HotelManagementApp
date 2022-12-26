using HotelManagementApp.Model;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManagementApp.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        public bool IsLogin { get; set; }
        private string _username;
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        private string _password;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        private string _loginMsg;
        public string loginMsg { get => _loginMsg; set { _loginMsg = value; OnPropertyChanged(); } }

        public ICommand LoginCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand ExitApplicationCommand { get; set; }

        //Load View
        public LoginViewModel()
        {
            IsLogin = false;
            Username = "";
            Password = "";
            LoginCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { Login(p); });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
            ExitApplicationCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if(!IsLogin)
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
           );
        }

        void Login(Window p)
        {
            if (p == null)
                return;
            var HashedPassword = MD5Hash(Base64Encode(Password));
            var count = DataProvider.Instance.DB.Accounts.Where(x => x.Username == Username && x.PasswordHash == HashedPassword).Count();
            if (count > 0)
            {
                loginMsg = "";
                IsLogin = true;
                Const.ActiveAccount = DataProvider.Instance.DB.Accounts.Single(x => x.Username == Username);
                p.Close();
            }
            else
            {
                loginMsg = "";
                IsLogin = false;
                loginMsg = Const.loginMsg;
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
    }
}
