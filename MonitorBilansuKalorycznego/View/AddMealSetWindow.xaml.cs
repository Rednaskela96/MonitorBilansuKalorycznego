using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    public partial class AddMealSetWindow : Window
    {
        public MealSet ResultSet { get; private set; } = new MealSet();
        private readonly List<MealSetItem> _items = new List<MealSetItem>();

        public AddMealSetWindow(List<FoodProduct> availableProducts)
        {
            InitializeComponent();
            ComboProducts.ItemsSource = availableProducts;
        }

        public AddMealSetWindow(List<FoodProduct> availableProducts, MealSet existing)
        {
            InitializeComponent();
            ComboProducts.ItemsSource = availableProducts;
            InputSetName.Text = existing.Name;
            _items.AddRange(existing.Items);
            ResultSet = existing;
            RefreshList();
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            var product = ComboProducts.SelectedItem as FoodProduct;
            if (product == null)
            {
                MessageBox.Show("Wybierz produkt z listy!");
                return;
            }

            if (!double.TryParse(InputItemAmount.Text, out double amount) || amount <= 0)
            {
                MessageBox.Show("Podaj poprawna ilosc (wieksza od zera)!");
                return;
            }

            _items.Add(new MealSetItem
            {
                Product = new FoodProduct
                {
                    Id = product.Id,
                    Name = product.Name,
                    CaloriesPer100g = product.CaloriesPer100g,
                    Protein = product.Protein,
                    Carbs = product.Carbs,
                    Fat = product.Fat,
                    Category = product.Category
                },
                Amount = amount
            });

            RefreshList();
        }

        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var item = ((Button)sender).Tag as MealSetItem;
            if (item != null)
            {
                _items.Remove(item);
                RefreshList();
            }
        }

        private void RefreshList()
        {
            ListItems.ItemsSource = null;
            ListItems.ItemsSource = _items;
            TxtTotalCalories.Text = _items.Sum(i => i.CalculateCalories()).ToString("N0");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputSetName.Text))
            {
                MessageBox.Show("Podaj nazwe zestawu!");
                return;
            }

            if (_items.Count == 0)
            {
                MessageBox.Show("Dodaj przynajmniej jeden produkt do zestawu!");
                return;
            }

            ResultSet.Name = InputSetName.Text;
            ResultSet.Items = new List<MealSetItem>(_items);
            DialogResult = true;
            Close();
        }
    }
}
