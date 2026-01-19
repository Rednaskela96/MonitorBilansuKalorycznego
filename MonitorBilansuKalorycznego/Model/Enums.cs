using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorBilansuKalorycznego.Model
{
    public enum Gender
    {
        Male,
        Female
    }
    public enum ActivityLevel
    {
        Sedentary,          // Siedzący (Brak ćwiczeń)
        LightlyActive,      // Lekko aktywny (1-3 dni ćwiczeń)
        ModeratelyActive,   // Średnio aktywny (3-5 dni)
        VeryActive,         // Bardzo aktywny (6-7 dni)
        ExtraActive         // Ekstra aktywny (praca fizyczna/sport zawodowy)
    }
}
