using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace Gmap.Localization
{
    public class LocalizationCache
    {
        Dictionary<string, string> dict;
        public LocalizationCache(string[] keys, string[] values)
        {
            if (keys.Length != values.Length)
                throw new System.Exception("Keys and values must be the same length");

            dict = new Dictionary<string, string>();
            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i], values[i]);
        }

        public string Get(string key)
        {
            if (!dict.TryGetValue(key, out string value)) {
                return $"[{key} NOT FOUND]";
            }
            return value;
        }
    }

    public class LocalizationCacheCollection
    {
        Dictionary<string, LocalizationCache> dict 
            = new Dictionary<string, LocalizationCache>();

        public void Add(string key, LocalizationCache cache)
        {
            dict.Add(key, cache);
        }

        public LocalizationCache Get(string key)
        {
            if (!dict.TryGetValue(key, out LocalizationCache cache)) {
                return null;
            }
            return cache;
        }
    }

    public class LocalizationDatabaseFactory
    {
        public static LocalizationDatabase[] FromDataTable(DataTable dataTable)
        {
            int numberOfLanguages = dataTable.Columns.Count-1;
            LocalizationDatabase[] databases = new LocalizationDatabase[numberOfLanguages];

            string[] keys = dataTable.AsEnumerable().Select(r => r.Field<string>("key")).ToArray();

            for (int i = 0; i < numberOfLanguages; i++)
            {
                string columnName = dataTable.Columns[i+1].ColumnName;
                string[] values = dataTable.AsEnumerable().Select(r=>r.Field<string>(columnName)).ToArray();
                LocalizationDatabase db = ScriptableObject.CreateInstance<LocalizationDatabase>();
                db.LanguageKey = dataTable.Columns[i+1].ColumnName;
                db.SetData(keys, values);
                databases[i] = db;
            }

            return databases;
        }
    }

    [CreateAssetMenu(menuName="Gmap/Localization/Localization Database")]
    public class LocalizationDatabase : ScriptableObject
    {
        public string LanguageKey="--";

        [SerializeField] string[] keys;
        [SerializeField] string[] values;

        private static LocalizationCacheCollection databases;
        private LocalizationCache GetCache()
        {
            LocalizationCache cache = databases.Get(LanguageKey);
            if (cache == null)
            {
                cache = new LocalizationCache(keys, values);
                databases.Add(LanguageKey, cache);
            }
            return cache;
        }

        public string GetString(string key)
        {
            var cache = GetCache();
            return cache.Get(key);
        }

        public void SetData(string[] keys, string[] values)
        {
            if (keys.Length != values.Length)
                throw new System.Exception("Keys and values must be the same length");
            
            this.keys = keys;
            this.values = values;
        }
    }
}