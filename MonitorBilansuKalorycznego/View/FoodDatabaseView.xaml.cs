using System.Windows;
using System.Windows.Controls;
using MonitorBilansuKalorycznego.Data;
using MonitorBilansuKalorycznego.Model;

namespace MonitorBilansuKalorycznego.View
{
    public partial class FoodDatabaseView : UserControl
    {
        private ApplicationData _appData;
        private bool _isShowingProducts = true;

        public FoodDatabaseView(ApplicationData appData)
        {
            InitializeComponent();
            _appData = appData;
            RefreshList();
        }

        private void Tab_Products_Checked(object sender, RoutedEventArgs e)
        {
            _isShowingProducts = true;
            UpdateViewMode();
            RefreshList();
        }

        private void Tab_Activities_Checked(object sender, RoutedEventArgs e)
        {
            _isShowingProducts = false;
            UpdateViewMode();
            RefreshList();
        }
        // METODA DO ZMIANY WYGLĄDU (NAPISY I KOLUMNY)
        private void UpdateViewMode()
        {
            // STRAŻNIK: Jeśli kolumny jeszcze nie istnieją (bo okno się ładuje), nic nie rób.
            if (ColKcalProd == null || ColProt == null || TxtHeaderTitle == null)
                return;

            if (_isShowingProducts)
            {
                // Napisy
                TxtHeaderTitle.Text = "Twoje produkty";
                TxtHeaderSubtitle.Text = "Zarządzaj nazwami i wartościami odżywczymi produktów";

                // Kolumny
                ColKcalProd.Visibility = Visibility.Visible;
                ColProt.Visibility = Visibility.Visible;
                ColCarbs.Visibility = Visibility.Visible;
                ColFat.Visibility = Visibility.Visible;

                ColKcalAct.Visibility = Visibility.Collapsed; // Ukryj spalanie
            }
            else
            {
                // Napisy
                TxtHeaderTitle.Text = "Twoje aktywności";
                TxtHeaderSubtitle.Text = "Zarządzaj listą ćwiczeń i ich spalaniem kalorii";

                // Kolumny
                ColKcalProd.Visibility = Visibility.Collapsed;
                ColProt.Visibility = Visibility.Collapsed;
                ColCarbs.Visibility = Visibility.Collapsed;
                ColFat.Visibility = Visibility.Collapsed;

                ColKcalAct.Visibility = Visibility.Visible; // Pokaż spalanie
            }
        }
        private void RefreshList()
        {
            if (SearchBox == null || _appData == null) return;
            string query = SearchBox.Text.ToLower();

            if (_isShowingProducts)
                ItemsGrid.ItemsSource = _appData.FoodManager.Find(p => p.Name.ToLower().Contains(query));
            else
                ItemsGrid.ItemsSource = _appData.ActivityManager.Find(a => a.Name.ToLower().Contains(query));
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => RefreshList();

        // --- DODAWANIE ---
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_isShowingProducts)
            {
                var addWindow = new AddProductWindow();
                if (addWindow.ShowDialog() == true)
                {
                    _appData.FoodManager.Add(addWindow.NewProduct);
                    RefreshList();
                }
            }
            else
            {
                // Otwieramy nowe okno aktywności
                var actWindow = new AddActivityWindow();
                if (actWindow.ShowDialog() == true)
                {
                    _appData.ActivityManager.Add(actWindow.ActivityData);
                    RefreshList();
                }
            }
        }

        // --- EDYCJA (NOWOŚĆ) ---
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_isShowingProducts)
            {
                var product = ((Button)sender).DataContext as FoodProduct;
                if (product == null) return;

                // Otwieramy okno w trybie edycji (przekazujemy produkt)
                var editWindow = new AddProductWindow(product);
                if (editWindow.ShowDialog() == true)
                {
                    // Dane zostały zaktualizowane wewnątrz obiektu, wystarczy zapisać plik
                    _appData.FoodManager.SaveToFile();
                    RefreshList();
                }
            }
            else
            {
                var activity = ((Button)sender).DataContext as PhysicalActivity;
                if (activity == null) return;

                var editWindow = new AddActivityWindow(activity);
                if (editWindow.ShowDialog() == true)
                {
                    _appData.ActivityManager.SaveToFile();
                    RefreshList();
                }
            }
        }

        // --- USUWANIE ---
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno usunąć?", "Potwierdzenie", MessageBoxButton.YesNo) == MessageBoxResult.No) return;

            if (_isShowingProducts)
            {
                var item = ((Button)sender).DataContext as FoodProduct;
                if (item != null) _appData.FoodManager.Remove(item);
            }
            else
            {
                var item = ((Button)sender).DataContext as PhysicalActivity;
                if (item != null) _appData.ActivityManager.Remove(item);
            }
            RefreshList();
        }
    }
}