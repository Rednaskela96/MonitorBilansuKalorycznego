using System;
using MonitorBilansuKalorycznego.Attributes;
using MonitorBilansuKalorycznego.Interfaces;

namespace MonitorBilansuKalorycznego.Model
{
    // IMPLEMENTACJA INTERFEJSU — FoodProduct implementuje ICalorieCalculable.
    public class FoodProduct : ICalorieCalculable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [DisplayName("Nazwa")]
        public string Name { get; set; } = string.Empty;

        [DisplayName("Kalorie", "kcal/100g")]
        public double CaloriesPer100g { get; set; }

        [DisplayName("Białko", "g")]
        public double Protein { get; set; }

        [DisplayName("Węglowodany", "g")]
        public double Carbs { get; set; }

        [DisplayName("Tłuszcze", "g")]
        public double Fat { get; set; }

        [DisplayName("Kategoria")]
        public string Category { get; set; } = string.Empty;

        // Implementacja metody z interfejsu ICalorieCalculable
        public double CalculateCalories() => CaloriesPer100g;

        // Logika oblicza kalorie proporcjonalnie do ilości gramów
        public double CalculateCaloriesForAmount(double grams)
        {
            return (CaloriesPer100g * grams) / 100.0;
        }

        // Walidacja danych modelu
        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) return false;
            if (CaloriesPer100g < 0) return false;
            return true;
        }
        public FoodProduct Clone()
        {
            return new FoodProduct
            {
                Id = Guid.NewGuid(),
                Name = this.Name + " (Kopia)",
                CaloriesPer100g = this.CaloriesPer100g,
                Protein = this.Protein,
                Carbs = this.Carbs,
                Fat = this.Fat,
                Category = this.Category
            };
        }
    }
}
