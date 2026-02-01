using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorBilansuKalorycznego.Model
{
    public class UserProfile
    {
        public string Name { get; set; } = "Użytkownik";
        public int Age { get; set; }
        public double Weight { get; set; } // w kg
        public double Height { get; set; } // w cm
        public Gender Gender { get; set; }
        public ActivityLevel ActivityLevel { get; set; }

        // Cel ręczny (np. user chce jeść 2000 kcal), jeśli 0 - wyliczymy automatycznie
        public double CustomDailyCalorieGoal { get; set; }

        // --- LOGIKA BIZNESOWA ---

        // 1. Obliczanie BMR (Ile organizm spala leżąc i nic nie robiąc)
        public double CalculateBMR()
        {
            // Wzór Mifflina-St Jeora
            double bmr = (10 * Weight) + (6.25 * Height) - (5 * Age);

            if (Gender == Gender.Male)
                bmr += 5;
            else
                bmr -= 161;

            return bmr;
        }

        // 2. Obliczanie TDEE (Całkowite zapotrzebowanie z uwzględnieniem aktywności)
        public double CalculateTDEE()
        {
            double bmr = CalculateBMR();
            double multiplier = 1.2; // Domyślnie siedzący

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