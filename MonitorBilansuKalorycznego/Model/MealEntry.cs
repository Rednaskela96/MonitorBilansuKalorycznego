using System;

namespace MonitorBilansuKalorycznego.Model
{
    // Reprezentuje jeden wpis posiłku w dzienniku (produkt + ilość gramów).
    public class MealEntry : BaseEntry
    {
        public DateTime MealTime { get; set; } = DateTime.Now;
        
        public FoodProduct Product { get; set; } = new FoodProduct();

        public double Amount { get; set; }

        //Oblicza kalorie na podstawie produktu i ilości
        public override double CalculateCalories()
        {
            if (Product == null) return 0;
            return Product.CalculateCaloriesForAmount(Amount);
        }

        // OVERRIDE metody wirtualnej — zwraca czytelne podsumowanie posiłku
        public override string GetSummary()
            => $"{Product.Name} — {Amount}g ({CalculateCalories():N0} kcal)";

        // Właściwość pomocnicza do bindowania w XAML
        public double CaloriesDisplay => CalculateCalories();
    }
}
