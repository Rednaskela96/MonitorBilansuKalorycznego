using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MonitorBilansuKalorycznego.Data;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    // =====================================================================
    // DailyLogView — widok dziennika kalorycznego (główny ekran codzienny).
    // DZIEDZICZENIE — dziedziczy po UserControl (WPF).
    // LINQ — FirstOrDefault() do wyszukiwania logów po dacie.
    // KOLEKCJE — operacje na List<MealEntry> i List<ActivityEntry>.
    // Wzorzec: okna dialogowe (ShowDialog) do dodawania/edycji wpisów.
    // =====================================================================
    public partial class DailyLogView : UserControl
    {
        // HERMETYZACJA — prywatne pola (niedostępne z zewnątrz)
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

        // Ładuje (lub tworzy) log dla danej daty
        private void LoadLogForDate(DateTime date)
        {
            TxtDate.Text = date.ToString("dddd, dd MMMM, yyyy");

            // LINQ FirstOrDefault() — wyszukuje log po dacie w kolekcji
            _currentLog = _appData.LogManager.Items.FirstOrDefault(l => l.Date.Date == date.Date);

            if (_currentLog == null)
            {
                _currentLog = new DailyLog { Date = date };
                // Nie dodajemy do kolekcji od razu — dopiero przy pierwszym wpisie (EnsureLogPersisted)
            }

            RefreshStats();
        }

        // Dodaje log do kolekcji tylko gdy użytkownik faktycznie coś wpisze (lazy persistence)
        private void EnsureLogPersisted()
        {
            if (!_appData.LogManager.Items.Contains(_currentLog))
            {
                _appData.LogManager.Add(_currentLog);
            }
        }

        // Odświeża UI (listy posiłków/aktywności, statystyki, pasek postępu)
        private void RefreshStats()
        {
            // Odświeżenie ItemsSource (Data Binding w code-behind)
            ListMeals.ItemsSource = null;
            ListMeals.ItemsSource = _currentLog.Meals;

            ListActivities.ItemsSource = null;
            ListActivities.ItemsSource = _currentLog.Activities;

            if (_currentLog.Activities.Count == 0) EmptyActivitiesState.Visibility = Visibility.Visible;
            else EmptyActivitiesState.Visibility = Visibility.Collapsed;

            // Obliczenia kaloryczne
            double consumed = _currentLog.GetTotalCaloriesConsumed();
            double burned = _currentLog.GetTotalCaloriesBurned();
            double goal = _appData.CurrentUser.GetDailyCalorieGoal();
            double remaining = goal - (consumed - burned);

            TxtConsumed.Text = Math.Round(consumed).ToString();
            TxtBurned.Text = Math.Round(burned).ToString();
            TxtGoal.Text = Math.Round(goal).ToString();
            TxtRemaining.Text = Math.Round(remaining).ToString();

            // Pasek postępu
            double percent = (goal > 0) ? ((consumed - burned) / goal) * 100 : 0;
            if (percent < 0) percent = 0;
            if (percent > 100) percent = 100;

            PbDayProgress.Value = percent;
            TxtProgressPercent.Text = $"{Math.Round(percent)}% celu";
        }

        // Nawigacja po datach (strzałki lewo/prawo)
        private void BtnPrevDay_Click(object sender, RoutedEventArgs e)
        {
            _currentDate = _currentDate.AddDays(-1);
            LoadLogForDate(_currentDate);
        }

        private void BtnNextDay_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDate.Date >= DateTime.Today) return;  // Blokada przyszłych dat
            _currentDate = _currentDate.AddDays(1);
            LoadLogForDate(_currentDate);
        }

        // Dodawanie posiłku — otwiera AddMealWindow z zakładkami (Produkty / Zestawy)
        private void BtnAddMeal_Click(object sender, RoutedEventArgs e)
        {
            // Przekazujemy listę produktów i zestawów dań do okna dialogowego
            var window = new AddMealWindow(_appData.FoodManager.Items, _appData.MealSetManager.Items);
            if (window.ShowDialog() == true)
            {
                EnsureLogPersisted();
                // Iteracja po ResultEntries — zestaw dań dodaje wiele wpisów naraz
                foreach (var entry in window.ResultEntries)
                {
                    _currentLog.AddMeal(entry);
                }
                _appData.LogManager.SaveToFile();  // SERIALIZACJA
                RefreshStats();
            }
        }

        // Dodawanie aktywności — otwiera AddActivityEntryWindow
        private void BtnAddActivity_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddActivityEntryWindow(_appData.ActivityManager.Items);
            if (window.ShowDialog() == true)
            {
                EnsureLogPersisted();
                _currentLog.AddActivity(window.ResultEntry);
                _appData.LogManager.SaveToFile();
                RefreshStats();
            }
        }

        // Edycja posiłku — otwiera AddMealWindow w trybie edycji
        private void BtnEditMeal_Click(object sender, RoutedEventArgs e)
        {
            var entry = ((Button)sender).Tag as MealEntry;  // Tag przechowuje referencję do obiektu
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

        // Edycja aktywności
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

        // Usuwanie posiłku z kolekcji
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

        // Usuwanie aktywności z kolekcji
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
