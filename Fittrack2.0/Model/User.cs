using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitTrack2._0.Model
{
    public class User : Person
    {
        //Egenskap för användarens land
        public string? Country { get; set; }
        //Egenskap för säkerhetsfråga
        public string? SecurityQuestion { get; set; }
        //Egenskap för säkerhetssvar
        public string? SecurityAnswer { get; set; }
        //Lista över användarens träningspass
        public ObservableCollection<Workout> UserWorkouts { get; set; }
        public bool IsAdmin { get; set; } // om det är AdminUser
        public event EventHandler? CanLogin;
        //Konstruktor som initierar egenskaperna
        public User(string username, string password, string country, string securityQuestion, string securityAnswer, bool isAdmin = false) : base(username, password)
        {
            Country = country;
            SecurityQuestion = securityQuestion;
            SecurityAnswer = securityAnswer;
            IsAdmin = isAdmin;

            UserWorkouts = new ObservableCollection<Workout>();
        }
        // Metod för inloggning, triggar CanLogin-eventet
        public override void SignIn()
        {
            CanLogin?.Invoke(this, EventArgs.Empty);
        }

    }

}
