using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    public partial class AddActivityEntryWindow : Window
    {
        public ActivityEntry ResultEntry { get; private set; } = null!;

        // Konstruktor — tryb dodawania
        public AddActivityEntryWindow(List<PhysicalActivity> availableActivities)
        {
            InitializeComponent();
            ComboActivities.ItemsSource = availableActivities;
        }

        // Konstruktor — tryb edycji (przeciążenie)
        public AddActivityEntryWindow(List<PhysicalActivity> availableActivities, ActivityEntry existing)
        {
            InitializeComponent();
            ComboActivities.ItemsSource = availableActivities;
            // LINQ FirstOrDefault() — wyszukanie aktywności po Id
            ComboActivities.SelectedItem = availableActivities.FirstOrDefault(a => a.Id == existing.Activity.Id);
            InputDuration.Text = existing.Duration.TotalMinutes.ToString();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var selected = ComboActivities.SelectedItem as PhysicalActivity;
            if (selected == null)
            {
                MessageBox.Show("Wybierz rodzaj aktywności!");
                return;
            }

            if (double.TryParse(InputDuration.Text, out double minutes) && minutes > 0)
            {
                ResultEntry = new ActivityEntry
                {
                    Activity = selected,
                    Duration = TimeSpan.FromMinutes(minutes),  // Konwersja minut na TimeSpan
                    StartTime = DateTime.Now
                };
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Podaj poprawny czas w minutach (większy od zera)!");
            }
        }
    }
}
