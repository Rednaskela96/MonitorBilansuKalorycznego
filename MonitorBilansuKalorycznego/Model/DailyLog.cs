using System;
using System.Collections.Generic;
using System.Linq;
using MonitorBilansuKalorycznego.Interfaces;

namespace MonitorBilansuKalorycznego.Model
{
    // IMPLEMENTACJA INTERFEJSU — DailyLog implementuje ICalorieCalculable.
    public class DailyLog : ICalorieCalculable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; } = DateTime.Today;

        // KOLEKCJE GENERYCZNE — List<T> przechowujące wpisy posiłków i aktywności
        public List<MealEntry> Meals { get; set; } = new List<MealEntry>();
        public List<ActivityEntry> Activities { get; set; } = new List<ActivityEntry>();

        public string Notes { get; set; } = string.Empty;

        // Metody zarządzające kolekcjami (dodawanie/usuwanie elementów)
        public void AddMeal(MealEntry meal) => Meals.Add(meal);
        public void RemoveMeal(MealEntry meal) => Meals.Remove(meal);
        public void AddActivity(ActivityEntry activity) => Activities.Add(activity);
        public void RemoveActivity(ActivityEntry activity) => Activities.Remove(activity);

        // LINQ Sum() — sumuje kalorie ze wszystkich posiłków
        public double GetTotalCaloriesConsumed()
        {
            return Meals.Sum(m => m.CalculateCalories());  // <-- LINQ + POLIMORFIZM (wywołanie override)
        }

        // LINQ Sum() — sumuje spalone kalorie ze wszystkich aktywności
        public double GetTotalCaloriesBurned()
        {
            return Activities.Sum(a => a.CalculateCalories());  // <-- POLIMORFIZM (inna implementacja override)
        }

        // Implementacja interfejsu ICalorieCalculable — bilans netto
        public double CalculateCalories()
        {
            return GetTotalCaloriesConsumed() - GetTotalCaloriesBurned();
        }

        // Ile kalorii pozostało do celu dziennego?
        public double GetRemainingCalories(double dailyGoal)
        {
            return dailyGoal - CalculateCalories();
        }
    }
}
