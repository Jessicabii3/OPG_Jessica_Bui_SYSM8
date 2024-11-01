
using Fittrack2._0.View;
using FitTrack2._0.Commands;
using FitTrack2._0.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Fittrack2._0.ViewModel
{
    public class WorkoutsViewModel : BaseViewModel
    {
        
        private Workout _originalWorkout;
        public ObservableCollection<Workout> Workouts { get; private set; }
        private Workout? _selectedWorkout;
        private string _errorMessage = string.Empty;
        private ManageUser _userManager;

        // Kommandon
        public RelayCommand AddWorkoutCommand { get; }
        public RelayCommand DeleteWorkoutCommand { get; }
        public RelayCommand OpenUserDetailsCommand { get; }
        public RelayCommand OpenWorkoutDetailsCommand { get; }
        public RelayCommand LogOutCommand { get; }
        public RelayCommand ShowInfoCommand { get; }
        public RelayCommand OpenAddWorkoutWindowCommand { get; }
        


        public WorkoutsViewModel(ManageUser userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
          
            Workouts = new ObservableCollection<Workout>();
            AvailableTypes = new ObservableCollection<string> { "Cardio", "Strength" };
            LoadWorkouts();

            // Initialiserar kommandon
            DeleteWorkoutCommand = new RelayCommand(_ => DeleteWorkout(), _ => CanDeleteWorkout());
            OpenUserDetailsCommand = new RelayCommand(_ => OpenUserDetails());
            OpenWorkoutDetailsCommand = new RelayCommand(_ => OpenWorkoutDetails());
            LogOutCommand = new RelayCommand(_ => LogOut());
            OpenAddWorkoutWindowCommand = new RelayCommand(_ => OpenAddWorkoutWindow());
            ShowInfoCommand = new RelayCommand(_ => ShowInfo());
    }

        // Egenskap för att kontrollera om den inloggade användaren är admin
        public bool IsAdmin => _userManager.LoggedInUser?.IsAdmin ?? false;

        // Egenskap för att visa användarnamn i UI
        public string Username => _userManager.LoggedInUser?.Username ?? string.Empty;

        public ObservableCollection<string> AvailableTypes { get; private set; }

        // Egenskap för det aktuella valda träningspasset
        public Workout? SelectedWorkout
        {
            get => _selectedWorkout;
            set
            {
                _selectedWorkout = value;
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

        // Filteregenskaper
        private DateTime? _filterDate;
        public DateTime? FilterDate
        {
            get => _filterDate;
            set
            {
                _filterDate = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        private string? _filterType;
        public string FilterType
        {
            get => _filterType ?? string.Empty;
            set
            {
                _filterType = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        private TimeSpan? _filterDuration;
        public TimeSpan? FilterDuration
        {
            get => _filterDuration;
            set
            {
                _filterDuration = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        // Metoder
        private void ShowInfo()
        {
            MessageBox.Show("Välkommen till FitTrack!\n\nFitTrack är en träningsapp som hjälper dig att spåra och förbättra din träning. Här kan du lägga till träningspass, granska detaljer, och anpassa din träningsplan.\n\nFör support, kontakta oss på support@fittrack.com.",
                            "Om FitTrack", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadWorkouts()
        {
            if (_userManager.LoggedInUser == null)
            {
                ErrorMessage = "Ingen användare är inloggad.";
                return;
            }

            if (_userManager.LoggedInUser.Username == "Admin1" && IsAdmin)
            {
                // Visa alla träningspass för admin
                Workouts = new ObservableCollection<Workout>(_userManager.GetAllWorkouts());
            }
            else
            {
                // Visa endast användarens egna träningspass
                var userWorkouts = _userManager.GetWorkoutsForUser(_userManager.LoggedInUser.Username);
                Workouts = new ObservableCollection<Workout>(userWorkouts);
            }
            OnPropertyChanged(nameof(Workouts));
        }

       

        private void OpenWorkoutDetails()
        {
            if (SelectedWorkout == null)
            {
                ErrorMessage = "Välj ett träningspass för att se detaljer.";
                return;
            }

            
        }

        private void DeleteWorkout()
        {
            if (SelectedWorkout != null && CanDeleteWorkout())
            {
                Workouts.Remove(SelectedWorkout);
                SelectedWorkout = null;
            }
            else
            {
                ErrorMessage = "Välj ett träningspass att radera.";
            }
        }
        private void ApplyFilters()
        {
            var filteredWorkouts = _userManager.GetAllWorkouts();

            if (FilterDate.HasValue)
                filteredWorkouts = filteredWorkouts.Where(w => w.Date.Date == FilterDate.Value.Date);

            if (!string.IsNullOrWhiteSpace(FilterType))
                filteredWorkouts = filteredWorkouts.Where(w => w.Type.Equals(FilterType, StringComparison.OrdinalIgnoreCase));

            if (FilterDuration.HasValue)
                filteredWorkouts = filteredWorkouts.Where(w => w.Duration >= FilterDuration.Value);

            Workouts = new ObservableCollection<Workout>(filteredWorkouts);
            OnPropertyChanged(nameof(Workouts));
        }

        private bool CanDeleteWorkout() => SelectedWorkout != null;
        private void OpenAddWorkoutWindow()
        {
            var addWorkoutWindow = new AddWorkoutWindow();
            Application.Current.MainWindow = addWorkoutWindow; // Uppdaterar MainWindow till AddWorkoutWindow
            addWorkoutWindow.Show();
            CloseCurrentWindow();
        }

        private void OpenUserDetails()
        {
            var userDetailsViewModel = new UserDetailsViewModel(_userManager);

            // Skapa UserDetailsWindow och tilldela DataContext till UserDetailsViewModel
            var userDetailsWindow = new UserDetailsWindow(userDetailsViewModel);

            Application.Current.MainWindow = userDetailsWindow; // Sätt huvudfönstret till UserDetailsWindow
            userDetailsWindow.Show();
            CloseCurrentWindow();
        }

        private void LogOut()
        {
            var mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow; // Uppdaterar MainWindow till MainWindow (huvudfönstret)
            mainWindow.Show();
            CloseCurrentWindow();
        }
        private void CloseCurrentWindow()
        {
            // Stänger nuvarande fönster
            Application.Current.MainWindow?.Close();
        }
       
    }
}
