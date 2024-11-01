using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitTrack2._0.Model;
using FitTrack2._0.Commands;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Fittrack2._0.View;
using System.Windows;


namespace Fittrack2._0.ViewModel
{
    public class WorkoutDetailsViewModel:BaseViewModel
    {
        // Fält för att hålla redigeringsstatus
        private bool _isEditing;
        private readonly Workout _workout;
       
       

        public Workout Workout => _workout;
        //private Workout _originalWorkout;

        
        private ObservableCollection<Workout> _workouts;
       
        private ManageUser _userManager;
        public event Action RequestCloseDetails;
        public event Action WorkoutDeleted;

        // Konstruktor som tar träningspasset, listan över träningspass och användarhanteraren
        public WorkoutDetailsViewModel(Workout workout, ObservableCollection<Workout> workouts, ManageUser userManager)
        {
            _workout = workout ?? throw new ArgumentNullException(nameof(workout));
            _workouts = workouts ?? throw new ArgumentNullException(nameof(workouts));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
           
            // Initialisering av kommandon
            EditCommand = new RelayCommand(_ => EnableEditing());
            SaveCommand = new RelayCommand(_ => SaveWorkout(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => CancelEditing());
            CopyCommand = new RelayCommand(_ => CopyWorkout());
            DeleteCommand = new RelayCommand(_ => DeleteWorkout());

            IsEditing = false; 

        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested(); // Uppdaterar kommandon beroende på redigeringsläge
            }
        }
        // Egenskap för att kontrollera om användaren är admin
        public bool IsAdmin => _userManager.LoggedInUser?.IsAdmin ?? false;

