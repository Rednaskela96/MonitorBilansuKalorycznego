using System;
using System.Collections.Generic;
using System.Linq;

namespace MonitorBilansuKalorycznego.Model
{
    public class MealSetItem
    {
        public FoodProduct Product { get; set; } = new FoodProduct();
        public double Amount { get; set; }

        public double CalculateCalories()
        {
            return Product.CalculateCaloriesForAmount(Amount);
        }
    }

    public class MealSet
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public List<MealSetItem> Items { get; set; } = new List<MealSetItem>();

        public double TotalCalories => Items.Sum(i => i.CalculateCalories());

        public string ItemsSummary => Items.Count > 0
            ? string.Join(", ", Items.Select(i => i.Product.Name))
            : "Pusty zestaw";

        public override string ToString() => Name;
    }
}
