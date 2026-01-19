using System;
using System.Collections.Generic;
using System.Windows;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    public partial class AddActivityEntryWindow : Window
    {
        public ActivityEntry ResultEntry { get; private set; } = null!;

        public AddActivityEntryWindow(List<PhysicalActivity> availableActivities)
        {
            InitializeComponent();
            ComboActivities.ItemsSource = availableActivities;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var selected = ComboActivities.SelectedItem as PhysicalActivity;
            if (selected == null)
            {
                MessageBox.Show("Wybierz rodzaj aktywności!");
                return;
            }

            if (double.TryParse(InputDuration.Text, out double minutes))
            {
                ResultEntry = new ActivityEntry
                {
                    Activity = selected,
                    Duration = TimeSpan.FromMinutes(minutes),
                    StartTime = DateTime.Now
                };
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Podaj poprawny czas w minutach!");
            }
        }
    }
}