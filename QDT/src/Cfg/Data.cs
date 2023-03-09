using System.Collections;
using System.Drawing;
using System.Reflection;

namespace Vescon.EplAddin.Qdt.Cfg
{
    public class Data
    {
        private readonly SortedList _data;

        public Data(SortedList data)
        {
            _data = data;
        }

        public string Prefix { get; set; }

        public void Insert(string key, object data)
        {
            Insert(Prefix ?? string.Empty, key, data);
        }

        public void Insert(string frmName, string key, object data)
        {
            if (data is Color)
            {
                var c = (Color)data;
                data = new[] { c.A, c.R, c.G, c.B };
            }

            _data[frmName + key] = data;
        }

        public T Get<T>(string key, T defaultValue)
        {
            var value = defaultValue;
            Get(Prefix ?? string.Empty, key, ref value);
            return value;
        }

        public T Get<T>(string formName, string key, T defaultValue)
        {
            var value = defaultValue;
            Get(formName, key, ref value);
            return value;
        }

        public bool Get<T>(string key, ref T value)
        {
            return Get(Prefix ?? string.Empty, key, ref value);
        }

        public bool Get<T>(string formName, string key, ref T value)
        {
            if (!_data.ContainsKey(formName + key))
            {
                return false;
            }

            if (value is Color)
            {
                if (_data[formName + key] is byte[])
                {
                    try
                    {
                        var arr = (byte[])_data[formName + key];
                        if (arr.Length == 4)
                        {
                            value = (T)(object)Color.FromArgb(arr[0], arr[1], arr[2], arr[3]);
                            return true;
                        }
                    }
                    catch
                    {
                    }
                }

                return false;
            }

            if (_data[formName + key] is T)
            {
                try
                {
                    value = (T)_data[formName + key];
                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        public void Get(object instance, PropertyInfo propInfo, object defaultValue)
        {
            if (instance != null 
                && propInfo != null)
            {
                propInfo.SetValue(instance, Get(propInfo.Name, defaultValue), null);
            }
        }

        public void Insert(object instance, PropertyInfo propInfo)
        {
            if (instance != null
                && propInfo != null)
            {
                Insert(propInfo.Name, propInfo.GetValue(instance, null));
            }
        }
    }
}