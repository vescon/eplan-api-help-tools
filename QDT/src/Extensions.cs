using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace Vescon.EplAddin.Qdt
{
    public static class Extensions
    {
        public static bool IsEmpty(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        public static bool IsNotEmpty(this string text)
        {
            return !text.IsEmpty();
        }

        public static int ToShortInt(this XAttribute item)
        {
            if (item.Value == "L")
                return -16002; // from layer

            if (int.TryParse(item.Value, out var i))
            {
                var bytes = BitConverter.GetBytes(i);
                return BitConverter.ToUInt16(bytes, 0);
            }

            return 0;
        }

        public static double[] ToLocation(this XAttribute item)
        {
            var r = new[] { 0d, 0d };
            
            if (item != null)
            {
                var ss = item.Value.Split('/');
                double.TryParse(ss[0], NumberStyles.Float, CultureInfo.InvariantCulture, out r[0]);
                double.TryParse(ss[1], NumberStyles.Float, CultureInfo.InvariantCulture, out r[1]);
            }

            return r;
        }

        public static double ToDouble(this XAttribute item)
        {
            if (string.IsNullOrEmpty(item.Value))
                return 0.0;

            if (item.Value == "L") 
                return -16002; // from layer

            double.TryParse(item.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var r);
            return r;
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection) action(item);
        }

        public static void AddIfNotExists<T>(this ICollection<T> collection, T item)
        {
            if (!collection.Contains(item)) collection.Add(item);
        }
    }
}
