using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitTrack2._0.Model
{
    public class CardioWorkout: Workout
    {
        //Egenskap
        public new int Distance { get; set; }

        public CardioWorkout(DateTime date, TimeSpan duration, int distance, string notes, string owner, int id)
            : base(date, "Cardio", duration, notes, owner, id)
        {
            Distance = distance;
        }
        public override int CalculateCaloriesBurned()
        {
            if (Duration.TotalMinutes <= 0) return 0;

            
            double durationInHours = Duration.TotalHours;
            return (int)(Distance * 70 * durationInHours);
        }
        
    }
}
