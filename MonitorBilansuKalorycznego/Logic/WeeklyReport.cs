using System;
using System.Collections.Generic;
using System.Linq;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.Logic
{
    // WeeklyReport — generuje raport tygodniowy z logów użytkownika.
    public class WeeklyReport
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        
        private List<DailyLog> _weeklyLogs;
        private double _dailyGoal;

        public WeeklyReport(List<DailyLog> allLogs, double dailyGoal)
        {
            EndDate = DateTime.Today;
            StartDate = EndDate.AddDays(-6);
            _dailyGoal = dailyGoal;

            // LINQ Where() + OrderBy() — filtrowanie i sortowanie logów
            _weeklyLogs = allLogs
                .Where(l => l.Date.Date >= StartDate.Date && l.Date.Date <= EndDate.Date)
                .OrderBy(l => l.Date)
                .ToList();
        }

        // Metoda zwracająca DTO (Data Transfer Object) z danymi raportu
        public ReportData GenerateReport()
        {
            return new ReportData
            {
                Title = $"Raport tygodniowy ({StartDate:dd.MM} - {EndDate:dd.MM})",
                AverageConsumed = StatisticsCalculator.CalculateAverageConsumed(_weeklyLogs),
                TotalBurned = StatisticsCalculator.CalculateTotalBurned(_weeklyLogs),
                DaysTracked = _weeklyLogs.Count,
                GoalAchievementRate = CalculateGoalAchievementRate()
            };
        }

        // LINQ Count() z predykatem lambda — ile dni spełniło cel kaloryczny
        private double CalculateGoalAchievementRate()
        {
            if (!_weeklyLogs.Any()) return 0;
            int successDays = _weeklyLogs.Count(l => l.GetTotalCaloriesConsumed() <= _dailyGoal);
            return (double)successDays / _weeklyLogs.Count * 100.0;
        }
    }

    // DTO (Data Transfer Object) — klasa do przekazania danych do widoku
    public class ReportData
    {
        public string Title { get; set; } = string.Empty;
        public double AverageConsumed { get; set; }
        public double TotalBurned { get; set; }
        public int DaysTracked { get; set; }
        public double GoalAchievementRate { get; set; }
    }
}
