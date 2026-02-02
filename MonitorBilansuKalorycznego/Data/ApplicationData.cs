using System;
using System.IO;
using Newtonsoft.Json;
using MonitorBilansuKalorycznego.Model;
using MonitorBilansuKalorycznego.Logic;

namespace MonitorBilansuKalorycznego.Data
{
    /* ApplicationData — centralny punkt dostępu do danych aplikacji.
    Zarządza profilem użytkownika i czterema DataManager<T> (TYPY GENERYCZNE).
    Użyta jest tu serializacja do profilu użytkownika zapisywany/odczytywany z JSONa oraz
    hermetyzacja do prywatnych pól (_profilePath), publiczne właściwości z private set. */
    public class ApplicationData
    {
        private const string ProfileFileName = "user_profile.json";
        
        private string _profilePath;

        public UserProfile CurrentUser { get; set; } = new UserProfile();

        // Cztery instancje KLASY GENERYCZNEJ DataManager<T> — każda dla innego typu modelu
        public DataManager<FoodProduct> FoodManager { get; private set; }
        public DataManager<PhysicalActivity> ActivityManager { get; private set; }
        public DataManager<DailyLog> LogManager { get; private set; }
        public DataManager<MealSet> MealSetManager { get; private set; }

        public ApplicationData()
        {
            _profilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProfileFileName);

            // Inicjalizacja managerów — każdy zapisuje dane do osobnego pliku JSON
            FoodManager = new DataManager<FoodProduct>("foods.json");
            ActivityManager = new DataManager<PhysicalActivity>("activities.json");
            LogManager = new DataManager<DailyLog>("logs.json");
            MealSetManager = new DataManager<MealSet>("meal_sets.json");

            LoadAll();
        }

        // zapis wszystkich danych do plików JSON
        public void SaveAll()
        {
            try
            {
                // SERIALIZACJA profilu użytkownika do JSON
                string json = JsonConvert.SerializeObject(CurrentUser, Formatting.Indented);
                File.WriteAllText(_profilePath, json);
            }
            catch (Exception ex)  // wyjatki
            {
                System.Windows.MessageBox.Show($"Błąd zapisu profilu: {ex.Message}",
                    "Błąd zapisu", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            FoodManager.SaveToFile();
            ActivityManager.SaveToFile();
            LogManager.SaveToFile();
            MealSetManager.SaveToFile();
        }

        // DESERIALIZACJA — odczyt wszystkich danych z plików JSON
        public void LoadAll()
        {
            try
            {
                if (File.Exists(_profilePath))
                {
                    string json = File.ReadAllText(_profilePath);
                    CurrentUser = JsonConvert.DeserializeObject<UserProfile>(json) ?? new UserProfile();
                }
                else
                {
                    CurrentUser = new UserProfile();
                }
            }
            catch (Exception)
            {
                CurrentUser = new UserProfile();
            }

            FoodManager.LoadFromFile();
            ActivityManager.LoadFromFile();
            LogManager.LoadFromFile();
            MealSetManager.LoadFromFile();
        }

        // Generowanie raportu tygodniowego
        public ReportData GetCurrentWeeklyReport()
        {
            var reportGenerator = new WeeklyReport(LogManager.Items, CurrentUser.GetDailyCalorieGoal());
            return reportGenerator.GenerateReport();
        }

        // Resetowanie całej bazy danych do stanu początkowego
        public void ResetAll()
        {
            // Reset profilu użytkownika
            CurrentUser = new UserProfile();

            // Wyczyszczenie wszystkich kolekcji
            FoodManager.Items.Clear();
            ActivityManager.Items.Clear();
            LogManager.Items.Clear();
            MealSetManager.Items.Clear();

            // Zapis pustych danych do plików
            SaveAll();
        }
    }
}
