using System;
using System.Windows;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    public partial class AddProductWindow : Window
    {
        // Właściwość, którą pobierze główne okno po zamknięciu tego
        public FoodProduct NewProduct { get; private set; } = null!;

        public AddProductWindow()
        {
            InitializeComponent();
        }
        // NOWE: Konstruktor do EDYCJI
        public AddProductWindow(FoodProduct existing)
        {
            InitializeComponent();
            // Przepisujemy dane do pól, żeby użytkownik mógł je zmienić
            InputName.Text = existing.Name;
            InputKcal.Text = existing.CaloriesPer100g.ToString();
            InputProtein.Text = existing.Protein.ToString();
            InputCarbs.Text = existing.Carbs.ToString();
            InputFat.Text = existing.Fat.ToString();
            InputCategory.Text = existing.Category;

            // Pracujemy na TYM SAMYM obiekcie (referencja) lub można tworzyć kopię
            NewProduct = existing;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Walidacja - czy wpisano nazwę?
            if (string.IsNullOrWhiteSpace(InputName.Text))
            {
                MessageBox.Show("Podaj nazwę produktu!");
                return;
            }

            try
            {
                // Tworzymy obiekt
                NewProduct = new FoodProduct
                {
                    Name = InputName.Text,
                    Category = InputCategory.Text,
                    CaloriesPer100g = double.Parse(InputKcal.Text),
                    Protein = double.Parse(InputProtein.Text),
                    Carbs = double.Parse(InputCarbs.Text),
                    Fat = double.Parse(InputFat.Text)
                };

                // Zamykamy okno z sukcesem
                DialogResult = true;
                Close();
            }
            catch
            {
                MessageBox.Show("Wpisz poprawne liczby w polach wartości!");
            }
        }
    }
}