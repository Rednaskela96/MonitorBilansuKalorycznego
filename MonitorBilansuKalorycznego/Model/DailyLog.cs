using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitorBilansuKalorycznego.Interfaces;

namespace MonitorBilansuKalorycznego.Model
{
    public class DailyLog : ICalorieCalculable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; } = DateTime.Today;

        // Listy wpisów (posiłki i aktywności)
        public List<MealEntry> Meals { get; set; } = new List<MealEntry>();
        public List<ActivityEntry> Activities { get; set; } = new List<ActivityEntry>();

        public string Notes { get; set; } = string.Empty;

        // Metody zarządzające listami (zgodnie z diagramem klas)
        public void AddMeal(MealEntry meal)
        {
            Meals.Add(meal);
        }

        public void RemoveMeal(MealEntry meal)
        {
            Meals.Remove(meal);
        }

        public void AddActivity(ActivityEntry activity)
        {
            Activities.Add(activity);
        }

        // --- LOGIKA BIZNESOWA (OBLICZENIA) ---

        // 1. Ile zjedliśmy łącznie?
        public double GetTotalCaloriesConsumed()
        {
            return Meals.Sum(m => m.CalculateCalories());
        }

        // 2. Ile spaliliśmy łącznie (ćwiczenia)?
        public double GetTotalCaloriesBurned()
        {
            return Activities.Sum(a => a.CalculateCaloriesBurned());
        }

        // 3. Bilans netto (Zjedzone - Spalone)
        public double CalculateCalories()
        {
            return GetTotalCaloriesConsumed() - GetTotalCaloriesBurned();
        }

        // 4. Ile pozostało do celu? (Cel przekazujemy jako parametr, bo jest w UserProfile)
        public double GetRemainingCalories(double dailyGoal)
        {
            return dailyGoal - CalculateCalories();
        }
    }
}
