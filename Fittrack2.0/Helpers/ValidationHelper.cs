using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FitTrack2._0.Helpers
{
    public class ValidationHelper
    {
        public static bool IsValidPassword(string password)
        {
            if (password.Length < 8) return false;
            if (!Regex.IsMatch(password, @"\d")) return false; // Minst en siffra
            if (!Regex.IsMatch(password, @"[A-Z]")) return false;
            if (!Regex.IsMatch(password, @"[\W_]")) return false; // Minst ett specialtecken
            return true;
        }
        public static bool IsValidUsername(string username)
        {
            return !string.IsNullOrEmpty(username) && username.Length >= 3;
        }
        public static int RandomId()
        {
            
            Random random = new Random();
            return random.Next(100, 999);
        }
    }
}
