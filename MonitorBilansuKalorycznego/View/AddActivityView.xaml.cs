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
            if (string.IsNullOrWhiteSpace(InputName.Text))
            {
                MessageBox.Show("Podaj nazwę aktywności!");
                return;
            }

            if (!double.TryParse(InputKcal.Text, out double kcal) || kcal <= 0)
            {
                MessageBox.Show("Podaj poprawne spalanie kalorii (większe od zera)!");
                return;
            }

            ActivityData.Name = InputName.Text;
            ActivityData.CaloriesPerHour = kcal;

            if (!ActivityData.Validate())
            {
                MessageBox.Show("Dane aktywności są niepoprawne!");
                return;
            }

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}