using System;

namespace MonitorBilansuKalorycznego.Model
{
    // Baza dla MealEntry i ActivityEntry.
    public abstract class BaseEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        // Każda klasa potomna liczy kalorie inaczej
        public abstract double CalculateCalories();

        // Metoda wirtualna — MA domyślną implementację, ale MOŻE być nadpisana (override).
        public virtual string GetSummary() => $"{CalculateCalories():N0} kcal";
    }
}
