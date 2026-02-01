using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MonitorBilansuKalorycznego.Data;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    public partial class DailyLogView : UserControl
    {
        private ApplicationData _appData;
        private DateTime _currentDate;
        private DailyLog _currentLog = null!;

        public DailyLogView(ApplicationData appData)
        {
            InitializeComponent();
            _appData = appData;
            _currentDate = DateTime.Today;

            LoadLogForDate(_currentDate);
        }

        private void LoadLogForDate(DateTime date)
        {
            TxtDate.Text = date.ToString("dddd, dd MMMM, yyyy");

            _currentLog = _appData.LogManager.Items.FirstOrDefault(l => l.Date.Date == date.Date);

            if (_currentLog == null)
            {
                _currentLog = new DailyLog { Date = date };
            }

            RefreshStats();
        }

        private void EnsureLogPersisted()
        {
            if (!_appData.LogManager.Items.Contains(_currentLog))
            {
                _appData.LogManager.Add(_currentLog);
            }
        }

        private void RefreshStats()
        {
            // 1. Listy
            ListMeals.ItemsSource = null;
            ListMeals.ItemsSource = _currentLog.Meals;

            ListActivities.ItemsSource = null;
            ListActivities.ItemsSource = _currentLog.Activities;

            // Pokaż/Ukryj placeholder dla aktywności
            if (_currentLog.Activities.Count == 0) EmptyActivitiesState.Visibility = Visibility.Visible;
            else EmptyActivitiesState.Visibility = Visibility.Collapsed;

            // 2. Kółeczka
            double consumed = _currentLog.GetTotalCaloriesConsumed();
            double burned = _currentLog.GetTotalCaloriesBurned();
            double goal = _appData.CurrentUser.GetDailyCalorieGoal();
            double remaining = goal - (consumed - burned);

            TxtConsumed.Text = Math.Round(consumed).ToString();
            TxtBurned.Text = Math.Round(burned).ToString();
            TxtGoal.Text = Math.Round(goal).ToString();
            TxtRemaining.Text = Math.Round(remaining).ToString();

            // 3. Pasek Postępu
            double percent = (goal > 0) ? ((consumed - burned) / goal) * 100 : 0;
            if (percent < 0) percent = 0;
            if (percent > 100) percent = 100;

            PbDayProgress.Value = percent;
            TxtProgressPercent.Text = $"{Math.Round(percent)}% celu";
        }

        // --- NAWIGACJA ---
        private void BtnPrevDay_Click(object sender, RoutedEventArgs e)
        {
            _currentDate = _currentDate.AddDays(-1);
            LoadLogForDate(_currentDate);
        }

        private void BtnNextDay_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDate.Date >= DateTime.Today) return;
            _currentDate = _currentDate.AddDays(1);
            LoadLogForDate(_currentDate);
        }

        // --- AKCJE ---
        private void BtnAddMeal_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddMealWindow(_appData.FoodManager.Items, _appData.MealSetManager.Items);
            if (window.ShowDialog() == true)
            {
                EnsureLogPersisted();
                foreach (var entry in window.ResultEntries)
                {
                    _currentLog.AddMeal(entry);
                }
                _appData.LogManager.SaveToFile();
                RefreshStats();
            }
        }

        private void BtnAddActivity_Click(object sender, RoutedEventArgs e)
        {
            // Otwieramy nowe okno wyboru
            var window = new AddActivityEntryWindow(_appData.ActivityManager.Items);

            if (window.ShowDialog() == true)
            {
                EnsureLogPersisted();
                _currentLog.AddActivity(window.ResultEntry);
                _appData.LogManager.SaveToFile();
                RefreshStats();
            }
        }

        private void BtnEditMeal_Click(object sender, RoutedEventArgs e)
        {
            var entry = ((Button)sender).Tag as MealEntry;
            if (entry == null) return;

            var window = new AddMealWindow(_appData.FoodManager.Items, entry);
            if (window.ShowDialog() == true)
            {
                entry.Product = window.ResultEntry.Product;
                entry.Amount = window.ResultEntry.Amount;
                _appData.LogManager.SaveToFile();
                RefreshStats();
            }
        }

        private void BtnEditActivity_Click(object sender, RoutedEventArgs e)
        {
            var entry = ((Button)sender).Tag as ActivityEntry;
            if (entry == null) return;

            var window = new AddActivityEntryWindow(_appData.ActivityManager.Items, entry);
            if (window.ShowDialog() == true)
            {
                entry.Activity = window.ResultEntry.Activity;
                entry.Duration = window.ResultEntry.Duration;
                _appData.LogManager.SaveToFile();
                RefreshStats();
            }
        }

        private void BtnDeleteMeal_Click(object sender, RoutedEventArgs e)
        {
            var entry = ((Button)sender).Tag as MealEntry;
            if (entry != null)
            {
                _currentLog.RemoveMeal(entry);
                _appData.LogManager.SaveToFile();
                RefreshStats();
            }
        }

        private void BtnDeleteActivity_Click(object sender, RoutedEventArgs e)
        {
            var entry = ((Button)sender).Tag as ActivityEntry;
            if (entry != null)
            {
                _currentLog.RemoveActivity(entry);
                _appData.LogManager.SaveToFile();
                RefreshStats();
            }
        }
    }
}