using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitTrack2._0.Commands;
using FitTrack2._0.Model;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using FitTrack2._0.View;
using System.Windows;

namespace FitTrack2._0.ViewModel
{
    public class AddWorkoutViewModel : BaseViewModel

    {

        private readonly Window _workoutsWindow;
        private Visibility strenghtWorkoutVistbilty =Visibility.Collapsed;
        public Visibility StrenghtWorkoutVisibility

        {
            get => strenghtWorkoutVistbilty;
            set
            {
                strenghtWorkoutVistbilty = value;
                OnPropertyChanged();
            }
        }
        private Visibility cardioWorkoutVisibilty= Visibility.Collapsed;
        public Visibility CardioWorkoutVisibility
        {
            get => cardioWorkoutVisibilty;
            set
            {
                cardioWorkoutVisibilty = value;
                OnPropertyChanged();
            }
        }
        // Allmänna egenskaper för träningspass
        public DateTime WorkoutDate { get; set; } = DateTime.Today;
        private string workoutType;
        public string WorkoutType
        {
            get => workoutType;
            set
            {
                workoutType = value;
                if(value == "Strength")
                {
                    StrenghtWorkoutVisibility = Visibility.Visible;
                    CardioWorkoutVisibility= Visibility.Collapsed;

                }

                if (value == "Cardio")
                {
                    CardioWorkoutVisibility= Visibility.Visible;
                    StrenghtWorkoutVisibility = Visibility.Collapsed;
                }
                OnPropertyChanged();
                UpdateWorkoutType(); 
            }
        }


        // Egenskaper
        private TimeSpan workoutDuration;
        public TimeSpan WorkoutDuration
        {
            get { return workoutDuration; }
            set
            {
                workoutDuration = value;
                OnPropertyChanged();
            }
        }
        private int? _workoutCaloriesBurned;
        public int? WorkoutCaloriesBurned
        {
            get => _workoutCaloriesBurned;
            private set
            {
                _workoutCaloriesBurned = value;

                OnPropertyChanged();
            }
        }
        public string WorkoutNotes { get; set; } = string.Empty;

