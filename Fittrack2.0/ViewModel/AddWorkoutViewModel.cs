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

        private readonly Window _addworkoutsWindow;
        private Visibility strenghtWorkoutVistbilty =Visibility.Collapsed;
        private readonly ManageUser _userManager=ManageUser.Instance;
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
        private int _workoutReps;
        public int WorkoutReps
        {
            get => _workoutReps;
            set
            {
                _workoutReps = value;
                CalculateCalories();
                OnPropertyChanged();
            }
        }
        private int _workoutSets;
        public int WorkoutSets
        {
            get => _workoutSets;
            set
            {
                _workoutSets = value;
                CalculateCalories();
                OnPropertyChanged();
            }
            


        }
        private int _workoutDistance;
        public int WorkoutDistance
        {
            get => _workoutDistance;
            set
            {
                _workoutDistance = value;
                CalculateCalories();
                OnPropertyChanged();
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

   


        public AddWorkoutViewModel( Window addworkoutsWindow, Workout workoutToCopy = null)
        {
            _addworkoutsWindow = addworkoutsWindow;
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
                Workout = new StrengthWorkout(WorkoutDate, WorkoutType, WorkoutDuration, WorkoutNotes, _userManager.LoggedInUser.Username, WorkoutReps , WorkoutSets);
                WorkoutDistance = 0;
            }
            else if (WorkoutType == "Cardio")
            {
                Workout = new CardioWorkout(WorkoutDate, WorkoutDuration, WorkoutDistance, WorkoutNotes,_userManager.LoggedInUser.Username);
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
            if(workoutType == "Strength")
            {
                StrengthWorkout workout = new StrengthWorkout(date:WorkoutDate,type:WorkoutType,duration:WorkoutDuration,notes:WorkoutNotes,username:_userManager.LoggedInUser.Username,reps:WorkoutReps,sets:WorkoutSets);
                _userManager.LoggedInUser.UserWorkouts.Add(workout);
            }
            if (workoutType == "Cardio")
            {
                CardioWorkout workout = new CardioWorkout(date:WorkoutDate,duration:WorkoutDuration,distance: WorkoutDistance,notes: WorkoutNotes,owner:_userManager.LoggedInUser.Username);
                _userManager.LoggedInUser.UserWorkouts.Add(workout);
            }

            


           
            OpenWorkoutWindow();
        }
      
        private void Cancel()
        {
            OpenWorkoutWindow();

        }
        private void OpenWorkoutWindow()
        {
            var workoutsWindow = new WorkoutsWindow();
            Application.Current.MainWindow = workoutsWindow;
            workoutsWindow.Show();
            _addworkoutsWindow.Close();

        }
    

    }


}

