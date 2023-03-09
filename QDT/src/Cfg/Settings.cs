using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace Vescon.EplAddin.Qdt.Cfg
{
    public interface IWindow
    {
        string GetFormId();

        void LoadAdditionalConfigData(Data data);

        void SaveAdditionalConfigData(Data data);
    }

    public class Settings
    {
        public const string SubPath = @"Vescon\MDM\";
        public const string Ext = ".cfg";

        private static Settings _inst;
        private SortedList _data;

#if DEBUG
        private bool _disposed;
#endif

        private Settings(Stream streamIn)
        {
            // load
            _data = new SortedList();

            if (streamIn != null)
            {
                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Binder = new TypeBinder();
                    var data = (CfgSettingsData)formatter.Deserialize(streamIn);
                    for (var i = 0; i < data.Keys.Length && i < data.Values.Count; ++i)
                    {
                        _data.Add(data.Keys[i], data.Values[i]);
                    }
                }
                catch
#if DEBUG
                (Exception)
#endif
                {
                }
            }
        }

#if DEBUG
        ~Settings()
        {
            if (!_disposed && _data != null)
            {
                Debug.Fail("Configuration not saved!");
            }
        }
#endif

        public static void Load()
        {
            var stream = GetStream(true);
            try
            {
                Load(stream);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        public static void Load(Stream streamIn)
        {
#if DEBUG
            if (_inst != null)
            {
                _inst._disposed = true;
            }
#endif
            _inst = new Settings(streamIn);
        }

        public static void Save(Stream streamOut = null)
        {
            var closeStream = false;
            try
            {
                if (_inst != null 
                    && _inst._data != null)
                {
                    if (streamOut == null)
                    {
                        streamOut = GetStream(false);
                        closeStream = true;
                    }

                    var data = new CfgSettingsData(_inst._data);
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(streamOut, data);
                }

                if (_inst != null)
                    _inst._data = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
#if DEBUG
                MessageBox.Show(ex.Message, ex.Source);
#endif
            }
            finally
            {
                if (streamOut != null
                    && closeStream)
                {
                    streamOut.Close();
                }
            }
        }

        public static Data GetData(string prefix)
        {
            if (_inst == null
                || _inst._data == null)
            {
                Load();
            }

            return new Data(_inst._data) { Prefix = prefix };
        }

        public static void Add(IWindow dlg)
        {
            var frm = dlg as Form;
            if (frm != null)
            {
                frm.VisibleChanged += FormVisibleChanged;
                frm.Closed += FormClosed;
            }
        }

        private static FileStream GetStream(bool toRead)
        {
            var filename = Assembly.GetExecutingAssembly().GetName().Name + Ext;
            var configFilename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                SubPath,
                filename);

            if (toRead)
                return File.Exists(configFilename)
                           ? new FileStream(configFilename, FileMode.Open, FileAccess.Read, FileShare.Read)
                           : null;
            
            if (Directory.Exists(Path.GetDirectoryName(configFilename)) == false)
                Directory.CreateDirectory(Path.GetDirectoryName(configFilename));

            return new FileStream(configFilename, FileMode.Create, FileAccess.Write, FileShare.None);
        }

        private static void FormVisibleChanged(object sender, EventArgs args)
        {
            var frm = sender as Form;
            var frmCfg = sender as IWindow;
            if (frm != null 
                && frm.Visible 
                && frmCfg != null 
                && frm.WindowState == FormWindowState.Normal)
            {
                var cd = GetData(frmCfg.GetFormId());

                frmCfg.LoadAdditionalConfigData(cd);

                if (frm.FormBorderStyle == FormBorderStyle.Sizable 
                    || frm.FormBorderStyle == FormBorderStyle.SizableToolWindow)
                {
                    frm.Width = cd.Get("#Width#", frm.Width);
                    frm.Height = cd.Get("#Height#", frm.Height);
                }

                var location = cd.Get("#Location#", new Point(frm.Left, frm.Top));
                frm.Left = location.X;
                frm.Top = location.Y;

                // make sure it's not outside the screen
                var rctScreen = Screen.GetWorkingArea(frm);
                if (frm.Left + frm.Width > rctScreen.Right)
                {
                    frm.Left = rctScreen.Right - frm.Width - 10;
                }

                if (frm.Top + frm.Height > rctScreen.Bottom)
                {
                    frm.Top = rctScreen.Bottom - frm.Height - 10;
                }

                if (frm.Top < rctScreen.Top)
                {
                    frm.Top = rctScreen.Top + 10;
                }

                if (frm.Left < rctScreen.Left)
                {
                    frm.Left = rctScreen.Left + 10;
                }
            }
        }

        private static void FormClosed(object sender, EventArgs e)
        {
            var frm = sender as Form;
            var frmCfg = sender as IWindow;
            if (frm != null 
                && frmCfg != null 
                && _inst != null
                && _inst._data != null
                && frm.WindowState == FormWindowState.Normal)
            {
                var cd = GetData(frmCfg.GetFormId());

                cd.Insert("#Width#", frm.Width);
                cd.Insert("#Height#", frm.Height);
                cd.Insert("#Location#", new Point(frm.Left, frm.Top));

                frmCfg.SaveAdditionalConfigData(cd);
            }
        }

        [Serializable]
        private class CfgSettingsData
        {
            public readonly string[] Keys;
            public readonly ArrayList Values;

            public CfgSettingsData()
            {
            }

            public CfgSettingsData(SortedList data)
            {
                Keys = new string[data.Count];
                var i = 0;
                foreach (string s in data.Keys)
                {
                    Keys[i] = s;
                    ++i;
                }

                Values = new ArrayList(data.Values);
            }
        }

        private class TypeBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                if (typeName.EndsWith("CfgSettingsData"))
                    return typeof(CfgSettingsData);

                return Type.GetType($"{typeName}, {assemblyName}");
            }
        }
    }
}
