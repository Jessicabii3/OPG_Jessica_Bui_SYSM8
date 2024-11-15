using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitTrack2._0.Model
{
    public abstract class Workout
    {
        // Egenskaper för träningspassets datum, typ, varaktighet, brända kalorier och anteckningar
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public TimeSpan Duration { get; set; }
        public int CaloriesBurned { get; set; }
        public string Notes { get; set; }
        public string Owner { get; set; }  // Ägaren till träningspasset(användarnamn)
        public int Id { get; set; }
        //Konstruktor
        public Workout(DateTime date, string type, TimeSpan duration, string notes, string owner, int id)
        {
            Date = date;
            Type = type;
            Duration = duration;
            Notes = notes;
            Owner = owner;
            Id = id;
        }
      
        public virtual int CalculateCaloriesBurned()
        {
            return CaloriesBurned;
        }
    }
}
