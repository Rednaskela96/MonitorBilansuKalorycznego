using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MonitorBilansuKalorycznego.Data;
using MonitorBilansuKalorycznego.Logic;
using MonitorBilansuKalorycznego.Model;
using SkiaSharp;

namespace MonitorBilansuKalorycznego.View
{
    public partial class DashboardView : UserControl
    {
        private readonly ApplicationData _appData;

        public DashboardView(ApplicationData appData)
        {
            InitializeComponent();
            _appData = appData;
            // ZDARZENIE WPF — ładuje statystyki po załadowaniu kontrolki
            Loaded += (_, _) => LoadStatistics();
        }

        // Główna metoda ładująca wszystkie 4 karty statystyk
        private void LoadStatistics()
        {
            var dailyGoal = _appData.CurrentUser.GetDailyCalorieGoal();
            var allLogs = _appData.LogManager.Items;

            LoadTodayBalance(allLogs, dailyGoal);
            LoadWeeklyChart(allLogs);
            LoadMonthlyReport(allLogs, dailyGoal);
            LoadYearlyTrend(allLogs, dailyGoal);
        }

        // Karta 1 — dzisiejszy bilans + wykres donut (PieChart)
        private void LoadTodayBalance(List<DailyLog> allLogs, double dailyGoal)
        {
            // LINQ FirstOrDefault() — szukanie dziennika z dzisiejszą datą
            var todayLog = allLogs.FirstOrDefault(l => l.Date.Date == DateTime.Today);

            // Operator ?. (null-conditional) + ?? (null-coalescing)
            double consumed = todayLog?.GetTotalCaloriesConsumed() ?? 0;
            double burned = todayLog?.GetTotalCaloriesBurned() ?? 0;
            double netBalance = consumed - burned;
            double remaining = dailyGoal - netBalance;

            // Interpolacja stringów ($"...") — formatowanie wyświetlanych wartości
            TxtBalanceValue.Text = $"{netBalance:N0}";
            TxtBalanceGoal.Text = $"z {dailyGoal:N0} kcal";
            TxtConsumedToday.Text = $"{consumed:N0} kcal";
            TxtBurnedToday.Text = $"-{burned:N0} kcal";

            if (remaining >= 0)
            {
                TxtNetBalance.Text = $"Pozostało {remaining:N0}kcal";
                TxtNetBalance.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71"));
            }
            else
            {
                TxtNetBalance.Text = $"Nadwyżka {Math.Abs(remaining):N0}kcal";
                TxtNetBalance.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
            }

            // Wykres donut
            double filledValue = Math.Min(netBalance, dailyGoal);
            if (filledValue < 0) filledValue = 0;
            double emptyValue = Math.Max(dailyGoal - filledValue, 0);
            if (dailyGoal <= 0) { filledValue = 0; emptyValue = 1; }
            
            PieChartBalance.Series = new ISeries[]
            {
                new PieSeries<double>
                {
                    Values = new[] { filledValue },
                    InnerRadius = 60,
                    Fill = new SolidColorPaint(new SKColor(249, 115, 22)),
                    Pushout = 0,
                    MaxRadialColumnWidth = 18
                },
                new PieSeries<double>
                {
                    Values = new[] { emptyValue },
                    InnerRadius = 60,
                    Fill = new SolidColorPaint(new SKColor(230, 230, 230)),
                    Pushout = 0,
                    MaxRadialColumnWidth = 18
                }
            };
        }

        // Karta 2 — wykres tygodniowy (słupki poziome, ręcznie rysowane Border)
        private void LoadWeeklyChart(List<DailyLog> allLogs)
        {
            // Wywołanie metody statycznej z klasy StatisticsCalculator
            var weekLogs = StatisticsCalculator.GetLogsForPeriod(allLogs, 7);

            var today = DateTime.Today;
            var monday = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));

            // KOLEKCJA — tablice kontrolek WPF (Border[], TextBlock[])
            var bars = new Border[] { Bar0, Bar1, Bar2, Bar3, Bar4, Bar5, Bar6 };
            var vals = new TextBlock[] { Val0, Val1, Val2, Val3, Val4, Val5, Val6 };

            double maxCal = 1;

