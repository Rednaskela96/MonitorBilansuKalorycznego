using System;
using System.Collections.Generic;
using System.Linq;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.Logic
{
    public class WeeklyReport
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        private List<DailyLog> _weeklyLogs;
        private double _dailyGoal;

        public WeeklyReport(List<DailyLog> allLogs, double dailyGoal)
        {
            // Ustaw zakres na ostatnie 7 dni
            EndDate = DateTime.Today;
            StartDate = EndDate.AddDays(-6);
            _dailyGoal = dailyGoal;

            // Pobierz logi tylko z tego tygodnia
            _weeklyLogs = allLogs
                .Where(l => l.Date.Date >= StartDate.Date && l.Date.Date <= EndDate.Date)
                .OrderBy(l => l.Date)
                .ToList();
        }

        // Metoda zwracająca dane gotowe do wyświetlenia na Dashboardzie
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

        // Jaki procent dni spełnił cel kaloryczny?
        private double CalculateGoalAchievementRate()
        {
            if (!_weeklyLogs.Any()) return 0;

            int successDays = _weeklyLogs.Count(l => l.GetTotalCaloriesConsumed() <= _dailyGoal);
            return (double)successDays / _weeklyLogs.Count * 100.0;
        }
    }

    // Prosta klasa DTO (Data Transfer Object) do przekazania danych do widoku
    public class ReportData
    {
        // POPRAWKA: Inicjalizacja pustym tekstem
        public string Title { get; set; } = string.Empty;

        public double AverageConsumed { get; set; }
        public double TotalBurned { get; set; }
        public int DaysTracked { get; set; }
        public double GoalAchievementRate { get; set; }
    }
}