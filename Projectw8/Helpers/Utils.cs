// Ai Prompt 3: Write a singleton class in C# that contains a generic method to export any model class to JSON format.
// The method should accept any type of object and return its JSON representation as a string.
// Make sure the singleton instance can be accessed globally within the project.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Projectw8.Utils
{
    public class JsonExportUtil
    {
        // Singleton instance
        private static JsonExportUtil? _instance;
        private static readonly object _lock = new object();

        // Prevent external instantiation
        private JsonExportUtil() { }

        // Public accessor for singleton instance
        public static JsonExportUtil Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new JsonExportUtil();
                    }
                }
                return _instance;
            }
        }

        // Generic method to export any class to JSON
        public string ExportToJson<T>(List<T> data, List<string>? selectedProperties = null)
        {
            if (data == null || !data.Any())
                return "[]";

            // If no properties selected, export all
            if (selectedProperties == null || !selectedProperties.Any())
            {
                return JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }

            // Export only selected properties
            var result = data.Select(item => 
            {
                var type = typeof(T);
                var properties = type.GetProperties();
                var selectedObject = new Dictionary<string, object?>();

                foreach (var propName in selectedProperties)
                {
                    var property = properties.FirstOrDefault(p => p.Name == propName);
                    if (property != null)
                    {
                        selectedObject[propName] = property.GetValue(item);
                    }
                }

                return selectedObject;
            }).ToList();

            return JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}