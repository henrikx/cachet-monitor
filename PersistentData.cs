using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;

namespace cachet_monitor
{
    class PersistentData
    {
        public static void SavePersistentData(Dictionary<string, string> failedComponents, Dictionary<string, string> trackedIncidents, Dictionary<string, int> hostFailedCount, string path = "./Data.json")
        {
            List<dynamic> dataToSave = new List<dynamic>()
            {
                { failedComponents },
                { trackedIncidents },
                { hostFailedCount }
            };
            try
            {
                File.WriteAllText(path, JsonSerializer.Serialize(dataToSave, new JsonSerializerOptions() { IgnoreNullValues = true }));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while writing data:");
                throw ex;
            }

        }
        public static List<dynamic> LoadPersistentData(string path = "./Data.json")
        {
            List<dynamic> dataLoaded = new List<dynamic>()
            {
                { new Dictionary<string, object>() }, { new Dictionary<string, object>() }, { new Dictionary<string, object>() } //create data structure in case file doesn't exist. prevent nullreferenceexception
            };
            try
            {
                dataLoaded = JsonSerializer.Deserialize<List<dynamic>>(File.ReadAllText(path), new JsonSerializerOptions() { Converters = { new ObjectToInferredTypesConverter() } });
                return dataLoaded;
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    return dataLoaded;
                }
                Console.WriteLine("Error occured while loading data:");
                throw ex;
            }
        }
    }
}
