using Newtonsoft.Json;

namespace PlaceholderConfigLoader.Extensions
{
    public static class Json
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, CreateDefaultSettings());
        }

        public static T FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, CreateDefaultSettings());
        }

        private static JsonSerializerSettings CreateDefaultSettings()
        {
            return new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
        }
/*
        public static string ToJsonIncludingTypeNamesAndIgnoringEmptyEnumerables(this object obj)
        {
            return obj.ToJsonIncludingTypeNamesAndIgnoringEmptyEnumerables(null);
        }

        public static string ToJsonIncludingTypeNamesAndIgnoringEmptyEnumerables(
            this object obj,
            JsonSerializerSettings customSettings)
        {
            var settings = CreateDefaultSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;
            settings.ContractResolver = IgnoreEmptyEnumerablesResolver.Instance;
            ApplyCustomSettings(customSettings, settings);

            return JsonConvert.SerializeObject(obj, settings);
        }

        public static T FromJsonIncludingTypeNames<T>(this string json)
        {
            return json.FromJsonIncludingTypeNames<T>(null);
        }

        public static T FromJsonIncludingTypeNames<T>(this string json, JsonSerializerSettings customSettings)
        {
            var settings = CreateDefaultSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;
            ApplyCustomSettings(customSettings, settings);

            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static JsonSerializerSettings CreateDefaultSettingsWithReferenceHandling()
        {
            var settings = CreateDefaultSettings();
            settings.PreserveReferencesHandling = PreserveReferencesHandling.All;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            return settings;
        }

        private static void ApplyCustomSettings(JsonSerializerSettings customSettings, JsonSerializerSettings settings)
        {
            if (customSettings == null)
                return;
            
            settings.ContractResolver = customSettings.ContractResolver;
            settings.Binder = customSettings.Binder;
            settings.Converters = customSettings.Converters;
            settings.PreserveReferencesHandling = customSettings.PreserveReferencesHandling;
            settings.ReferenceLoopHandling = customSettings.ReferenceLoopHandling;
            settings.ReferenceResolverProvider = customSettings.ReferenceResolverProvider;
        }
*/
    }
}