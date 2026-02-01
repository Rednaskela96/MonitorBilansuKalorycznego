using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    // =====================================================================
    // AddMealWindow — okno dialogowe dodawania posiłku do dziennika.
    // Dwie zakładki: "Pojedynczy produkt" i "Zestaw dań".
    // KOLEKCJE — List<MealEntry> (ResultEntries) zwraca wiele wpisów przy zestawie.
    // LINQ — FirstOrDefault() do wyszukiwania produktu po Id.
    // Wzorzec okna dialogowego: ShowDialog() → DialogResult = true/false.
    // =====================================================================
    public partial class AddMealWindow : Window
    {
        public MealEntry ResultEntry { get; private set; } = null!;

        // KOLEKCJA — lista wynikowych wpisów (1 element dla produktu, N dla zestawu)
        public List<MealEntry> ResultEntries { get; private set; } = new List<MealEntry>();

        private readonly List<MealSet> _mealSets;
        private bool _isEditMode;

        // Konstruktor — tryb dodawania (tylko produkty, bez zestawów)
        public AddMealWindow(List<FoodProduct> availableProducts)
            : this(availableProducts, new List<MealSet>())
        {
        }

        // Konstruktor — tryb dodawania z zestawami dań
        public AddMealWindow(List<FoodProduct> availableProducts, List<MealSet> mealSets)
        {
            InitializeComponent();
            _mealSets = mealSets;
            ComboProducts.ItemsSource = availableProducts;
            ComboSets.ItemsSource = mealSets;

            // Ukryj zakładkę zestawów jeśli brak zestawów w bazie
            if (mealSets.Count == 0 && TabSet != null)
                TabSet.Visibility = Visibility.Collapsed;
        }

        // Konstruktor — tryb edycji istniejącego wpisu (ukrywa zakładki)
        public AddMealWindow(List<FoodProduct> availableProducts, MealEntry existing)
            : this(availableProducts, new List<MealSet>())
        {
            _isEditMode = true;
            // LINQ FirstOrDefault() — znajdź produkt po Id
            ComboProducts.SelectedItem = availableProducts.FirstOrDefault(p => p.Id == existing.Product.Id);
            InputAmount.Text = existing.Amount.ToString();

            TabProduct.Visibility = Visibility.Collapsed;
            TabSet.Visibility = Visibility.Collapsed;
            BtnConfirm.Content = "Zapisz zmiany";
        }

        // Przełączanie zakładek (RadioButton Checked events)
        private void TabProduct_Checked(object sender, RoutedEventArgs e)
        {
            if (PanelProduct == null) return;
            PanelProduct.Visibility = Visibility.Visible;
            PanelSet.Visibility = Visibility.Collapsed;
        }

        private void TabSet_Checked(object sender, RoutedEventArgs e)
        {
            if (PanelSet == null) return;
            PanelProduct.Visibility = Visibility.Collapsed;
            PanelSet.Visibility = Visibility.Visible;
        }

        // Podgląd zawartości wybranego zestawu
        private void ComboSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = ComboSets.SelectedItem as MealSet;
            if (selected == null)
            {
                ListSetPreview.ItemsSource = null;
                TxtSetTotal.Text = "";
                return;
            }

            ListSetPreview.ItemsSource = selected.Items;
            TxtSetTotal.Text = $"Razem: {selected.TotalCalories:N0} kcal";
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            bool isSetTab = TabSet.IsChecked == true && !_isEditMode;

            if (isSetTab)
                AddFromSet();
            else
                AddSingleProduct();
        }

        // Dodaje pojedynczy produkt — tworzy jeden MealEntry
        private void AddSingleProduct()
        {
            var selectedProduct = ComboProducts.SelectedItem as FoodProduct;
            if (selectedProduct == null)
            {
                MessageBox.Show("Wybierz produkt z listy!");
                return;
            }

            if (!double.TryParse(InputAmount.Text, out double amount) || amount <= 0)
            {
                MessageBox.Show("Podaj poprawną ilość (większą od zera)!");
                return;
            }

            var entry = new MealEntry
            {
                // Kopia produktu (nie referencja) — zabezpieczenie przed zmianą oryginału
                Product = new FoodProduct
                {
                    Id = selectedProduct.Id,
                    Name = selectedProduct.Name,
                    CaloriesPer100g = selectedProduct.CaloriesPer100g,
                    Protein = selectedProduct.Protein,
                    Carbs = selectedProduct.Carbs,
                    Fat = selectedProduct.Fat,
                    Category = selectedProduct.Category
                },
                Amount = amount,
                MealTime = DateTime.Now
            };

            ResultEntry = entry;
            ResultEntries = new List<MealEntry> { entry };
            DialogResult = true;
            Close();
        }

        // Dodaje zestaw dań — tworzy wiele MealEntry (jeden na każdy element zestawu)
        private void AddFromSet()
        {
            var selectedSet = ComboSets.SelectedItem as MealSet;
            if (selectedSet == null)
            {
                MessageBox.Show("Wybierz zestaw z listy!");
                return;
            }

            if (selectedSet.Items.Count == 0)
            {
                MessageBox.Show("Wybrany zestaw jest pusty!");
                return;
            }

            // Iteracja po elementach zestawu — tworzenie kopii produktów
            var entries = new List<MealEntry>();
            foreach (var item in selectedSet.Items)
            {
                entries.Add(new MealEntry
                {
                    Product = new FoodProduct
                    {
                        Id = item.Product.Id,
                        Name = item.Product.Name,
                        CaloriesPer100g = item.Product.CaloriesPer100g,
                        Protein = item.Product.Protein,
                        Carbs = item.Product.Carbs,
                        Fat = item.Product.Fat,
                        Category = item.Product.Category
                    },
                    Amount = item.Amount,
                    MealTime = DateTime.Now
                });
            }

            ResultEntry = entries.First();  // LINQ First()
            ResultEntries = entries;
            DialogResult = true;
            Close();
        }
    }
}
