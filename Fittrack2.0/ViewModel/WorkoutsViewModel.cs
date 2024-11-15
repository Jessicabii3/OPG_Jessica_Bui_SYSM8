
using FitTrack2._0.View;
using FitTrack2._0.Commands;
using FitTrack2._0.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FitTtrack2._0.ViewModel;
using FitTrack2._0.ViewModel;
using System.Diagnostics.Eventing.Reader;

namespace FitTtrack2._0.ViewModel
{
    public class WorkoutsViewModel :BaseViewModel
    {
        
        
        public ObservableCollection<Workout> WorkoutsList { get; private set; } 
        private Workout? _selectedWorkout;
        private string _errorMessage = string.Empty;
        private readonly ManageUser _userManager = ManageUser.Instance;
        private readonly Window _workoutsWindow;
        

        // Kommandon
        public RelayCommand AddWorkoutCommand { get; }
        public RelayCommand DeleteWorkoutCommand { get; }
        public RelayCommand OpenUserDetailsCommand { get; }
        public RelayCommand OpenWorkoutDetailsCommand { get; }
        public RelayCommand LogOutCommand { get; }
        public RelayCommand ShowInfoCommand { get; }
        public RelayCommand OpenAddWorkoutWindowCommand { get; }
        public RelayCommand ClearfilterCommand { get; }
        
