using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorBilansuKalorycznego.Model
{
    public class MealEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime MealTime { get; set; } = DateTime.Now;

        // Powiązanie z produktem (agregacja wg diagramu)
        public FoodProduct Product { get; set; } = new FoodProduct();

        // Ilość w gramach
        public double Amount { get; set; }

        public double CalculateCalories()
        {
            if (Product == null) return 0;
            return Product.CalculateCaloriesForAmount(Amount);
        }
        public double CaloriesDisplay => CalculateCalories();
    }
}