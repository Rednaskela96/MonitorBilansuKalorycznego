using System.Windows;
using System.Windows.Controls;
using MonitorBilansuKalorycznego.Data;
using MonitorBilansuKalorycznego.View;

namespace MonitorBilansuKalorycznego
{
    // =====================================================================
    // MainWindow — główne okno aplikacji z nawigacją (sidebar).
    // DZIEDZICZENIE — dziedziczy po WPF Window (klasa bazowa frameworka).
    // Tworzy ApplicationData (centralny obiekt danych) i przekazuje go do widoków.
    // =====================================================================
    public partial class MainWindow : Window
    {
        // HERMETYZACJA — prywatne pole przechowujące dane aplikacji
        private ApplicationData _appData;

        public MainWindow()
        {
            InitializeComponent();
            _appData = new ApplicationData();  // Inicjalizacja warstwy danych

            // Domyślny widok na start — Dashboard
            OpenDashboard();
        }

        // ZDARZENIA WPF — handlery przypisane do przycisków w XAML (Click="...")
        private void Nav_Dashboard_Click(object sender, RoutedEventArgs e) => OpenDashboard();
        private void Nav_Database_Click(object sender, RoutedEventArgs e) => OpenDatabase();
        private void Nav_Journal_Click(object sender, RoutedEventArgs e)
        {
            ActiveView.Content = new DailyLogView(_appData);
        }

        // Nawigacja — zmiana zawartości ContentControl (ActiveView) na nowy UserControl
        private void OpenDashboard()
        {
            ActiveView.Content = new DashboardView(_appData);
        }

        private void OpenDatabase()
        {
            ActiveView.Content = new FoodDatabaseView(_appData);
        }

        private void Nav_Profile_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ActiveView.Content = new ProfileView(_appData);
        }
    }
}
