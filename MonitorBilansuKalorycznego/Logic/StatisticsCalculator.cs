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

        public static double CalculateTotalBurned(List<DailyLog> logs)
        {
            if (logs == null || !logs.Any()) return 0;
            return logs.Sum(l => l.GetTotalCaloriesBurned());
        }

        public static int GetDaysOnGoal(List<DailyLog> logs, double dailyGoal)
        {
            if (logs == null || !logs.Any()) return 0;
            return logs.Count(l => l.CalculateCalories() <= dailyGoal);
        }

        public static int GetCurrentStreak(List<DailyLog> allLogs, double dailyGoal)
        {
            if (allLogs == null || !allLogs.Any()) return 0;

            var sorted = allLogs.OrderByDescending(l => l.Date.Date).ToList();
            int streak = 0;
            var expectedDate = DateTime.Today;

            foreach (var log in sorted)
            {
                if (log.Date.Date != expectedDate.Date) break;
                if (log.CalculateCalories() > dailyGoal) break;
                streak++;
                expectedDate = expectedDate.AddDays(-1);
            }

            return streak;
        }

        public static List<DailyLog> GetLogsForPeriod(List<DailyLog> allLogs, int days)
        {
            if (allLogs == null) return new List<DailyLog>();
            var startDate = DateTime.Today.AddDays(-(days - 1));
            return allLogs
                .Where(l => l.Date.Date >= startDate.Date && l.Date.Date <= DateTime.Today)
                .OrderBy(l => l.Date)
                .ToList();
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