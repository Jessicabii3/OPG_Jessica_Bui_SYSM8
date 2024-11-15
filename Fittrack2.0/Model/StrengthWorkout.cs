using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitTrack2._0.Model
{
   public class StrengthWorkout:Workout
    {
        // Egenskaper för repetitions och set
        public  int Reps { get; set; }
        public int Sets { get; set; }
        public StrengthWorkout(DateTime date, string type, TimeSpan duration, string notes, string username, int reps, int sets, int id)
            : base(date, "Strength", duration, notes, username, id)
        {
            Reps = reps;
            Sets = sets;
        }

        public override int CalculateCaloriesBurned()
        {
            if (Duration.TotalMinutes <= 0) return 0;

            // Ungefär 5 kalorier per rep
            double durationInHours = Duration.TotalHours;
            return (int)( Reps * Sets * 5);
        }
        

    }
}
