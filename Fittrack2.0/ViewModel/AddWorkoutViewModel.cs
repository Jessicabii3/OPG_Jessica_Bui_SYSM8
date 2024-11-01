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
using Fittrack2._0.View;
using System.Windows;

namespace Fittrack2._0.ViewModel
{
    public class AddWorkoutViewModel : BaseViewModel
    {
      
        // Allmänna egenskaper för träningspass
        public DateTime WorkoutDate { get; set; } = DateTime.Today;
        private string workoutType;
        public string WorkoutType
        {
            get => workoutType;
            set
            {
                workoutType = value;
                OnPropertyChanged();
                UpdateWorkoutType(); // Uppdatera Workout beroende på typ
            }
        }


        // Egenskaper
        public TimeSpan WorkoutDuration { get; set; }
        public int WorkoutCaloriesBurned { get; set; }
        public string WorkoutNotes { get; set; }= string.Empty;

        // Specifika egenskaper för Strength och Cardio
        public int WorkoutReps { get; set; }
        public int WorkoutSets { get; set; }
        public int WorkoutDistance { get; set; }
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

        public AddWorkoutViewModel() : this(null) { }


        public AddWorkoutViewModel(Workout workoutToCopy)
        {
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
                }
                else if (workoutToCopy is CardioWorkout cardioWorkout)
                {
                    WorkoutDistance = cardioWorkout.Distance;
                    Workout = cardioWorkout;
                }
            }
            SaveCommand = new RelayCommand(_ => SaveWorkout(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        private void UpdateWorkoutType()
        {
            if (WorkoutType == "Strength")
            {
                Workout = new StrengthWorkout(WorkoutDate, WorkoutType, WorkoutDuration, WorkoutNotes, "Anna", WorkoutReps, WorkoutSets);
            }
            else if (WorkoutType == "Cardio")
            {
                Workout = new CardioWorkout(WorkoutDate, WorkoutDuration, WorkoutDistance, WorkoutNotes, "Anna");
            }
        }
        // Validering: Kontrollerar om alla fält är korrekt ifyllda
        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(WorkoutType) &&
                   WorkoutDuration > TimeSpan.Zero &&
                   WorkoutCaloriesBurned > 0 &&
                   !string.IsNullOrWhiteSpace(WorkoutNotes);
        }
        private void SaveWorkout()
        {
            Workout newWorkout = WorkoutType == "Strength"
                ? new StrengthWorkout(WorkoutDate, WorkoutType, WorkoutDuration, WorkoutNotes, "Anna", WorkoutReps, WorkoutSets)
                : new CardioWorkout(WorkoutDate, WorkoutDuration, WorkoutDistance, WorkoutNotes, "Anna");

            WorkoutSaved?.Invoke(newWorkout);
            OpenWorkoutWindow();
        }
        private void Cancel()
        {
            OpenWorkoutWindow();
        }
        private void OpenWorkoutWindow()
        {
            var workoutsWindow = new WorkoutsWindow();
            Application.Current.MainWindow = workoutsWindow; // Sätter WorkoutsWindow som huvudfönster
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
