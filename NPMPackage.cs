using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace TextUtil
{
    class NPMPackage
    {
        public static bool SetChangeLogAndFootprint(string filePath, string changelog, string footprint)
        {
            if (!File.Exists(filePath))
                return false;

            string json = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(json))
                return false;

            JObject root = JObject.Parse(json);
            SetValueForChildJObject("_upm", "changelog", changelog, root);
            SetValueForChildJObject("_upmCi", "footprint", footprint, root);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(root,Formatting.Indented));
            return true;
        }

        public static string GetFootprint(string filePath)
        {
            if (!File.Exists(filePath))
                return string.Empty;

            string json = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(json))
                return string.Empty;

            JObject root = JObject.Parse(json);
            if (root.ContainsKey("_upmCi") && root["_upmCi"] is JObject _upmCi)
            {
                if (_upmCi != null && _upmCi.ContainsKey("footprint"))
                {
                    if (_upmCi["footprint"] == null)
                        return string.Empty;
                    return _upmCi["footprint"]!.ToString();
                }
            }

            return string.Empty;
        }

        private static void SetValueForChildJObject(string propertyName, string childPropertyName, string childValue, JObject root)
        {
            JObject? child;
            if (root.ContainsKey(propertyName))
                child = root[propertyName] as JObject;
            else
            {
                child = new JObject();
                root[propertyName] = child;
            }
            
            if(child != null)
                child[childPropertyName] = childValue;
        }
    }
}
