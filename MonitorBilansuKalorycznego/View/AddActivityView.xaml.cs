using System.Windows;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    public partial class AddActivityWindow : Window
    {
        public PhysicalActivity ActivityData { get; private set; }

        // Konstruktor do dodawania
        public AddActivityWindow()
        {
            InitializeComponent();
            ActivityData = new PhysicalActivity();
        }

        // Konstruktor do EDYCJI (przyjmuje istniejący obiekt)
        public AddActivityWindow(PhysicalActivity existing)
        {
            InitializeComponent();
            ActivityData = existing;

            // Wypełniamy pola
            InputName.Text = existing.Name;
            InputKcal.Text = existing.CaloriesPerHour.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputName.Text)) return;

            // Aktualizujemy obiekt
            ActivityData.Name = InputName.Text;
            if (double.TryParse(InputKcal.Text, out double kcal))
                ActivityData.CaloriesPerHour = kcal;

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}