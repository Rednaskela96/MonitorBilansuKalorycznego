using System;

namespace MonitorBilansuKalorycznego.Model
{
    /* PhysicalActivity — definicja aktywności fizycznej (np. bieganie, pływanie).
    Przechowuje spalanie kalorii na godzinę i oblicza kalorie dla danego czasu.*/
    public class PhysicalActivity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        //dane (kcal/h) ukryte za właściwością, logika w metodzie
        public double CaloriesPerHour { get; set; }

        // Oblicza spalone kalorie na podstawie czasu trwania (TimeSpan)
        public double CalculateCaloriesBurned(TimeSpan duration)
        {
            return (duration.TotalMinutes * (CaloriesPerHour / 60.0));
        }

        // Walidacja danych — sprawdza poprawność przed zapisem
        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) return false;
            if (CaloriesPerHour <= 0) return false;
            return true;
        }
    }
}
