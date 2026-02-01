using System;

namespace MonitorBilansuKalorycznego.Attributes
{
    /* CUSTOM ATRYBUT — dziedziczy po System.Attribute.
    Nakładany na właściwości modeli (np. FoodProduct) za pomocą [DisplayName("...")].
    Odczytywany w runtime przez REFLEKSJĘ w ReflectionHelper.
    [AttributeUsage] określa, że atrybut można stosować tylko na Property.*/
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayNameAttribute : Attribute  // Dziedziczenie po Attribute
    {
        public string Name { get; }
        public string Unit { get; }

        // Konstruktor z parametrem opcjonalnym (unit)
        public DisplayNameAttribute(string name, string unit = "")
        {
            Name = name;
            Unit = unit;
        }
    }
}
