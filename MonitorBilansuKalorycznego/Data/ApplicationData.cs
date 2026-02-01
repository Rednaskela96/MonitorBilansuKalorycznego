using System;
using System.IO;
using Newtonsoft.Json;
using MonitorBilansuKalorycznego.Model;
using MonitorBilansuKalorycznego.Logic;

namespace MonitorBilansuKalorycznego.Data
{
    public class ApplicationData
    {
        private const string ProfileFileName = "user_profile.json";
        private string _profilePath;

        public UserProfile CurrentUser { get; set; } = new UserProfile();

        public DataManager<FoodProduct> FoodManager { get; private set; }
        public DataManager<PhysicalActivity> ActivityManager { get; private set; }
        public DataManager<DailyLog> LogManager { get; private set; }
        public DataManager<MealSet> MealSetManager { get; private set; }

        // --- KONSTRUKTOR ---
        public ApplicationData()
        {
            _profilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProfileFileName);

            FoodManager = new DataManager<FoodProduct>("foods.json");
            ActivityManager = new DataManager<PhysicalActivity>("activities.json");
            LogManager = new DataManager<DailyLog>("logs.json");
            MealSetManager = new DataManager<MealSet>("meal_sets.json");

            LoadAll();
        }
        // --- KONIEC KONSTRUKTORA ---

        public void SaveAll()
        {
            try
            {
                string json = JsonConvert.SerializeObject(CurrentUser, Formatting.Indented);
                File.WriteAllText(_profilePath, json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Błąd zapisu profilu: {ex.Message}",
                    "Błąd zapisu", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            FoodManager.SaveToFile();
            ActivityManager.SaveToFile();
            LogManager.SaveToFile();
            MealSetManager.SaveToFile();
        }

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

        public ReportData GetCurrentWeeklyReport()
        {
            // (używamy CurrentUser zamiast nazwy klasy)
            var reportGenerator = new WeeklyReport(LogManager.Items, CurrentUser.GetDailyCalorieGoal());
            return reportGenerator.GenerateReport();
        }
    }
}