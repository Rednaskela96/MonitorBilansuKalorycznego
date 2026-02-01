using System;
using System.Collections.Generic;
using System.Linq;

namespace MonitorBilansuKalorycznego.Model
{
    // MealSetItem — element zestawu dań (produkt + ilość gramów).
    public class MealSetItem
    {
        public FoodProduct Product { get; set; } = new FoodProduct();
        public double Amount { get; set; }

        // Deleguje obliczenie kalorii do FoodProduct (AGREGACJA)
        public double CalculateCalories()
        {
            return Product.CalculateCaloriesForAmount(Amount);
        }
    }
    
    // MealSet — zestaw dań łączący wiele produktów w jeden posiłek.
    public class MealSet
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        //lista elementów zestawu
        public List<MealSetItem> Items { get; set; } = new List<MealSetItem>();

        // LINQ Sum() — oblicza łączne kalorie zestawu
        public double TotalCalories => Items.Sum(i => i.CalculateCalories());

        // LINQ Select() + string.Join() — generuje tekstowe podsumowanie zawartości
        public string ItemsSummary => Items.Count > 0
            ? string.Join(", ", Items.Select(i => i.Product.Name))
            : "Pusty zestaw";
        
        public override string ToString() => Name;
    }
}
