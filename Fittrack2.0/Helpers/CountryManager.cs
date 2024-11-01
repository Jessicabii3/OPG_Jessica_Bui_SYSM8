using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitTrack2._0.Helpers
{
    public class CountryManager
    {
        // Hämtar en lista med alla tillgängliga länder, sorterade i alfabetisk ordning
        public static ObservableCollection<string> GetCountries()
        {
            var countries = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(culture => new RegionInfo(culture.Name).DisplayName)
                .Distinct() // Tar bort dubbletter
                .OrderBy(country => country) // Sorterar länder i alfabetisk ordning
                .ToList();

            return new ObservableCollection<string>(countries); // Returnerar listan som en ObservableCollection
        }
    }
}
