using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MonitorBilansuKalorycznego.Data;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    public partial class ProfileView : UserControl
    {
        private ApplicationData _appData;

        public ProfileView(ApplicationData appData)
        {
            InitializeComponent();
            _appData = appData;

            //typeof() pobiera typ, Enum.GetValues() zwraca wszystkie wartości enuma
            ComboGender.ItemsSource = Enum.GetValues(typeof(Gender));
            ComboActivity.ItemsSource = Enum.GetValues(typeof(ActivityLevel));

            LoadData();
        }

        // Ładuje dane z modelu UserProfile do kontrolek UI
        private void LoadData()
        {
            var user = _appData.CurrentUser;
            InputName.Text = user.Name;
            InputAge.Text = user.Age.ToString();
            InputWeight.Text = user.Weight.ToString();
            InputHeight.Text = user.Height.ToString();
            ComboGender.SelectedItem = user.Gender;
            ComboActivity.SelectedItem = user.ActivityLevel;
            InputCustomGoal.Text = user.CustomDailyCalorieGoal.ToString();

            UpdateResultText();
        }

        // Handler przycisku "Zapisz" — walidacja + zapis danych
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = _appData.CurrentUser;
                user.Name = InputName.Text;
                user.Age = int.Parse(InputAge.Text);

                // CultureInfo.InvariantCulture — parsowanie niezależne od lokalizacji systemu
                user.Weight = double.Parse(InputWeight.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                user.Height = double.Parse(InputHeight.Text.Replace(",", "."), CultureInfo.InvariantCulture);

                if (user.Age <= 0 || user.Weight <= 0 || user.Height <= 0)
                {
                    MessageBox.Show("Wiek, waga i wzrost muszą być większe od zera!");
                    return;
                }

                // Rzutowanie SelectedItem na typ enum
                if (ComboGender.SelectedItem != null)
                    user.Gender = (Gender)ComboGender.SelectedItem;

                if (ComboActivity.SelectedItem != null)
                    user.ActivityLevel = (ActivityLevel)ComboActivity.SelectedItem;

                user.CustomDailyCalorieGoal = 0;
                InputCustomGoal.Text = "0";

                _appData.SaveAll();  // zapis do pliku JSON
                UpdateResultText();
                MessageBox.Show("Profil zaktualizowany!");
            }
            catch 
            {
                MessageBox.Show("Sprawdź poprawność danych (tylko liczby)!");
            }
        }

        private void BtnCustomGoal_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(InputCustomGoal.Text, out double custom))
            {
                _appData.CurrentUser.CustomDailyCalorieGoal = custom;
                _appData.SaveAll();
                UpdateResultText();
            }
        }

        private void UpdateResultText()
        {
            TxtResultGoal.Text = $"{_appData.CurrentUser.GetDailyCalorieGoal()} kcal";
        }
    }
}