        // Specifika egenskaper för Strength och Cardio
        private int? _workoutReps;
        public int? WorkoutReps
        {
            get => (Workout as StrengthWorkout)?.Reps;
            set
            {
                if (Workout is StrengthWorkout strengthWorkout)
                {
                    strengthWorkout.Reps = value ?? 0;
                    CalculateCalories();
                    OnPropertyChanged();
                }
            }
        }
        private int? _workoutSets;
        public int? WorkoutSets
        {
            //get => _workoutSets;
            //set
            //{
            //    _workoutSets = value;
            //    CalculateCalories();
            //    OnPropertyChanged();
            //}
            get => (Workout as StrengthWorkout)?.Sets;
            set
            {
                if (Workout is StrengthWorkout strengthWorkout)
                {
                    strengthWorkout.Sets = value ?? 0;
                    CalculateCalories(); OnPropertyChanged();
                }
            }


        }
        private int? _workoutDistance;
        public int? WorkoutDistance
        {
            //get => _workoutDistance;
            //set
            //{
            //    _workoutDistance = value;
            //    CalculateCalories();
            //    OnPropertyChanged();
            //}
            get => (Workout as CardioWorkout)?.Distance;
            set
            {
                if (Workout is CardioWorkout cardioWorkout)
                {
                    cardioWorkout.Distance = value ?? 0;
                    CalculateCalories(); OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<string> WorkoutTypes { get; } = new ObservableCollection<string> { "Strength", "Cardio" };

        // Egenskap som innehåller kopierat träningspass
        private Workout _workout;
        public Workout Workout
        {
            get => _workout;
            set
            {
                _workout = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<Workout> WorkoutSaved;
        public event Action Cancelled;
        public Action CloseWorkoutWindow { get; set; }

   


        public AddWorkoutViewModel( Window workoutsWindow, Workout workoutToCopy = null)
        {
            _workoutsWindow = workoutsWindow;
            if (workoutToCopy != null)
            {
                WorkoutDate = workoutToCopy.Date;
                WorkoutType = workoutToCopy.Type;
                WorkoutDuration = workoutToCopy.Duration;
                WorkoutCaloriesBurned = workoutToCopy.CaloriesBurned;
                WorkoutNotes = workoutToCopy.Notes;

                if (workoutToCopy is StrengthWorkout strengthWorkout)
                {
                    WorkoutReps = strengthWorkout.Reps;
                    WorkoutSets = strengthWorkout.Sets;
                    Workout = strengthWorkout;
                    CalculateCalories();
                }
                else if (workoutToCopy is CardioWorkout cardioWorkout)
                {
                    WorkoutDistance = cardioWorkout.Distance;
                    Workout = cardioWorkout;
                    CalculateCalories();
                }
            }
            SaveCommand = new RelayCommand(_ => SaveWorkout(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => Cancel());
        }


        private void UpdateWorkoutType()
        {
           
            if (WorkoutType == "Strength")
            {
                Workout = new StrengthWorkout(WorkoutDate, WorkoutType, WorkoutDuration, WorkoutNotes, "Anna", WorkoutReps ?? 0, WorkoutSets ?? 0);
                WorkoutDistance = 0;
            }
            else if (WorkoutType == "Cardio")
            {
                Workout = new CardioWorkout(WorkoutDate, WorkoutDuration, WorkoutDistance ?? 0, WorkoutNotes, "Anna");
                WorkoutReps = 0;
                WorkoutSets = 0;
            }
            CalculateCalories();
            OnPropertyChanged(nameof(WorkoutReps));
            OnPropertyChanged(nameof(WorkoutSets));
            OnPropertyChanged(nameof(WorkoutDistance));

        }


        private void CalculateCalories()
        {

            
            if (Workout is StrengthWorkout && WorkoutReps > 0 && WorkoutSets > 0)
            {
                WorkoutCaloriesBurned = WorkoutSets * WorkoutReps * 5;
            }
            else if (Workout is CardioWorkout && WorkoutDistance > 0)
            {
                WorkoutCaloriesBurned = WorkoutDistance * 10;
            }
            else
            {
                WorkoutCaloriesBurned = 0;
            }
            OnPropertyChanged(nameof(WorkoutCaloriesBurned));
        }


        // Validering: Kontrollerar om alla fält är korrekt ifyllda
        private bool CanSave()
        {
            bool hasAllFields = !string.IsNullOrWhiteSpace(WorkoutType) &&
                       WorkoutDuration > TimeSpan.Zero &&
                       WorkoutCaloriesBurned > 0 &&
                       !string.IsNullOrWhiteSpace(WorkoutNotes);

            if (WorkoutType == "Strength")
            {
                return hasAllFields && WorkoutReps > 0 && WorkoutSets > 0;
            }
            else if (WorkoutType == "Cardio")
            {
                return hasAllFields && WorkoutDistance > 0;
            }
            return false;

            

        }
        private void SaveWorkout()
        {
            if (!CanSave())
            {
                // Visar meddelanden om fälten är ofullständiga
                StringBuilder errorMessage = new StringBuilder();

                if (string.IsNullOrWhiteSpace(WorkoutType))
                    errorMessage.AppendLine("Vänligen ange typ av träning.");
                
                if (WorkoutDuration <= TimeSpan.Zero)
                    errorMessage.AppendLine("Varaktigheten måste vara större än 0.");

                if (WorkoutCaloriesBurned <= 0)
                    errorMessage.AppendLine("Kaloriförbränning måste vara ett positivt tal.");

                if (string.IsNullOrWhiteSpace(WorkoutNotes))
                    errorMessage.AppendLine("Anteckningar om träningspasset får inte vara tomma.");

                if (WorkoutType == "Strength")
                {
                    if (WorkoutReps <= 0)
                        errorMessage.AppendLine("Vänligen angen antal repetiotioner");
                    if (WorkoutSets <= 0)
                        errorMessage.AppendLine("Vänligen ange antal sets");
                }
                else if (WorkoutType == "Cardio")
                {
                    if (WorkoutDistance <= 0)
                        errorMessage.AppendLine("Vänligen ange ett avstånd större än 0");
                }


                if (errorMessage.Length > 0)
                {
                    MessageBox.Show(errorMessage.ToString(), "Fel vid sparande", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; // Avbryter sparandet
                }
            }


            //Workout newWorkout = WorkoutType == "Strength"
            //    ? new StrengthWorkout(WorkoutDate, WorkoutType, WorkoutDuration, WorkoutNotes, "Anna", WorkoutReps ?? 0, WorkoutSets ?? 0)
            //    : new CardioWorkout(WorkoutDate, WorkoutDuration, WorkoutDistance ?? 0, WorkoutNotes, "Anna");

            _workoutsWindow.Show();
            //OpenWorkoutWindow();
        }
      
        private void Cancel()
        {
            _workoutsWindow.Show();

        }
        private void OpenWorkoutWindow()
        {
            var workoutsWindow = new WorkoutsWindow();
            Application.Current.MainWindow = workoutsWindow; 
            workoutsWindow.Show();

            CloseAddWorkoutWindow();
        }
        private void CloseAddWorkoutWindow()
        {
            var addWorkoutWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);
            addWorkoutWindow?.Close();
        }

    }


}