        // Datum för träningspasset, som visas i gränssnittet
        public string WorkoutDate
        {
            get => _workout.Date.ToString("yyyy-MM-dd");
            set
            {
                if (DateTime.TryParse(value, out DateTime date))
                {
                    _workout.Date = date;
                    OnPropertyChanged();
                }
            }
        }
        public string WorkoutType
        {
            get => _workout.Type;
            set
            {
                _workout.Type = value;
                OnPropertyChanged();
            }
        }
        // Varaktighet för träningspasset
        public string WorkoutDuration
        {
            get => _workout.Duration.ToString(@"hh\:mm");
            set
            {
                if (TimeSpan.TryParse(value, out TimeSpan duration))
                {
                    _workout.Duration = duration;
                    OnPropertyChanged();
                }
            }
        }
        // Kaloriförbränning för träningspasset
        public int WorkoutCaloriesBurned
        {
            get => _workout.CaloriesBurned;
            set
            {
                _workout.CaloriesBurned = value;
                OnPropertyChanged();
            }
        }
        // Anteckningar om träningspasset
        public string WorkoutNotes
        {
            get => _workout.Notes;
            set
            {
                _workout.Notes = value;
                OnPropertyChanged();
            }
        }
        //Egenskap för reps
        public int WorkoutReps
        {
            get => (_workout as StrengthWorkout)?.Reps ?? 0;
            set
            {
                if (_workout is StrengthWorkout strengthWorkout)
                {
                    strengthWorkout.Reps = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(WorkoutCaloriesBurned)); // Uppdatera kaloriberäkninge
                }
            }
        }
        //Egenskap för sets
        public int WorkoutSets
        {
            get => (_workout as StrengthWorkout)?.Sets ?? 0;
            set
            {
                if (_workout is StrengthWorkout strengthWorkout)
                {
                    strengthWorkout.Sets = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(WorkoutCaloriesBurned));
                }
            }
        }
        public int WorkoutDistance
        {
            get => (_workout as CardioWorkout)?.Distance ?? 0;
            set
            {
                if (_workout is CardioWorkout cardioWorkout)
                {
                    cardioWorkout.Distance = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(WorkoutCaloriesBurned));
                }
            }
        }

        public bool CanDeleteWorkout => IsAdmin || _workout.Owner == _userManager.LoggedInUser?.Username;
        // Kommandon för olika åtgärder
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand DeleteCommand { get; }


        


        // Metod för att aktivera redigeringsläge
        private void EnableEditing() => IsEditing = true;

        private void SaveWorkout()
        {
            IsEditing = false;
            ReturnToWorkoutWindow();

        }
        // Kontroll om det är möjligt att spara ändringar
        private bool CanSave()
        {
            if (!IsEditing) return false;

            // Kontrollera att datum inte ligger i framtiden
            if (DateTime.TryParse(WorkoutDate, out DateTime date))
            {
                if (date > DateTime.Today)
                    return false; // Ogiltigt om datumet är i framtiden
            }
            else
            {
                return false;
            }
            // Kontrollera att typ av träningspass är giltigt
            if (string.IsNullOrWhiteSpace(WorkoutType))
                return false;

            // Kontrollera att varaktighet är satt och giltig
            if (!TimeSpan.TryParse(WorkoutDuration, out TimeSpan duration) || duration <= TimeSpan.Zero)
                return false;
            if(WorkoutReps < 0) // Kontrollera att repetitioner är positiva
            // Kontrollera att kaloriförbränning är större än noll
            if (WorkoutCaloriesBurned <= 0)
                return false;

            // Om alla kontroller har klarat, returnera true
            return true;
        }
            

        // Metod för att avbryta redigering och återställa till ursprungsvärdena
        private void CancelEditing()
        {
            //RestoreWorkout(_workout, _originalWorkout); // Återställer ändringar
            IsEditing = false; // Avsluta redigeringsläge
            OnPropertyChanged(string.Empty);
            ReturnToWorkoutWindow();
        }
        // Metod för att kopiera träningspasset till listan över träningspass
        private void CopyWorkout()
        {
            // En ny instans av AddWorkoutViewModel med kopierade fält från workout
            var addWorkoutWindow = new AddWorkoutWindow();
            var addWorkoutViewModel = new AddWorkoutViewModel(_workout)
            {
                WorkoutDate = _workout.Date,
                WorkoutType = _workout.Type,
                WorkoutDuration = _workout.Duration,
                WorkoutCaloriesBurned = _workout.CaloriesBurned,
                WorkoutNotes = _workout.Notes,
            };

            addWorkoutWindow.DataContext = addWorkoutViewModel;
            Application.Current.MainWindow = addWorkoutWindow;
            addWorkoutWindow.Show();

            CloseWorkoutDetailsWindow();


        }
        // Metod för att ta bort träningspasset från listan
        private void DeleteWorkout()
        {
            if (_workouts.Contains(_workout))
            {
                _workouts.Remove(_workout); // Ta bort workout från den lokala listan
            }
            _userManager.LoggedInUser?.UserWorkouts.Remove(_workout);
            


        }
        private void CalculateCalories()
        {
            if (_workout is StrengthWorkout strengthWorkout)
            {
                // Kalkylerar kalorier för styrketräning
                WorkoutCaloriesBurned = strengthWorkout.Sets * strengthWorkout.Reps * 5; 
            }
            else if (_workout is CardioWorkout cardioWorkout)
            {
                // Kalkylerar kalorier för konditionsträning
                WorkoutCaloriesBurned = (int)(cardioWorkout.Distance * 10); 
            }
        }


        // Metod för att stänga WorkoutDetails och återgå till WorkoutWindow
        private void CloseAndReturnToWorkoutWindow()
        {

            var workoutDetailsWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);
            workoutDetailsWindow?.Close();
        }
        private void ReturnToWorkoutWindow()
        {
            var workoutsWindow = new WorkoutsWindow();
            Application.Current.MainWindow = workoutsWindow; // Uppdaterar till WorkoutsWindow som huvudfönster
            workoutsWindow.Show();

            CloseWorkoutDetailsWindow();
        }
        // Metod för att stänga WorkoutDetailsWindow
        private void CloseWorkoutDetailsWindow()
        {
            var workoutDetailsWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);
            workoutDetailsWindow?.Close();
        }

    }
}
