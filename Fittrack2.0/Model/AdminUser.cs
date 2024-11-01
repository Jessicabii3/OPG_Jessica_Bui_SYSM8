using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitTrack2._0.Model
{
    public class AdminUser:User
    {
        // Konstruktor
        public AdminUser(string username, string password, string country, string securityQuestion, string securityAnswer)
            : base(username, password, country, securityQuestion, securityAnswer, isAdmin: true)
        {
        }
        // Metod för att hämta information om alla användares träningspass
        public ObservableCollection<UserWorkoutInfo> GetAllWorkoutsInfo(ManageUser manageUser)
        {
            // Samlar alla träningspass från alla användare
            var workoutDetails = new ObservableCollection<UserWorkoutInfo>(
                manageUser.GetAllUsers()
                    .SelectMany(user => user.UserWorkouts,
                                (user, workout) => new UserWorkoutInfo(user.Username, workout.Type, workout.Duration))
            );

            return workoutDetails;
        }
        // Metod för att ta bort ett träningspass för en specifik användare
        public void RemoveWorkoutForUser(User user, Workout workout)
        {
            if (user?.UserWorkouts != null && workout != null)
            {
                user.UserWorkouts.Remove(workout);
            }
        }
        // Metod för att hämta en specifik användare baserat på användarnamn
        public User? GetUser(string username, ManageUser manageUser)
        {
            return manageUser.GetUser(username); // Admin använder ManageUser för att hitta användaren
        }
    }
}
