using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using MonitorBilansuKalorycznego.Interfaces;
using Newtonsoft.Json;

namespace MonitorBilansuKalorycznego.Data
{
    /* CUSTOM DELEGAT — definiuje sygnaturę metod, które mogą obsłużyć zdarzenie.
    Delegat to typ referencyjny wskazujący na metodę o określonej sygnaturze. */
    public delegate void DataChangedEventHandler<T>(object sender, DataChangedEventArgs<T> args);
    
    // CUSTOM EventArgs — klasa pomocnicza przekazująca dane zdarzenia. DZIEDZICZENIE po EventArgs 
    public class DataChangedEventArgs<T> : EventArgs
    {
        public T? Item { get; }
        public string Action { get; }  // "Add", "Remove", "Save"

        public DataChangedEventArgs(T? item, string action)
        {
            Item = item;
            Action = action;
        }
    }
    
    /* KLASA GENERYCZNA — DataManager<T> zarządza kolekcją obiektów dowolnego typu T.
    zapisuje/odczytuje dane z pliku JSON, custom event DataChanged informuje subskrybentów o zmianach. */
    public class DataManager<T>: ISerializable where T : class
    {
        private string _filePath;
        
        public List<T> Items { get; private set; } = new List<T>();

        // CUSTOM EVENT — zdarzenie wywoływane po zmianie danych (DELEGAT + EVENT)
        public event DataChangedEventHandler<T>? DataChanged;

        // Metoda protected virtual — może być nadpisana w klasach potomnych
        protected virtual void OnDataChanged(T? item, string action)
        {
            DataChanged?.Invoke(this, new DataChangedEventArgs<T>(item, action));
        }

        public DataManager(string fileName)
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }

        public void Add(T item)
        {
            Items.Add(item);
            SaveToFile();
            OnDataChanged(item, "Add");
        }

        public void Remove(T item)
        {
            Items.Remove(item);
            SaveToFile();
            OnDataChanged(item, "Remove");
        }

        // DELEGAT Func<T, bool> jako parametr
        public List<T> Find(Func<T, bool> predicate)
        {
            return Items.Where(predicate).ToList();  // LINQ Where()
        }
        
        // Implementacja ISerializable.ToJson()
        public string ToJson()
        {
            return JsonConvert.SerializeObject(Items, Formatting.Indented);
        }

        // Implementacja ISerializable.FromJson()
        public void FromJson(string json)
        {
            Items = JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }

        public void SaveToFile()
        {
            string json = ToJson();  // ← używa metody z ISerializable
            File.WriteAllText(_filePath, json);
        }

        public void LoadFromFile()
        {
            string json = File.ReadAllText(_filePath);
            FromJson(json);  // ← używa metody z ISerializable
        }
    }
}
