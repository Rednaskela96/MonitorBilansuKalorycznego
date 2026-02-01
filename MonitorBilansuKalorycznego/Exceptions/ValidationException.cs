using System;

namespace MonitorBilansuKalorycznego.Exceptions
{
    /* WŁASNY WYJĄTEK — dziedziczy po System.Exception.
    Rzucany w widokach. Dodatkowa właściwość FieldName wskazuje, które pole nie przeszło walidacji.*/
    public class ValidationException : Exception  // <-- DZIEDZICZENIE po Exception
    {
        // Nazwa pola, które nie przeszło walidacji (np. "Name", "CaloriesPer100g")
        public string FieldName { get; }

        // Konstruktor z nazwą pola i komunikatem błędu
        public ValidationException(string fieldName, string message)
            : base(message)  // <-- wywołanie konstruktora klasy bazowej (Exception)
        {
            FieldName = fieldName;
        }
    }
}
