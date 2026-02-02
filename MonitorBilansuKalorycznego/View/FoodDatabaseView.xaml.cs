using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MonitorBilansuKalorycznego.Data;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    // Enum do przełączania zakładek w bazie danych
    public enum DatabaseTab { Products, Activities, MealSets }
    
    public partial class FoodDatabaseView : UserControl
    {
        private ApplicationData _appData;
        private DatabaseTab _currentTab = DatabaseTab.Products;

        public FoodDatabaseView(ApplicationData appData)
        {
            InitializeComponent();
            _appData = appData;
            RefreshList();
        }

        // ZDARZENIA WPF — handlery Checked przypisane w XAML do RadioButton
        private void Tab_Products_Checked(object sender, RoutedEventArgs e)
        {
            _currentTab = DatabaseTab.Products;
            UpdateViewMode();
            RefreshList();
        }

        private void Tab_Activities_Checked(object sender, RoutedEventArgs e)
        {
            _currentTab = DatabaseTab.Activities;
            UpdateViewMode();
            RefreshList();
        }

        private void Tab_MealSets_Checked(object sender, RoutedEventArgs e)
        {
            _currentTab = DatabaseTab.MealSets;
            UpdateViewMode();
            RefreshList();
        }

        // Przełącza widoczność kolumn i zmienia napisy w zależności od aktywnej zakładki
        private void UpdateViewMode()
        {
            if (ColKcalProd == null || ColProt == null || TxtHeaderTitle == null)
                return;

            // Ukryj wszystkie opcjonalne kolumny
            ColKcalProd.Visibility = Visibility.Collapsed;
            ColProt.Visibility = Visibility.Collapsed;
            ColCarbs.Visibility = Visibility.Collapsed;
            ColFat.Visibility = Visibility.Collapsed;
            ColKcalAct.Visibility = Visibility.Collapsed;
            ColSetItems.Visibility = Visibility.Collapsed;
            ColSetCalories.Visibility = Visibility.Collapsed;

            // SWITCH na enum — pokaż odpowiednie kolumny i teksty
            switch (_currentTab)
            {
                case DatabaseTab.Products:
                    TxtHeaderTitle.Text = "Twoje produkty";
                    TxtHeaderSubtitle.Text = "Zarządzaj nazwami i wartościami odżywczymi produktów";
                    if (BtnAdd != null) BtnAdd.Content = "+ Dodaj produkt";
                    ColKcalProd.Visibility = Visibility.Visible;
                    ColProt.Visibility = Visibility.Visible;
                    ColCarbs.Visibility = Visibility.Visible;
                    ColFat.Visibility = Visibility.Visible;
                    break;

                case DatabaseTab.Activities:
                    TxtHeaderTitle.Text = "Twoje aktywności";
                    TxtHeaderSubtitle.Text = "Zarządzaj listą ćwiczeń i ich spalaniem kalorii";
                    if (BtnAdd != null) BtnAdd.Content = "+ Dodaj aktywność";
                    ColKcalAct.Visibility = Visibility.Visible;
                    break;

                case DatabaseTab.MealSets:
                    TxtHeaderTitle.Text = "Twoje zestawy dań";
                    TxtHeaderSubtitle.Text = "Łącz produkty w gotowe zestawy posiłków";
                    if (BtnAdd != null) BtnAdd.Content = "+ Dodaj zestaw";
                    ColSetItems.Visibility = Visibility.Visible;
                    ColSetCalories.Visibility = Visibility.Visible;
                    break;
            }
        }

        // Filtrowanie listy wg wpisanej frazy (SearchBox) — LINQ + DELEGAT Func<T,bool>
        private void RefreshList()
        {
            if (SearchBox == null || _appData == null) return;
            string query = SearchBox.Text.ToLower();

            switch (_currentTab)
            {
                case DatabaseTab.Products:
                    // DELEGAT Func<T, bool> przekazywany do generycznej metody Find<T>()
                    ItemsGrid.ItemsSource = _appData.FoodManager.Find(p => p.Name.ToLower().Contains(query));
                    break;
                case DatabaseTab.Activities:
                    ItemsGrid.ItemsSource = _appData.ActivityManager.Find(a => a.Name.ToLower().Contains(query));
                    break;
                case DatabaseTab.MealSets:
                    ItemsGrid.ItemsSource = _appData.MealSetManager.Find(s => s.Name.ToLower().Contains(query));
                    break;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => RefreshList();

        // otwiera odpowiednie okno dialogowe w zależności od zakładki
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            switch (_currentTab)
            {
                case DatabaseTab.Products:
                {
                    var addWindow = new AddProductWindow();
                    if (addWindow.ShowDialog() == true)
                    {
                        _appData.FoodManager.Add(addWindow.NewProduct);  // Wywołuje ZDARZENIE DataChanged
                        RefreshList();
                    }
                    break;
                }
                case DatabaseTab.Activities:
                {
                    var actWindow = new AddActivityWindow();
                    if (actWindow.ShowDialog() == true)
                    {
                        _appData.ActivityManager.Add(actWindow.ActivityData);
                        RefreshList();
                    }
                    break;
                }
                case DatabaseTab.MealSets:
                {
                    var setWindow = new AddMealSetWindow(_appData.FoodManager.Items);
                    if (setWindow.ShowDialog() == true)
                    {
                        _appData.MealSetManager.Add(setWindow.ResultSet);
                        RefreshList();
                    }
                    break;
                }
            }
        }

        // otwiera okno w trybie edycji (przekazuje istniejący obiekt)
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            switch (_currentTab)
            {
                case DatabaseTab.Products:
                {
                    var product = ((Button)sender).DataContext as FoodProduct;
                    if (product == null) return;
                    var editWindow = new AddProductWindow(product);  // Przeciążony konstruktor (edycja)
                    if (editWindow.ShowDialog() == true)
                    {
                        _appData.FoodManager.SaveToFile();  // SERIALIZACJA
                        RefreshList();
                    }
                    break;
                }
                case DatabaseTab.Activities:
                {
                    var activity = ((Button)sender).DataContext as PhysicalActivity;
                    if (activity == null) return;
                    var editWindow = new AddActivityWindow(activity);
                    if (editWindow.ShowDialog() == true)
                    {
                        _appData.ActivityManager.SaveToFile();
                        RefreshList();
                    }
                    break;
                }
                case DatabaseTab.MealSets:
                {
                    var mealSet = ((Button)sender).DataContext as MealSet;
                    if (mealSet == null) return;
                    var editWindow = new AddMealSetWindow(_appData.FoodManager.Items, mealSet);
                    if (editWindow.ShowDialog() == true)
                    {
                        _appData.MealSetManager.SaveToFile();
                        RefreshList();
                    }
                    break;
                }
            }
        }

        // USUWANIE z potwierdzeniem (MessageBox YesNo)
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno usunąć?", "Potwierdzenie", MessageBoxButton.YesNo) == MessageBoxResult.No) return;

            switch (_currentTab)
            {
                case DatabaseTab.Products:
                {
                    var item = ((Button)sender).DataContext as FoodProduct;
                    if (item != null) _appData.FoodManager.Remove(item);
                    break;
                }
                case DatabaseTab.Activities:
                {
                    var item = ((Button)sender).DataContext as PhysicalActivity;
                    if (item != null) _appData.ActivityManager.Remove(item);
                    break;
                }
                case DatabaseTab.MealSets:
                {
                    var item = ((Button)sender).DataContext as MealSet;
                    if (item != null) _appData.MealSetManager.Remove(item);
                    break;
                }
            }
            RefreshList();
        }
    }
}
