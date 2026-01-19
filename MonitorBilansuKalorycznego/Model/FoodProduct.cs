using System;
using MonitorBilansuKalorycznego.Interfaces;

namespace MonitorBilansuKalorycznego.Model
{
    public class FoodProduct : ICalorieCalculable
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // POPRAWKA: Inicjalizacja pustym ciągiem znaków
        public string Name { get; set; } = string.Empty;

        public double CaloriesPer100g { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }

        // POPRAWKA: Inicjalizacja pustym ciągiem znaków
        public string Category { get; set; } = string.Empty;
        public double CalculateCalories() => CaloriesPer100g;

        public double CalculateCaloriesForAmount(double grams)
        {
            return (CaloriesPer100g * grams) / 100.0;
        }

        // --- NOWE METODY Z DIAGRAMU ---

        // Walidacja danych (wymagane przez diagram)
        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) return false; // Musi mieć nazwę
            if (CaloriesPer100g < 0) return false;             // Kalorie nie mogą być ujemne
            return true;
        }

        // Klonowanie obiektu (wymagane przez diagram)
        // Tworzy NOWY obiekt z tymi samymi danymi, ale nowym ID
        public FoodProduct Clone()
        {
            return new FoodProduct
            {
                Id = Guid.NewGuid(), // Nowe ID, żeby nie nadpisać oryginału w bazie
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