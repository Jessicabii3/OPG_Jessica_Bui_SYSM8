using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitTrack2._0.Model;
using FitTrack2._0.Commands;
using FitTrack2._0.Helpers;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows;
using FitTrack2._0.View;

namespace FitTrack2._0.ViewModel
{
    public class UserDetailsViewModel : BaseViewModel
    {
        private User _user;
        private readonly ManageUser _userManager=ManageUser.Instance;

        public UserDetailsViewModel()
        {
            

            _user = ManageUser.Instance.LoggedInUser ?? throw new InvalidOperationException("Ingen användare är inloggad.");
            Username = _user.Username;
            SelectedCountry = _user.Country;
            Countries=CountryManager.GetCountries();

        }
        public string Username
        {
            get => _userManager.LoggedInUser?.Username?? string.Empty;
            set
            {
                if(_userManager.LoggedInUser != null)
                {
                    _userManager.LoggedInUser.Username = value; 
                    OnPropertyChanged();
                }

                
            }
        }
        public string? InputSecurityAnswer { get; set; }

        private string? _newPassword;
        public string? NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged();
            }
        }
        private string? _confirmPassword;
        public string? ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }
        public string? SelectedCountry
        {
            get =>_userManager.LoggedInUser?.Country ?? string.Empty;
            set
            {
                if(_userManager.LoggedInUser != null)
                {
                    _userManager.LoggedInUser.Country = value;
                    OnPropertyChanged();
                }
            }
        }

        // Meddelande som visas för användaren, t.ex. för fel eller framgång
        private string? _message;
        public string? Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }
        // Samling av länder som användaren kan välja mellan
        public ObservableCollection<string> Countries { get; set; }

        // Kommandon för att spara och avbryta ändringar
        public ICommand SaveUserCommand => new RelayCommand(_ => SaveUserDetails());
        public ICommand CancelUserCommand => new RelayCommand(_ => Cancel());
       

        // Metod för att spara användaruppgifter med validering
        private void SaveUserDetails()
        {
            _user.Username = Username;
            _user.Country=SelectedCountry;
            // Validera att användarnamnet är giltigt
            if (!ValidationHelper.IsValidUsername(Username))
            {
                Message = "Användarnamn måste vara minst 3 tecken.";
                return;
            }

            // Kontrollera om användarnamnet redan är upptaget
            if (ManageUser.Instance.IsUsernameTaken(Username) && Username != _user.Username)
            {
                Message = "Användarnamnet är redan upptaget.";
                return;
            }
            // Validera lösenord om ett nytt lösenord anges
            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                if (!ValidationHelper.IsValidPassword(NewPassword))
                {
                    Message = "Lösenordet måste vara minst 8 tecken, innehålla en stor bokstav, en siffra och ett specialtecken.";
                    return;
                }

                if (NewPassword != ConfirmPassword)
                {
                    Message = "Lösenorden matchar inte.";
                    return;
                }

                // Uppdaterar lösenord om det valideras korrekt
                _user.Password = NewPassword;
                _user.Username = Username;
                _user.Country = SelectedCountry;
            }
            
          var workoutsWindow= new WorkoutsWindow();
           ShowNewWindow(workoutsWindow);

        }
        private void Cancel()
        {
            var workoutsWindow = new WorkoutsWindow();
            ShowNewWindow(workoutsWindow);
        }
       
        private void ShowNewWindow(Window window)
        {
            var currentWindow = Application.Current.Windows
                   .OfType<Window>()
                   .FirstOrDefault(w => w.DataContext == this);

            currentWindow?.Close();  // Stäng det aktiva fönstret
            window.Show();  // Öppna det nya fönstret

        }
        private void CloseCurrentWindow()
        {
            Application.Current.MainWindow?.Close();
        }


    }
    }

