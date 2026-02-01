using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    // =====================================================================
    // AddMealSetWindow — okno dialogowe tworzenia/edycji zestawu dań.
    // KOLEKCJE — List<MealSetItem> przechowuje elementy zestawu.
    // LINQ — Sum() do obliczania łącznych kalorii zestawu.
    // Wzorzec okna dialogowego z dynamiczną listą elementów.
    // =====================================================================
    public partial class AddMealSetWindow : Window
    {
        public MealSet ResultSet { get; private set; } = new MealSet();

        // KOLEKCJA — prywatna lista elementów zestawu (HERMETYZACJA)
        private readonly List<MealSetItem> _items = new List<MealSetItem>();

        // Konstruktor — tryb dodawania nowego zestawu
        public AddMealSetWindow(List<FoodProduct> availableProducts)
        {
            InitializeComponent();
            ComboProducts.ItemsSource = availableProducts;
        }

        // Konstruktor — tryb edycji istniejącego zestawu (przeciążenie)
        public AddMealSetWindow(List<FoodProduct> availableProducts, MealSet existing)
        {
            InitializeComponent();
            ComboProducts.ItemsSource = availableProducts;
            InputSetName.Text = existing.Name;
            _items.AddRange(existing.Items);  // Kopiowanie elementów do listy roboczej
            ResultSet = existing;
            RefreshList();
        }

        // Dodaje produkt do zestawu (tworzy kopię FoodProduct)
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

            // Tworzenie kopii produktu (nie referencji) do elementu zestawu
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

        // Usuwa element z zestawu (Tag przechowuje referencję do MealSetItem)
        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var item = ((Button)sender).Tag as MealSetItem;
            if (item != null)
            {
                _items.Remove(item);
                RefreshList();
            }
        }

        // Odświeża listę i sumę kalorii
        private void RefreshList()
        {
            ListItems.ItemsSource = null;
            ListItems.ItemsSource = _items;
            // LINQ Sum() — oblicza łączne kalorie wszystkich elementów zestawu
            TxtTotalCalories.Text = _items.Sum(i => i.CalculateCalories()).ToString("N0");
        }

        // Zapis zestawu — walidacja i zamknięcie okna
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
            ResultSet.Items = new List<MealSetItem>(_items);  // Kopia kolekcji
            DialogResult = true;
            Close();
        }
    }
}
