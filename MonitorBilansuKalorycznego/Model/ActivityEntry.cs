using System;

namespace MonitorBilansuKalorycznego.Model
{
    // Reprezentuje wpis aktywności fizycznej w dzienniku (ćwiczenie + czas trwania).
    public class ActivityEntry : BaseEntry
    {
        public DateTime StartTime { get; set; } = DateTime.Now;

        //ActivityEntry posiada referencję do PhysicalActivity
        public PhysicalActivity Activity { get; set; } = new PhysicalActivity();

        // Czas trwania ćwiczenia (typ TimeSpan — wbudowany typ .NET)
        public TimeSpan Duration { get; set; }

        // OVERRIDE metody abstrakcyjnej — inny sposób obliczania kalorii niż w MealEntry = POLIMORFIZM
        public override double CalculateCalories()
        {
            if (Activity == null) return 0;
            return Activity.CalculateCaloriesBurned(Duration);
        }

        // OVERRIDE metody wirtualnej — opis aktywności z czasem trwania
        public override string GetSummary()
            => $"{Activity.Name} — {Duration.TotalMinutes:N0} min ({CalculateCalories():N0} kcal)";

        public double CaloriesDisplay => CalculateCalories();
    }
}
