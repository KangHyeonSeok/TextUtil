using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            File.WriteAllText(filePath, JsonConvert.SerializeObject(root));

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
            if (root.ContainsKey("_upmCi") && root["_upmCi"] is JObject)
            {
                if (root["_upmCi"] is JObject upmCi && upmCi.ContainsKey("footprint"))
                {
                    if (upmCi["footprint"] is not JObject footprint)
                        return string.Empty;
                    return footprint.ToString();
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
                child = new JObject();
            
            if(child != null)
                child[childPropertyName] = childValue;
        }
    }
}
