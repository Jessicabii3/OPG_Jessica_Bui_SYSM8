using FitTrack2._0.Model;
using System;
using System.Windows;
using FitTrack2._0.View;
using System.Windows.Input;
using FitTrack2._0.Commands;
using FitTrack2._0.Helpers;

namespace FitTrack2._0.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        // Egenskaper
        private string _username;
        private string _password;
        private string _errorMessage;
        private bool _isLoggedIn;
        private readonly ManageUser _userManager = ManageUser.Instance;
        private bool _isLoginButtonVisible;
        private Window _mainWindow;
        
        public string _sendkey;
        public string SendKey
        {
            get => _sendkey;
            set
            {
                _sendkey = value;
                OnPropertyChanged();
            }
        }

        private string _keyInput;
        public string KeyInput
        {
            get => _keyInput;
            set
            {
                _keyInput = value;
                OnPropertyChanged();
            }
        }

        private int _key;
        public int Key
        {
            get => _key;
            set
            {
                _key = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoginButtonVisible
        {
            get => _isLoginButtonVisible;
            set
            {
                _isLoginButtonVisible = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SendKeyCommand => new RelayCommand(execute => ExecuteSendKey());
        public RelayCommand VerifyCodeCommand => new RelayCommand(execute => ExecuteVerifyCode());
        public RelayCommand LoginCommand => new RelayCommand(execute => SignIn());
        public RelayCommand ResetPasswordCommand => new RelayCommand(execute => OpenResetPasswordWindow());
        public RelayCommand RegisterUserCommand => new RelayCommand(execute => OpenRegisterUserWindow());

        public MainViewModel(Window _mainWindow)
        {
            this._mainWindow =  _mainWindow;
            _username = string.Empty;
            _password = string.Empty;
            _errorMessage = string.Empty;
            _isLoggedIn = false;
            _isLoginButtonVisible = false;
        }

        private void ExecuteSendKey()
        {
            Random random = new Random();
            Key = random.Next(100000, 999999);
            MessageBox.Show($"{Key}", "Authentication numbers", MessageBoxButton.OK);
        }

        private void ExecuteVerifyCode()
        {
            if (Key.ToString() == KeyInput)
            {
                IsLoginButtonVisible = true;
                ErrorMessage = string.Empty;
            }
            else
            {
                ErrorMessage = "Authentication key är fel, försök igen";
                IsLoginButtonVisible = false;
            }
        }

        public void SignIn()
        {
            if (!IsLoginButtonVisible)
            {
                ErrorMessage = "Försök igen";
                return;
            }

            if (!ValidationHelper.IsValidUsername(Username))
            {
                ErrorMessage = "Användarnamnet måste vara minst 3 tecken långt.";
                return;
            }

            if (!ValidationHelper.IsValidPassword(Password))
            {
                ErrorMessage = "Lösenordet måste vara minst 8 tecken långt och innehålla minst en siffra och ett specialtecken.";
                return;
            }

            if (_userManager.ValidateCredentials(Username, Password))
            {
                OpenWorkoutWindow();
            }
            else
            {
                ErrorMessage = "Ogiltigt användarnamn eller lösenord.";
            }
        }

        public void OpenResetPasswordWindow()
        {
           
            new ResetPasswordWindow().Show();
            CloseCurrentWindow();
        }

        public void OpenRegisterUserWindow()
        {
           
            new RegisterUserWindow().Show(); //Öppna registering, skapa ett nytt registerWindow
            CloseCurrentWindow();
        }

        private void OpenWorkoutWindow()
        {
         
            new WorkoutsWindow().Show();
            CloseCurrentWindow();    

        }

        private void CloseCurrentWindow()
        {
            _mainWindow.Close();
        }
       
    }
}
