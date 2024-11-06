using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitTrack2._0.Commands;
using FitTrack2._0.Helpers;
using FitTrack2._0.Model;
using FitTrack2._0.View;
using System.Windows;

namespace FitTrack2._0.ViewModel
{
    public class ResetPasswordViewModel : BaseViewModel
    {
        public Window _resetPasswordWindow;
        public Window _mainWindow;
        private ManageUser _userManager;
        

        // Användarnamn som anges av användaren
        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                FetchSecurityQuestion(); // Hämtar säkerhetsfrågan automatiskt när användarnamn anges
            }
        }
        // Säkerhetsfråga hämtad från användarens data
        private string _securityQuestion;
        public string SecurityQuestion
        {
            get => _securityQuestion;
            set
            {
                _securityQuestion = value;
                OnPropertyChanged();
            }
        }
        // Användarens svar på säkerhetsfrågan
        private string _inputSecurityAnswer;
        public string InputSecurityAnswer
        {
            get => _inputSecurityAnswer;
            set
            {
                _inputSecurityAnswer = value;
                OnPropertyChanged();
            }
        }
        // Nytt lösenord som användaren vill sätta
        private string _newPassword;
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged();
                ResetPasswordCommand.RaiseCanExecuteChanged();  // Uppdaterar tillgängligheten för ResetPasswordCommand
            }
        }
        // Bekräftat nytt lösenord
        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
                ResetPasswordCommand.RaiseCanExecuteChanged();
            }
        }
        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }
        // Styr om fälten för lösenord ska visas
        private bool _isPasswordFieldsVisible;
        public bool IsPasswordFieldsVisible
        {
            get => _isPasswordFieldsVisible;
            set
            {
                _isPasswordFieldsVisible = value;
                OnPropertyChanged();
            }
        }
        // Kommandon
        public RelayCommand CheckSecurityAnswerCommand => new RelayCommand(execute => CheckSecurityAnswer());
        public RelayCommand ResetPasswordCommand => new RelayCommand(execute => ResetPassword(), canExecute => CanResetPassword());

        // Konstruktor
        public ResetPasswordViewModel(Window resetPasswordWindow,ManageUser userManager)
        {
            _resetPasswordWindow = resetPasswordWindow ?? throw new ArgumentNullException(nameof(resetPasswordWindow));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            IsPasswordFieldsVisible = false;
        }
        // Hämtar säkerhetsfrågan baserat på angivet användarnamn
        private void FetchSecurityQuestion()
        {
            var user = _userManager.GetUser(Username);
            if (user != null)
            {
                SecurityQuestion = user.SecurityQuestion;
                Message = string.Empty; 
            }
            else
            {
                SecurityQuestion = string.Empty;
                Message = "Användarnamnet hittades inte."; 
                IsPasswordFieldsVisible = false;
            }
        }
        // Kontrollera om säkerhetssvaret är korrekt
        private void CheckSecurityAnswer()
        {
            var user = _userManager.GetUser(Username);
            if (user != null && SecurityQuestion != string.Empty)
            {
                if (InputSecurityAnswer.Equals(user.SecurityAnswer, StringComparison.OrdinalIgnoreCase))
                {
                    IsPasswordFieldsVisible = true;
                    Message = "Säkerhetssvaret är korrekt. Ange ditt nya lösenord.";
                    ResetPasswordCommand.RaiseCanExecuteChanged();
                }
                else
                {
                    Message = "Felaktigt säkerhetssvar.";
                    IsPasswordFieldsVisible = false;
                }
            }
            else
            {
                Message = "Användarnamn eller säkerhetsfråga är ogiltig.";
            }
        }
        // Återställer användarens lösenord om alla krav är uppfyllda
        private void ResetPassword()
        {
            var user = _userManager.GetUser(Username);
            if (user != null && IsPasswordFieldsVisible)
            {
                if (NewPassword == ConfirmPassword)
                {
                    if (ValidationHelper.IsValidPassword(NewPassword))
                    {
                        user.Password = NewPassword;
                        Message = "Lösenordet har återställts.";

                        // Öppnar inloggningsfönstret och stänger nuvarande fönster

                        OpenMainWindow();

                    }
                    else
                    {
                        Message = "Lösenordet uppfyller inte säkerhetskraven.";
                    }
                }
                else
                {
                    Message = "Lösenorden matchar inte.";
                }
            }
            else
            {
                Message = "Ange svaret på säkerhetsfrågan först.";
            }
        }
        // Kontrollerar om lösenordsåterställningen kan genomföras
        private bool CanResetPassword()
        {
            return IsPasswordFieldsVisible
                && !string.IsNullOrEmpty(NewPassword)
                && !string.IsNullOrEmpty(ConfirmPassword);
        }
        private void OpenMainWindow()
        {
            var mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            _resetPasswordWindow.Close();
        }
    }

}
