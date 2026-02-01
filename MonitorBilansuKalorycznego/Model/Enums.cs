namespace MonitorBilansuKalorycznego.Model
{
    // enumy — ograniczają wartości do zdefiniowanego zbioru.
    // Używane w UserProfile do określenia płci i poziomu aktywności.

    public enum Gender
    {
        Male,
        Female
    }

    public enum ActivityLevel
    {
        Sedentary,          // Siedzący (brak ćwiczeń)
        LightlyActive,      // Lekko aktywny (1-3 dni ćwiczeń/tydzień)
        ModeratelyActive,   // Średnio aktywny (3-5 dni)
        VeryActive,         // Bardzo aktywny (6-7 dni)
        ExtraActive         // Ekstra aktywny (praca fizyczna/sport zawodowy)
    }
}
