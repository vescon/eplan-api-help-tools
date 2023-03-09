using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.MasterData;

namespace Vescon.Eplan.Utilities.Misc
{
    public static class Extensions
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

        public static T[] AsArray<T>(this T item)
        {
            return new[] { item };
        }
        
        public static bool Exists(this ProjectSettings settings, string path)
        {
            return settings.ExistSetting(path);
        }
        
        public static string GetString(this ProjectSettings settings, string path)
        {
            return settings.GetStringSetting(path, 0);
        }
        
        public static UndoStepWrapper CreateStep(this UndoManager undoManager, string description)
        {
            var undoStep = undoManager.CreateUndoStep();
            undoStep.SetUndoDescription(description);
            return new UndoStepWrapper(undoStep);
        }

        public static void Add(this ProjectSettings settings, string path, string value)
        {
#if VERSION_23
            settings.AddStringSetting(
                path,
                new[] { string.Empty },
                new string[0], 
                string.Empty, 
                new[] { value },
                ISettings.CreationFlag.Overwrite);
#else
            settings.AddStringSetting(
                path,
                new[] { value },
                new[] { string.Empty },
                ISettings.CreationFlag.Overwrite);
#endif
        }
        
        public static Project.Hierarchy GetLocationType(this StructurePropertyId propertyId)
        {
            switch (propertyId)
            {
                case StructurePropertyId.HigherLevelFunction:
                    return Project.Hierarchy.Plant;

                case StructurePropertyId.Location:
                    return Project.Hierarchy.Location;

                case StructurePropertyId.FunctionalAssignment:
                    return Project.Hierarchy.Functional;

                case StructurePropertyId.InstallationSite:
                    return Project.Hierarchy.Place;

                case StructurePropertyId.DocumentType:
                    return Project.Hierarchy.Document;

                case StructurePropertyId.UserDefined:
                    return Project.Hierarchy.UserDef;

                case StructurePropertyId.HlfNumber:
                    return Project.Hierarchy.Installation;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static StructurePropertyId GetPropertyId(this Project.Hierarchy hierarchyId)
        {
            switch (hierarchyId)
            {
                case Project.Hierarchy.Plant:
                    return StructurePropertyId.HigherLevelFunction;

                case Project.Hierarchy.Location:
                    return StructurePropertyId.Location;

                case Project.Hierarchy.Functional:
                    return StructurePropertyId.FunctionalAssignment;

                case Project.Hierarchy.Place:
                    return StructurePropertyId.InstallationSite;

                case Project.Hierarchy.Document:
                    return StructurePropertyId.DocumentType;

                case Project.Hierarchy.UserDef:
                    return StructurePropertyId.UserDefined;

                case Project.Hierarchy.Installation:
                    return StructurePropertyId.HlfNumber;

                default:
                    throw new ArgumentOutOfRangeException();
            }
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

        public class UndoStepWrapper : IDisposable
        {
            private readonly UndoStep _undoStep;

            public UndoStepWrapper(UndoStep undoStep)
            {
                _undoStep = undoStep;
            }

            public void Dispose()
            {
                _undoStep.CloseOpenUndo();
                _undoStep.Dispose();
            }
        }
    }
}
