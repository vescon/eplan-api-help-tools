using System;
using System.Collections.Generic;
using System.IO;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.DataModel.Graphics;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using PlaceholderConfigLoader.Data;
using PlaceholderConfigLoader.EplAddin;
using PlaceholderConfigLoader.Extensions;

namespace PlaceholderConfigLoader
{
    class LoadAction : ActionBase<LoadAction>, IEplAction
    {
        public static string Description => "Loads a placeholder's configuration.";

        public static string MenuCaption => "Load placeholder's configuration...";

        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                var placeholder = EplanHelper.GetSelectedPlaceholder();
                if (placeholder == null)
                {
                    MsgError("Please, select a single placeholder object.");
                    return true;
                }

                var openFileDlg = new OpenFileDialog
                {
                    Title = "Select a placeholder's configuration",
                    Filter = "Placeholder's configuration|*.json|All files|*.*"
                };

                if (openFileDlg.ShowDialog() != true) return true;

                PlaceholderConfig config;
                try
                {
                    config = File.ReadAllText(openFileDlg.FileName).FromJson<PlaceholderConfig>();
                }
                catch (Exception ex)
                {
                    MsgError(
                        $"Error reading placeholder's configuration from '{openFileDlg.FileName}'."
                        + Environment.NewLine + ex.Message);
                    return true;
                }

                try
                {
                    using (new LockingStep())
                    {
                        placeholder.LockObject();

                        placeholder.Name = config.Name;

                        if (config.ClearAll)
                        {
                            DeleteAllRecords(placeholder);

                            placeholder.DeleteUnusedVariables();
                        }

                        foreach (var set in config.Sets)
                        {
                            AddRecord(placeholder, set);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MsgError("Error setting placeholder's configuration." + Environment.NewLine + ex.Message);
                    return true;
                }

                // done
                return true;
            });
        }

        private void AddRecord(PlaceHolder placeholder, PlaceholderVariablesSet set)
        {
            if (set.Name.IsEmpty()) return;

            DeleteRecord(placeholder, set.Name);

            placeholder.AddRecord(set.Name);

            foreach (var variable in set.Variables)
            {
                if (variable.Key.IsEmpty()) continue;

                AddValue(placeholder, set, variable);
            }

            if (set.Enabled)
            {
                using (new LockingStep())
                {
                    try
                    {
                        foreach (var assignedObject in placeholder.AssignedObjects)
                            assignedObject.LockObject();

                        placeholder.ApplyRecord(set.Name);
                    }
                    catch (Exception ex)
                    {
                        MsgWarning("Error applying placeholder's variables set." + Environment.NewLine + ex.Message);
                    }
                }
            }
        }

        private static void AddValue(
            PlaceHolder placeholder, 
            PlaceholderVariablesSet set, 
            KeyValuePair<string, object> variable)
        {
            var mls = new MultiLangString();

            if (variable.Value is string)
            {
                mls.AddString(ISOCode.Language.L___, (string)variable.Value);

                placeholder.SetValue(set.Name, variable.Key, mls);
            }
            else if (variable.Value is JObject)
            {
                foreach (var languageValue in (JObject)variable.Value)
                {
                    var value = languageValue.Value;

                    if (value.Type != JTokenType.String)
                        continue;

                    var id = languageValue.Key;
                    switch (id.ToLowerInvariant())
                    {
                        case "en":
                        case "en_us":
                            mls.AddString(ISOCode.Language.L_en_US, value.Value<string>());
                            break;

                        case "de":
                        case "de_de":
                            mls.AddString(ISOCode.Language.L_de_DE, value.Value<string>());
                            break;

                        case "_":
                            mls.AddString(ISOCode.Language.L___, value.Value<string>());
                            break;

                        // TODO: other languages
                    }
                }
            }
            else
            {
                return;
            }

            placeholder.SetValue(set.Name, variable.Key, mls);
        }

        private static void DeleteRecord(PlaceHolder placeholder, string recordName)
        {
            var i = placeholder.FindRecord(recordName);
            if (i < 0) return;
            placeholder.DeleteRecord(i);
        }

        private static void DeleteAllRecords(PlaceHolder placeholder)
        {
            var recordNames = placeholder.GetRecordNames();
            foreach (var recordName in recordNames)
                placeholder.DeleteRecord(recordName);
        }
    }
}