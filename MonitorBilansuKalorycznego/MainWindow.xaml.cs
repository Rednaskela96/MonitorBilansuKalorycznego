using System.Windows;
using System.Windows.Controls;
using MonitorBilansuKalorycznego.Data;
using MonitorBilansuKalorycznego.View;

namespace MonitorBilansuKalorycznego
{
    public partial class MainWindow : Window
    {
        private ApplicationData _appData;

        public MainWindow()
        {
            InitializeComponent();
            _appData = new ApplicationData(); // Start backendu

            // Domyślny widok na start
            OpenDashboard();
        }

        private void Nav_Dashboard_Click(object sender, RoutedEventArgs e) => OpenDashboard();
        private void Nav_Database_Click(object sender, RoutedEventArgs e) => OpenDatabase();
        private void Nav_Journal_Click(object sender, RoutedEventArgs e)
        {
            ActiveView.Content = new MonitorBilansuKalorycznego.View.DailyLogView(_appData);
        }

        private void OpenDashboard()
        {
            // Przekazujemy appData do widoku (Dependency Injection w wersji prostej)
            ActiveView.Content = new DashboardView(_appData);
        }

        private void OpenDatabase()
        {
            ActiveView.Content = new FoodDatabaseView(_appData);
        }
        private void Nav_Profile_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ActiveView.Content = new MonitorBilansuKalorycznego.View.ProfileView(_appData);
        }
    }
}