using System;

namespace MonitorBilansuKalorycznego.Model
{
    public class PhysicalActivity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public double CaloriesPerHour { get; set; }

        public double CalculateCaloriesBurned(TimeSpan duration)
        {
            return (duration.TotalMinutes * (CaloriesPerHour / 60.0));
        }

        // --- NOWA METODA ---
        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) return false;
            if (CaloriesPerHour <= 0) return false; // Spalanie musi być dodatnie
            return true;
        }
    }
}