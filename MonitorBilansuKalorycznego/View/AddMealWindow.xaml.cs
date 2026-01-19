using System.Collections.Generic;
using System.Windows;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    public partial class AddMealWindow : Window
    {
        public MealEntry ResultEntry { get; private set; } = null!;

        public AddMealWindow(List<FoodProduct> availableProducts)
        {
            InitializeComponent();
            ComboProducts.ItemsSource = availableProducts; // Wypełniamy listę
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ComboProducts.SelectedItem as FoodProduct;
            if (selectedProduct == null)
            {
                MessageBox.Show("Wybierz produkt z listy!");
                return;
            }

            if (double.TryParse(InputAmount.Text, out double amount))
            {
                ResultEntry = new MealEntry
                {
                    Product = selectedProduct,
                    Amount = amount,
                    MealTime = System.DateTime.Now
                };
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Podaj poprawną ilość!");
            }
        }
    }
}