            var dailyValues = new double[7];
            for (int i = 0; i < 7; i++)
            {
                var date = monday.AddDays(i);
                // LINQ FirstOrDefault() — szukanie loga na konkretny dzień
                var log = weekLogs.FirstOrDefault(l => l.Date.Date == date.Date);
                dailyValues[i] = log?.GetTotalCaloriesConsumed() ?? 0;
                if (dailyValues[i] > maxCal) maxCal = dailyValues[i];
            }

            for (int i = 0; i < 7; i++)
            {
                double ratio = dailyValues[i] / maxCal;
                bars[i].Width = Math.Max(ratio * 150, dailyValues[i] > 0 ? 5 : 0);
                vals[i].Text = dailyValues[i] > 0 ? $"{dailyValues[i]:N0}" : "—";
            }
        }

        // Karta 3 — raport miesięczny (średnia, suma, streak)
        private void LoadMonthlyReport(List<DailyLog> allLogs, double dailyGoal)
        {
            var monthLogs = StatisticsCalculator.GetLogsForPeriod(allLogs, 30);

            // Wywołania metod statycznych z klasy StatisticsCalculator
            double avgConsumed = StatisticsCalculator.CalculateAverageConsumed(monthLogs);
            double totalBurned = StatisticsCalculator.CalculateTotalBurned(monthLogs);
            int daysOnGoal = StatisticsCalculator.GetDaysOnGoal(monthLogs, dailyGoal);
            int streak = StatisticsCalculator.GetCurrentStreak(allLogs, dailyGoal);

            // KOLEKCJA List<T> — Count to właściwość listy generycznej
            double goalRate = monthLogs.Count > 0
                ? (double)daysOnGoal / monthLogs.Count * 100.0
                : 0;

            TxtGoalPercent.Text = $"{goalRate:N0}%";
            TxtAvgDaily.Text = $"{avgConsumed:N0}";
            TxtTotalBurned.Text = $"{totalBurned:N0}";
            TxtDaysOnGoal.Text = $"{daysOnGoal} z {monthLogs.Count}";
            TxtStreak.Text = $"{streak} dni";
        }

        // Karta 4 — trend roczny (wykres liniowy CartesianChart)
        private void LoadYearlyTrend(List<DailyLog> allLogs, double dailyGoal)
        {
            var yearLogs = StatisticsCalculator.GetLogsForPeriod(allLogs, 365);

            // LINQ — łańcuch: GroupBy → OrderBy → ThenBy → Select → ToList
            // GroupBy grupuje dzienniki po roku i miesiącu (typ anonimowy)
            // Select tworzy kolekcję typów anonimowych (new { Year, Month, AvgConsumed })
            // Average — oblicza średnią kalorii w każdym miesiącu
            var monthlyData = yearLogs
                .GroupBy(l => new { l.Date.Year, l.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    AvgConsumed = g.Average(l => l.GetTotalCaloriesConsumed())
                })
                .ToList();

            // LINQ Select + ToArray — projekcja danych na tablice wartości
            var consumedValues = monthlyData.Select(m => m.AvgConsumed).ToArray();
            var goalValues = monthlyData.Select(_ => dailyGoal).ToArray();
            var labels = monthlyData.Select(m => $"{m.Month:00}/{m.Year % 100}").ToArray();

            // LINQ Any() — sprawdzenie czy kolekcja zawiera elementy
            if (!consumedValues.Any())
            {
                consumedValues = new double[] { 0 };
                goalValues = new double[] { dailyGoal };
                labels = new[] { DateTime.Today.ToString("MM/yy") };
            }

            // Wykres liniowy — LiveCharts2 LineSeries<double> (TYP GENERYCZNY)
            ChartTrend.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = consumedValues,
                    Name = "Spożyte",
                    Stroke = new SolidColorPaint(new SKColor(46, 204, 113), 2),
                    Fill = new SolidColorPaint(new SKColor(46, 204, 113, 40)),
                    GeometrySize = 0,
                    LineSmoothness = 0.3
                },
                new LineSeries<double>
                {
                    Values = goalValues,
                    Name = "Cel",
                    Stroke = new SolidColorPaint(new SKColor(200, 200, 200), 2),
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0
                }
            };

            ChartTrend.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels,
                    TextSize = 10,
                    LabelsPaint = new SolidColorPaint(new SKColor(150, 150, 150))
                }
            };

            ChartTrend.YAxes = new Axis[]
            {
                new Axis
                {
                    TextSize = 10,
                    LabelsPaint = new SolidColorPaint(new SKColor(150, 150, 150)),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(240, 240, 240), 1)
                }
            };
        }
    }
}
