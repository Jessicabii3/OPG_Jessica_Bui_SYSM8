using FitTrack2._0.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitTrack2._0.ViewModel;

namespace FitTrack2._0.Model
{
    public class ManageUser:BaseViewModel
    {
        
        
       // Trådsäker Singleton-instans av ManageUser
        private static readonly Lazy<ManageUser> _instance = new Lazy<ManageUser>(() => new ManageUser());

        // Publik åtkomst till Singleton-instansen
        public static ManageUser Instance => _instance.Value;

        // Lista över registrerade användare
        public static ObservableCollection<User> RegisteredUsers { get; set; } = new ObservableCollection<User>();

        
        // Egenskap för att hålla koll på nuvarande inloggade användare
        private User? _loggedInUser;
        public User? LoggedInUser
        {
            get => _loggedInUser;
            private set
            {
                _loggedInUser = value;
                OnPropertyChanged(nameof(LoggedInUser));
                OnPropertyChanged(nameof(IsLoggedIn));
                OnPropertyChanged(nameof(CurrentUserName));
                OnPropertyChanged(nameof(IsCurrentUserAdmin));
            }
        }

        // Standardadministratör som skapas när applikationen startar
        private readonly AdminUser _defaultAdmin;
        // Kontrollera om den nuvarande inloggade användaren är en admin
        public bool IsCurrentUserAdmin => LoggedInUser is AdminUser;
        public string CurrentUserName => LoggedInUser?.Username ?? "Ingen användare";
        // Kontroll för inloggningsstatus
        public bool IsLoggedIn => LoggedInUser != null;

        private ManageUser()
        {
            _defaultAdmin = new AdminUser("Admin1", "Password1234!", "Sweden", "Fave Color?", "Blue");
            var user1 = new User("Anna", "Password123!", "Sweden", "What's my cats name?", "Miso");
            var user2 = new User("Theo", "Password456!", "Norway", "What's my dogs name?", "Bog");

            user1.UserWorkouts.Add(new CardioWorkout(DateTime.Parse("2024-10-30"), TimeSpan.FromMinutes(60), 6, "Morning run", user1.Username));
            user1.UserWorkouts.Add(new StrengthWorkout(DateTime.Parse("2024-10-31"), "Upper body", TimeSpan.FromMinutes(45), "Strength training", user1.Username,4,6));

            user2.UserWorkouts.Add(new CardioWorkout(DateTime.Parse("2024-10-30"), TimeSpan.FromMinutes(60), 6, "Jogging", user2.Username));
            user2.UserWorkouts.Add(new StrengthWorkout(DateTime.Parse("2024-10-31"), "Leg Day", TimeSpan.FromMinutes(45), "Strength training", user2.Username,3,6));


            RegisteredUsers.Add(_defaultAdmin);
            RegisteredUsers.Add(user1);
            RegisteredUsers.Add(user2);

        }
        // Hämtar alla träningspass för alla användare
        public IEnumerable<Workout> GetAllWorkouts()
        {
            return RegisteredUsers.SelectMany(user => user.UserWorkouts);
        }

        // Hämtar träningspass för en specifik användare baserat på användarnamn
        public IEnumerable<Workout> GetWorkoutsForUser(string username)
        {
            var user = GetUser(username);
            return user?.UserWorkouts ?? Enumerable.Empty<Workout>();
        }
        public bool ValidateCredentials(string username, string password)
        {
            var user = RegisteredUsers.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                LoggedInUser = user; // Sätter den nuvarande inloggade användaren
                return true;
            }
            LoggedInUser = null;
            return false;
        }
        // Lägger till en ny användare med validering av användarnamn och lösenord
        public bool AddUser(User user)
        {
            // Validera användarnamn
            if (!ValidationHelper.IsValidUsername(user.Username))
            {
                throw new ArgumentException("Användarnamnet måste vara minst 3 tecken långt.");
            }

            // Kontrollera om användarnamnet redan är taget
            if (IsUsernameTaken(user.Username))
            {
                throw new ArgumentException("Användarnamnet är redan taget.");
            }

            // Validera lösenord
            if (!ValidationHelper.IsValidPassword(user.Password))
            {
                throw new ArgumentException("Lösenordet måste vara minst 8 tecken, innehålla minst en siffra och ett specialtecken.");
            }

            RegisteredUsers.Add(user);
            return true;
        }
        // Tar bort en användare från listan över registrerade användare
        public void RemoveUser(User user)
        {
            if (user == LoggedInUser)
            {
                LoggedInUser = null; // Återställer den inloggade användaren om det är samma användare som tas bort
            }
            RegisteredUsers.Remove(user);
        }

        // Hämtar en användare baserat på användarnamn
        public User? GetUser(string username)
        {
            return RegisteredUsers.FirstOrDefault(u => u.Username == username);
        }

        // Returnerar alla användare i listan
        public ObservableCollection<User> GetAllUsers()
        {
            return RegisteredUsers;
        }

        // Kontrollerar om ett användarnamn redan är taget
        public bool IsUsernameTaken(string username)
        {
            return RegisteredUsers.Any(u => u.Username == username);
        }
        public void SignOut()
        {
            LoggedInUser = null;
        }

    }
}