        public WorkoutsViewModel(Window workoutsWindow)
        {
            
            _workoutsWindow = workoutsWindow;
            AvailableTypes = new ObservableCollection<string> { "Cardio", "Strength" };
            LoadWorkouts();

            // Initialiserar kommandon
            DeleteWorkoutCommand = new RelayCommand(execute:e => DeleteWorkout(),canExecute: e => CanDeleteWorkout());
            OpenUserDetailsCommand = new RelayCommand(_ => OpenUserDetails());
            OpenWorkoutDetailsCommand = new RelayCommand(_ => OpenWorkoutDetails());
            LogOutCommand = new RelayCommand(_ => LogOut());
            OpenAddWorkoutWindowCommand = new RelayCommand(_ => OpenAddWorkoutWindow());
            ShowInfoCommand = new RelayCommand(_ => ShowInfo());
            ClearfilterCommand = new RelayCommand(_ =>ClearFilter() );
          
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
        private DateTime? _filterDate = null;
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
        
    

        // Metoder
        private void ShowInfo()
        {
            MessageBox.Show("Välkommen till FitTrack!\n\nFitTrack är en träningsapp som hjälper dig att spåra och förbättra din träning. Här kan du lägga till träningspass, granska detaljer, och anpassa din träningsplan.\n\nFör support, kontakta oss på support@fittrack.com.",
                            "Om FitTrack", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadWorkouts()
        {
            WorkoutsList=new ObservableCollection<Workout>();
            if (_userManager.LoggedInUser == null)
            {
                ErrorMessage = "Ingen användare är inloggad.";
                return;
            }


            if (_userManager.LoggedInUser.Username == "Admin1" && IsAdmin)
            {
                // Visa alla träningspass för admin
                WorkoutsList = new ObservableCollection<Workout>(_userManager.GetAllWorkouts());
                return;
            }
            if (_userManager.LoggedInUser is User)
            {
                WorkoutsList = new ObservableCollection<Workout>(_userManager.LoggedInUser.UserWorkouts);
                return;
            }


        }

      

        private void DeleteWorkout()
        {
            if(_userManager.LoggedInUser.Username != "Admin1")
            {
            if (SelectedWorkout != null && CanDeleteWorkout())
            {
                Workout workoutRemove = null;
              
                foreach (Workout workout in _userManager.LoggedInUser.UserWorkouts)
                {
                    if (workout.Id == SelectedWorkout.Id)
                    {
                        
                        workoutRemove = workout;
                    }
                   
                }
                if(workoutRemove != null)
                {
                    _userManager.LoggedInUser.UserWorkouts.Remove(workoutRemove);
                }
               

                WorkoutsList.Remove(SelectedWorkout);
                SelectedWorkout = null;
            }
            else
            {
                ErrorMessage = "Välj ett träningspass att radera.";
            }

            }
            
            //Hanterar borttagning som admin
            if (_userManager.LoggedInUser.Username == "Admin1" && IsAdmin)
            {
                if (SelectedWorkout != null && CanDeleteWorkout())
                {

                    Workout workoutRemove = null;
                    User owner = null;
                    foreach (User user in _userManager.RegisteredUsers)
                    {
                        if (user.Username == SelectedWorkout.Owner)
                        {
                            owner = user;
                        }

                    }
                    foreach (Workout workout in owner.UserWorkouts)
                    {
                        if (workout.Id == SelectedWorkout.Id)
                        {

                            workoutRemove = workout;
                        }

                    }
                    if (workoutRemove != null)
                    {
                        owner.UserWorkouts.Remove(workoutRemove);
                    }

                    WorkoutsList.Remove(SelectedWorkout);
                    SelectedWorkout = null;
                }

               

                    //foreach (User user in _userManager.RegisteredUsers) //Loopar igenom alla användare
                    //{
                    //    foreach (Workout workout in _userManager.UserWorkouts)
                    //    {
                    //        if (workout.Id == SelectedWorkout.Id)
                    //        {
                    //            owner = user;
                    //            workoutRemove = workout;
                    //            break;
                    //        }
                    //    }
                    //    if (owner != null)
                    //    {
                    //        break;
                    //    }
                    //}
                    //if (workoutRemove != null && owner != null)
                    //{
                    //    owner.UserWorkouts.Remove(workoutRemove);
                    //}

                    //WorkoutsList.Remove(SelectedWorkout);
                    //SelectedWorkout = null;


                //}
                //else
                //{
                //    ErrorMessage = "Välj ett träningspass att radera.";
                //}
            }



              
            
           
        }
        private void ApplyFilters()
        {
            WorkoutsList.Clear();
           

             _userManager.LoggedInUser.UserWorkouts.Where(workout =>
                workout.Type==FilterType && (!FilterDate.HasValue || workout.Date == FilterDate
                 )
                
                )
            .ToList()
            .ForEach(WorkoutsList.Add); 

        }
        private void ClearFilter()
        {
            //Tar bort all data
            WorkoutsList.Clear();
           
            
            //populate workouts med userWorkout
            foreach ( Workout workout in _userManager.LoggedInUser.UserWorkouts)
            {
                WorkoutsList.Add(workout);
            }
           
        }

        private bool CanDeleteWorkout() => SelectedWorkout != null;


        // Metod för att öppna nytt fönster och stänga det aktuella fönstret
        private void ShowNewWindow(Window window)
        {
            var currentWindow = Application.Current.Windows;
   
        }
      

        private void OpenAddWorkoutWindow()
        {


            var addWorkoutWindow = new AddWorkoutWindow();
            _workoutsWindow.Close();
            addWorkoutWindow.Show();


        }

        private void OpenUserDetails()
        {
            
            var userDetailsWindow = new UserDetailsWindow();
            _workoutsWindow.Close();
            userDetailsWindow.Show();
        }

        private void OpenWorkoutDetails()
        {
            if (SelectedWorkout == null)
            {
                ErrorMessage = "Välj ett träningspass för att se detaljer.";
                return;
            }
            var workoutDetailsWindow = new WorkoutDetailsWindow();
            var workoutDetailsViewModel = new WorkoutDetailsViewModel(SelectedWorkout, workoutDetailsWindow);
            workoutDetailsViewModel.WorkoutSaved += LoadWorkouts;
            workoutDetailsViewModel.RequestCloseDetails += () =>
            {
                // Ladda om träningspass för att inkludera eventuella ändringar
                LoadWorkouts();
            };
            _workoutsWindow.Close();
             workoutDetailsWindow.DataContext = workoutDetailsViewModel;
            workoutDetailsWindow.Show();


        }

        private void LogOut()
        {

            var mainWindow = new MainWindow();
            _workoutsWindow.Close();
            mainWindow.Show();
        }
       
       
    }
}
