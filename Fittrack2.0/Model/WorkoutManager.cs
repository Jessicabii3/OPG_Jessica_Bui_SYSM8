using FitTrack2._0.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fittrack2._0.Model
{
    public class WorkoutManager
    {
        private static WorkoutManager _instance;
        public static WorkoutManager Instance => _instance ??= new WorkoutManager();

        public ObservableCollection<Workout> Workouts { get; private set; }

        private WorkoutManager()
        {
            Workouts = new ObservableCollection<Workout>();
        }

        public void AddWorkout(Workout workout)
        {
            if (workout != null)
            {
                Workouts.Add(workout);
            }
        }
    }
}
