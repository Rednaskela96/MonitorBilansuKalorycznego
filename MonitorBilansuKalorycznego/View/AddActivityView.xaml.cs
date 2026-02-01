using System.Windows;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    // =====================================================================
    // AddActivityWindow — okno dialogowe do dodawania/edycji definicji aktywności.
    // Wzorzec okna dialogowego: ShowDialog() → DialogResult = true/false.
    // Przeciążone konstruktory: bezparametrowy (dodawanie) i z parametrem (edycja).
    // =====================================================================
    public partial class AddActivityWindow : Window
    {
        public PhysicalActivity ActivityData { get; private set; }

        // Konstruktor do DODAWANIA nowej aktywności
        public AddActivityWindow()
        {
            InitializeComponent();
            ActivityData = new PhysicalActivity();
        }

        // Konstruktor do EDYCJI istniejącej aktywności (przeciążenie konstruktora)
        public AddActivityWindow(PhysicalActivity existing)
        {
            InitializeComponent();
            ActivityData = existing;

            InputName.Text = existing.Name;
            InputKcal.Text = existing.CaloriesPerHour.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Walidacja danych wejściowych
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

            // Wywołanie metody Validate() z modelu PhysicalActivity
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
