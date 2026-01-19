using System;
using System.Collections.Generic;
using System.Linq;
using MonitorBilansuKalorycznego.Model;
using MonitorBilansuKalorycznego.Interfaces;

namespace MonitorBilansuKalorycznego.Logic
{
    public static class StatisticsCalculator
    {
        // Oblicz średnią spożytych kalorii z listy dni
        public static double CalculateAverageConsumed(List<DailyLog> logs)
        {
            if (logs == null || !logs.Any()) return 0;
            return logs.Average(l => l.GetTotalCaloriesConsumed());
        }

        // Oblicz średnią spalonych kalorii
        public static double CalculateAverageBurned(List<DailyLog> logs)
        {
            if (logs == null || !logs.Any()) return 0;
            return logs.Average(l => l.GetTotalCaloriesBurned());
        }

        // Oblicz sumę kalorii (np. dla tygodnia)
        public static double CalculateTotalConsumed(List<DailyLog> logs)
        {
            if (logs == null || !logs.Any()) return 0;
            return logs.Sum(l => l.GetTotalCaloriesConsumed());
        }

        // Wykrywanie trendu (czy użytkownik je coraz więcej, czy mniej)
        public static string GetTrend(List<DailyLog> logs)
        {
            if (logs == null || logs.Count < 2) return "Brak danych";

            // Porównujemy średnią z pierwszej połowy okresu do drugiej
            int mid = logs.Count / 2;
            var firstHalf = logs.Take(mid).Average(l => l.CalculateCalories());
            var secondHalf = logs.Skip(mid).Average(l => l.CalculateCalories());

            if (secondHalf > firstHalf) return "Rosnący"; // Nadwyżka rośnie
            if (secondHalf < firstHalf) return "Malejący"; // Deficyt się pogłębia
            return "Stabilny";
        }
    }
}