using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.MasterData;

namespace PlaceholderConfigLoader.Extensions
{
    static class Other
    {
        public static string ToStringSafe(this PropertyValue propValue)
        {
            return propValue.IsEmpty ? string.Empty : propValue.ToString();
        }

        public static string ToStringSafe(this MDPropertyValue propValue)
        {
            return propValue.IsEmpty ? string.Empty : propValue.ToString();
        }

        public static bool? ToSingleValue(this IEnumerable<bool?> values)
        {
            var existingValues = values.Where(x => x.HasValue).Select(x => x.Value);
            bool value;
            return SameValues(existingValues, out value) 
                ? value 
                : (bool?)null;
        }

        public static int? ToInt(this string value)
        {
            int result;
            if (string.IsNullOrEmpty(value)
                || !int.TryParse(value, out result))
                return null;
            return result;
        }

        public static int ToInt(this string value, int defaultValue)
        {
            int result;
            if (string.IsNullOrEmpty(value)
                || !int.TryParse(value, out result))
                return defaultValue;
            return result;
        }

        public static MultiLangString ToMultiLangString(this string text)
        {
            var mls = new MultiLangString();
            mls.AddString(ISOCode.Language.L___, text);
            return mls;
        }

        public static bool IsNotEmpty(this string value)
        {
            return !value.IsEmpty();
        }

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string MakeSureNotExists(this string path)
        {
            if (!File.Exists(path))
                return path;

            var ext = Path.GetExtension(path);
            var i = 1;
            while (File.Exists(Path.ChangeExtension(path, ext + i)))
                ++i;

            return Path.ChangeExtension(path, ext + i);
        }

        public static bool Differs<T>(this T object1, T object2, string propertyName)
            where T : class
        {
            if (object2 == null)
                return true;

            var propInfo = object1.GetType().GetProperty(propertyName);
            if (propInfo == null)
                return false;

            bool differs;
            if (propInfo.PropertyType == typeof(string))
            {
                differs = !Equals(
                    ((propInfo.GetValue(object1, null) as string) ?? string.Empty).Trim(),
                    ((propInfo.GetValue(object2, null) as string) ?? string.Empty).Trim());
            }
            else
            {
                differs = !Equals(propInfo.GetValue(object1, null), propInfo.GetValue(object2, null));
            }

            return differs;
        }

        private static bool SameValues<T>(IEnumerable<T> values, out T value)
        {
            values = values as IList<T> ?? values.ToList();
            if (values.Any())
            {
                value = values.First();
                var v1 = value;
                if (values.Skip(1).Any(x => !x.Equals(v1)))
                    return false;
                return true;
            }

            value = default(T);
            return false;
        }
    }
}
