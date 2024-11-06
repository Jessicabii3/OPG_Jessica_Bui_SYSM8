using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitTrack2._0.Model;
using FitTrack2._0.Commands;
using System.Windows.Input;
using System.Collections.ObjectModel;
using FitTrack2._0.View;
using System.Windows;


namespace FitTrack2._0.ViewModel
{
    public class WorkoutDetailsViewModel:BaseViewModel
    {
        // Fält för att hålla redigeringsstatus
        private bool _isEditing;
        private readonly Workout _workout;
        private Workout _originalWorkout;
        public Workout Workout => _workout;
        //private Workout _originalWorkout;
        private ObservableCollection<Workout> _workouts;
       
        private readonly ManageUser _userManager=ManageUser.Instance;
        public event Action RequestCloseDetails;
        public event Action WorkoutDeleted;
        public event Action WorkoutSaved;

        // Konstruktor som tar träningspasset, listan över träningspass och användarhanteraren
        public WorkoutDetailsViewModel(Workout workout)
        {
            _workout = workout ?? throw new ArgumentNullException(nameof(workout));
            // Skapa en kopia av _workout beroende på dess typ
            if (_workout is StrengthWorkout strengthWorkout)
            {
                _originalWorkout = new StrengthWorkout(
                        strengthWorkout.Date,               
                        strengthWorkout.Type,               
                        strengthWorkout.Duration,           
                        strengthWorkout.Notes,              
                        strengthWorkout.Owner,              
                        strengthWorkout.Sets,               
                        strengthWorkout.Reps                
                       );
            }
            else if (_workout is CardioWorkout cardioWorkout)
            {
                _originalWorkout = new CardioWorkout(
                          cardioWorkout.Date,                 
                          cardioWorkout.Duration,             
                          cardioWorkout.Distance,             
                          cardioWorkout.Notes,               
                          cardioWorkout.Owner                 
                      );
            }




            // Initialisering av kommandon
            EditCommand = new RelayCommand(_ => EnableEditing());
            SaveCommand = new RelayCommand(_ => SaveWorkout(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => CancelEditing());
            CopyCommand = new RelayCommand(_ => CopyWorkout());
            DeleteCommand = new RelayCommand(_ => DeleteWorkout());

            IsEditing = false;
        }
      
        private void EnableEditing()
        {
            Console.WriteLine("Edit button clicked. Enabling editing mode.");
            IsEditing = true; // Sätter redigeringsläge till aktivt
            HasAttemptedSave = false; 
        }


        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    Console.WriteLine($"IsEditing changed to: {_isEditing}");
                    OnPropertyChanged(nameof(IsEditing));
                    OnPropertyChanged(nameof(IsReadOnly));
                } 
                
            }
        }
        public bool IsReadOnly => !IsEditing;
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
                    OnPropertyChanged(nameof(WorkoutCaloriesBurned)); 
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
        
        private bool _hasAttemptedSave;
        public bool HasAttemptedSave
        {
            get => _hasAttemptedSave;
            set
            {
                _hasAttemptedSave = value;
                OnPropertyChanged(nameof(HasAttemptedSave));
            }
        }



        // Kontroll om det är möjligt att spara ändringar
        private bool CanSave()
        {
            if (!IsEditing || !HasAttemptedSave)
                return true;

            bool isValid = true;

            if (DateTime.TryParse(WorkoutDate, out DateTime date))
            {
                if (date > DateTime.Today) 
                { MessageBox.Show("Datumet kan inte vara i framtiden.\n");
                    isValid = false; }
                    
            }
            else
            {
                MessageBox.Show("Ogiltigt datumformat. Ange datumet som ÅÅÅÅ-MM-DD.\n");
                isValid = false;
                
            }

            if (string.IsNullOrWhiteSpace(WorkoutType))
            {
                MessageBox.Show( "Typ av träning får inte vara tomt.\n");
                isValid = false;
            }
            if (!TimeSpan.TryParse(WorkoutDuration, out TimeSpan duration) || duration <= TimeSpan.Zero)
            {
                MessageBox.Show( "Varaktigheten måste vara en giltig tid och större än 0.\n");
                isValid = false;
            }
            if (WorkoutCaloriesBurned <= 0)
            {
                MessageBox.Show("Kaloriförbränning måste vara ett positivt tal.\n");
                isValid = false;
            }

            return isValid;

        }
        private void RestoreWorkout(Workout target, Workout original)
        {
            target.Date = original.Date;
            target.Type = original.Type;
            target.Duration = original.Duration;
            target.CaloriesBurned = original.CaloriesBurned;
            target.Notes = original.Notes;
        }


        // Metod för att avbryta redigering och återställa till ursprungsvärdena
        private void CancelEditing()
        {
            RestoreWorkout(_workout, _originalWorkout); // Återställer ändringar
            IsEditing = false; // Avsluta redigeringsläge
            OnPropertyChanged(string.Empty);
            ReturnToWorkoutWindow();
        }
        // Metod för att kopiera träningspasset till listan över träningspass
        private void CopyWorkout()
        {
            // En ny instans av AddWorkoutViewModel med kopierade fält från workout
            //var addWorkoutWindow = new AddWorkoutWindow();
            //var addWorkoutViewModel = new AddWorkoutViewModel(workoutsWindow)
            //{
            //    WorkoutDate = _workout.Date,
            //    WorkoutType = _workout.Type,
            //    WorkoutDuration = _workout.Duration,
            //    WorkoutNotes = _workout.Notes,
            //};

            //addWorkoutWindow.DataContext = addWorkoutViewModel;
          
            //ShowNewWindow(addWorkoutWindow);


        }
        private void SaveWorkout()
        {
            HasAttemptedSave = true;
            if (CanSave())
            {
                IsEditing=false;
                HasAttemptedSave = false;
            }
            WorkoutSaved?.Invoke();
            ReturnToWorkoutWindow();
            IsEditing = false;
        }
        // Metod för att ta bort träningspasset från listan
        private void DeleteWorkout()
        {

            if (_userManager.LoggedInUser?.UserWorkouts.Contains(_workout) == true)
            {
                _userManager.LoggedInUser.UserWorkouts.Remove(_workout);
            }
            ReturnToWorkoutWindow();  
        }
        public int CalculateCalories()
        {
            if (_workout is StrengthWorkout strengthWorkout)
            {
                // Kalkylerar kalorier för styrketräning
                WorkoutCaloriesBurned = strengthWorkout.Sets * strengthWorkout.Reps * 5; 
                return WorkoutCaloriesBurned;
            }
            else if (_workout is CardioWorkout cardioWorkout)
            {
                // Kalkylerar kalorier för konditionsträning
                WorkoutCaloriesBurned = (int)(cardioWorkout.Distance * 10);
                return WorkoutCaloriesBurned;
            }
           return 0;
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
            Application.Current.MainWindow = workoutsWindow;
            ShowNewWindow(workoutsWindow);
           

        }
     
        private void ShowNewWindow(Window window)
        {
            var currentWindow = Application.Current.Windows
                   .OfType<Window>()
                   .FirstOrDefault(w => w.DataContext == this);

            currentWindow?.Close();  
            window.Show();  

        }
        private void CloseCurrentWindow()
        {
            Application.Current.MainWindow?.Close();
        }

    }
}
