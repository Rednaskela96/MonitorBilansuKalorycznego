using System;
using System.Windows;
using MonitorBilansuKalorycznego.Exceptions;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    public partial class AddProductWindow : Window
    {
        public FoodProduct NewProduct { get; private set; } = null!;
        
        private readonly bool _isEditMode;

        // Konstruktor do DODAWANIA nowego produktu
        public AddProductWindow()
        {
            InitializeComponent();
            _isEditMode = false;
        }

        // Konstruktor do EDYCJI istniejącego produktu (przeciążenie konstruktora)
        public AddProductWindow(FoodProduct existing)
        {
            InitializeComponent();
            _isEditMode = true;
            NewProduct = existing;

            // Wypełnienie pól danymi istniejącego produktu
            InputName.Text = existing.Name;
            InputKcal.Text = existing.CaloriesPer100g.ToString();
            InputProtein.Text = existing.Protein.ToString();
            InputCarbs.Text = existing.Carbs.ToString();
            InputFat.Text = existing.Fat.ToString();
            InputCategory.Text = existing.Category;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(InputName.Text))
                    throw new ValidationException("Name", "Podaj nazwę produktu!");

                double kcal = ParseField(InputKcal.Text, "Kalorie");
                double protein = ParseField(InputProtein.Text, "Białko");
                double carbs = ParseField(InputCarbs.Text, "Węglowodany");
                double fat = ParseField(InputFat.Text, "Tłuszcze");

                if (kcal < 0)
                    throw new ValidationException("CaloriesPer100g", "Kalorie nie mogą być ujemne!");

                if (_isEditMode)
                {
                    // Tryb edycji — modyfikacja istniejącego obiektu (ta sama referencja)
                    NewProduct.Name = InputName.Text;
                    NewProduct.Category = InputCategory.Text;
                    NewProduct.CaloriesPer100g = kcal;
                    NewProduct.Protein = protein;
                    NewProduct.Carbs = carbs;
                    NewProduct.Fat = fat;
                }
                else
                {
                    // Tryb dodawania — tworzenie nowego obiektu
                    NewProduct = new FoodProduct
                    {
                        Name = InputName.Text,
                        Category = InputCategory.Text,
                        CaloriesPer100g = kcal,
                        Protein = protein,
                        Carbs = carbs,
                        Fat = fat
                    };
                }

                DialogResult = true;
                Close();
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show("Wpisz poprawne liczby w polach wartości!");
            }
        }

        // Metoda pomocnicza — parsuje tekst na double, rzuca ValidationException przy błędzie
        private double ParseField(string text, string fieldName)
        {
            if (!double.TryParse(text, out double value))
                throw new ValidationException(fieldName, $"Pole \"{fieldName}\" musi zawierać poprawną liczbę!");
            return value;
        }
    }
}
