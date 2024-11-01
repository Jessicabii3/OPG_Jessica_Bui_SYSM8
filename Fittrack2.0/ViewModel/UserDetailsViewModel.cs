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
using Fittrack2._0.View;

namespace Fittrack2._0.ViewModel
{
    public class UserDetailsViewModel : BaseViewModel
    {
        private User _user;
        private readonly ManageUser _userManager;
        private readonly RegisterUserViewModel registerUserViewModel;
        private readonly WorkoutsViewModel workoutsViewModel;
        private readonly Window userDetailsWindow;
        
        public UserDetailsViewModel(Window userDetailsWindow, ManageUser userManager,RegisterUserViewModel regisUserWindow)
        {
            this.userDetailsWindow = userDetailsWindow;
            this.registerUserViewModel = regisUserWindow;
            this._userManager = userManager;
           
           
            Countries = CountryManager.GetCountries();
            
        }
        public string? Username { get; set; }
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
        public string? SelectedCountry { get; set; }

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
            // Validera att användarnamnet är giltigt
            if (!ValidationHelper.IsValidUsername(Username))
            {
                Message = "Användarnamn måste vara minst 3 tecken.";
                return;
            }

            // Kontrollera om användarnamnet redan är upptaget
            if (_userManager.IsUsernameTaken(Username) && Username != _user.Username)
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
            }
            // Uppdatera användarens namn och land
           WorkoutsWindow newWorkoutsWindow=new WorkoutsWindow();
            newWorkoutsWindow.Show();
            userDetailsWindow.Close();


        }
        private void Cancel()
        {
            WorkoutsWindow newWorkoutsWindow = new WorkoutsWindow();
            newWorkoutsWindow.Show();
            userDetailsWindow.Close();
        }
       



    }
    }

