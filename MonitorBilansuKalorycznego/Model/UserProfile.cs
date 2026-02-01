using System;

namespace MonitorBilansuKalorycznego.Model
{
    // UserProfile — profil użytkownika z danymi osobowymi i logiką obliczeniową.
    // Używa ENUM-ów (Gender, ActivityLevel) do typowania danych.
    public class UserProfile
    {
        public string Name { get; set; } = "Użytkownik";
        public int Age { get; set; }
        public double Weight { get; set; }   // w kg
        public double Height { get; set; }   // w cm
        public Gender Gender { get; set; }
        public ActivityLevel ActivityLevel { get; set; }

        // Cel ręczny (jeśli 0 — wyliczymy automatycznie z BMR/TDEE)
        public double CustomDailyCalorieGoal { get; set; }

        // Obliczanie BMR (Basal Metabolic Rate) — wzór Mifflina-St Jeora
        public double CalculateBMR()
        {
            double bmr = (10 * Weight) + (6.25 * Height) - (5 * Age);

            if (Gender == Gender.Male)
                bmr += 5;
            else
                bmr -= 161;

            return bmr;
        }

        // Obliczanie TDEE (Total Daily Energy Expenditure) — BMR * mnożnik aktywności
        public double CalculateTDEE()
        {
            double bmr = CalculateBMR();
            double multiplier = 1.2;

            //wybór mnożnika na podstawie poziomu aktywności (enum)
            switch (ActivityLevel)
            {
                case ActivityLevel.Sedentary: multiplier = 1.2; break;
                case ActivityLevel.LightlyActive: multiplier = 1.375; break;
                case ActivityLevel.ModeratelyActive: multiplier = 1.55; break;
                case ActivityLevel.VeryActive: multiplier = 1.725; break;
                case ActivityLevel.ExtraActive: multiplier = 1.9; break;
            }

            return Math.Round(bmr * multiplier);
        }
        
        private const double DefaultCalorieGoal = 2000;

        // Zwraca cel kaloryczny: ręczny > automatyczny > domyślny 2000 kcal
        public double GetDailyCalorieGoal()
        {
            if (CustomDailyCalorieGoal > 0)
                return CustomDailyCalorieGoal;

            if (Weight <= 0 || Height <= 0 || Age <= 0)
                return DefaultCalorieGoal;

            return CalculateTDEE();
        }
    }
}
