using System;
using System.Windows;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    public partial class AddProductWindow : Window
    {
        public FoodProduct NewProduct { get; private set; } = null!;
        private readonly bool _isEditMode;

        public AddProductWindow()
        {
            InitializeComponent();
            _isEditMode = false;
        }

        public AddProductWindow(FoodProduct existing)
        {
            InitializeComponent();
            _isEditMode = true;
            NewProduct = existing;

            InputName.Text = existing.Name;
            InputKcal.Text = existing.CaloriesPer100g.ToString();
            InputProtein.Text = existing.Protein.ToString();
            InputCarbs.Text = existing.Carbs.ToString();
            InputFat.Text = existing.Fat.ToString();
            InputCategory.Text = existing.Category;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputName.Text))
            {
                MessageBox.Show("Podaj nazwę produktu!");
                return;
            }

            try
            {
                if (_isEditMode)
                {
                    NewProduct.Name = InputName.Text;
                    NewProduct.Category = InputCategory.Text;
                    NewProduct.CaloriesPer100g = double.Parse(InputKcal.Text);
                    NewProduct.Protein = double.Parse(InputProtein.Text);
                    NewProduct.Carbs = double.Parse(InputCarbs.Text);
                    NewProduct.Fat = double.Parse(InputFat.Text);
                }
                else
                {
                    NewProduct = new FoodProduct
                    {
                        Name = InputName.Text,
                        Category = InputCategory.Text,
                        CaloriesPer100g = double.Parse(InputKcal.Text),
                        Protein = double.Parse(InputProtein.Text),
                        Carbs = double.Parse(InputCarbs.Text),
                        Fat = double.Parse(InputFat.Text)
                    };
                }

                if (!NewProduct.Validate())
                {
                    MessageBox.Show("Dane produktu są niepoprawne! Kalorie nie mogą być ujemne.");
                    return;
                }

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