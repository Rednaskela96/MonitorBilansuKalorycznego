using System;
using System.Collections.Generic;
using System.IO;            
using System.Linq;          
using Newtonsoft.Json;

namespace MonitorBilansuKalorycznego.Data
{
    public class DataManager<T> where T : class
    {
        private string _filePath;
        public List<T> Items { get; private set; } = new List<T>();

        public DataManager(string fileName)
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }

        public void Add(T item)
        {
            Items.Add(item);
            SaveToFile();
        }

        public void Remove(T item)
        {
            Items.Remove(item);
            SaveToFile();
        }

        public List<T> Find(Func<T, bool> predicate)
        {
            return Items.Where(predicate).ToList();
        }

        public void SaveToFile()
        {
            string json = JsonConvert.SerializeObject(Items, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public void LoadFromFile()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                Items = JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
            }
        }
    }
}