using System;
using System.Collections.Generic;
using System.Linq;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.Logic
{
    /*StatisticsCalculator — klasa statyczna z metodami obliczeniowymi.
    użycie LINQ: Average, Sum, Count, Where, OrderBy, Take, Skip.*/
    public static class StatisticsCalculator
    {
        // LINQ Average() — średnia spożytych kalorii
        public static double CalculateAverageConsumed(List<DailyLog> logs)
        {
            if (logs == null || !logs.Any()) return 0;
            return logs.Average(l => l.GetTotalCaloriesConsumed());
        }

        // LINQ Average() — średnia spalonych kalorii
        public static double CalculateAverageBurned(List<DailyLog> logs)
        {
            if (logs == null || !logs.Any()) return 0;
            return logs.Average(l => l.GetTotalCaloriesBurned());
        }

        // LINQ Sum() — suma spożytych kalorii
        public static double CalculateTotalConsumed(List<DailyLog> logs)
        {
            if (logs == null || !logs.Any()) return 0;
            return logs.Sum(l => l.GetTotalCaloriesConsumed());
        }

        // LINQ Sum() — suma spalonych kalorii
        public static double CalculateTotalBurned(List<DailyLog> logs)
        {
            if (logs == null || !logs.Any()) return 0;
            return logs.Sum(l => l.GetTotalCaloriesBurned());
        }

        // LINQ Count() z predykatem — ile dni bilans <= cel
        public static int GetDaysOnGoal(List<DailyLog> logs, double dailyGoal)
        {
            if (logs == null || !logs.Any()) return 0;
            return logs.Count(l => l.CalculateCalories() <= dailyGoal);
        }

        // Algorytm streak — ciągła seria dni "na celu" licząc od dziś wstecz
        // LINQ OrderByDescending() — sortowanie malejące po dacie
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

        // LINQ Where() + OrderBy() — filtrowanie logów za ostatnie N dni
        public static List<DailyLog> GetLogsForPeriod(List<DailyLog> allLogs, int days)
        {
            if (allLogs == null) return new List<DailyLog>();
            var startDate = DateTime.Today.AddDays(-(days - 1));
            return allLogs
                .Where(l => l.Date.Date >= startDate.Date && l.Date.Date <= DateTime.Today)
                .OrderBy(l => l.Date)
                .ToList();
        }
    }
}
