using System;
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

            // Wypełnij ComboBoxy enumami
            ComboGender.ItemsSource = Enum.GetValues(typeof(Gender));
            ComboActivity.ItemsSource = Enum.GetValues(typeof(ActivityLevel));

            LoadData();
        }

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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = _appData.CurrentUser;
                user.Name = InputName.Text;
                user.Age = int.Parse(InputAge.Text);
                user.Weight = double.Parse(InputWeight.Text.Replace(".", ","));
                user.Height = double.Parse(InputHeight.Text.Replace(".", ","));

                if (ComboGender.SelectedItem != null)
                    user.Gender = (Gender)ComboGender.SelectedItem;

                if (ComboActivity.SelectedItem != null)
                    user.ActivityLevel = (ActivityLevel)ComboActivity.SelectedItem;

                // Reset ręcznego celu przy przeliczaniu automatycznym
                user.CustomDailyCalorieGoal = 0;
                InputCustomGoal.Text = "0";

                _appData.SaveAll();
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