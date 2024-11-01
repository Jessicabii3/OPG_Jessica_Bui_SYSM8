using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitTrack2._0.Model
{
    public class UserWorkoutInfo
    {
        ////Egenskaper
        public string UserName { get; set; }
        public string WorkoutType { get; set; }
        public TimeSpan Duration { get; set; }

        // Konstruktor för att initialisera alla egenskaper
        public UserWorkoutInfo(string username, string workoutType, TimeSpan duration)
        {
            UserName = username;
            WorkoutType = workoutType;
            Duration = duration;
        }

    }
}
