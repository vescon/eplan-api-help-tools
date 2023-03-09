using System.Linq;
using System.Windows;
using Vescon.Common.Extensions;
using Vescon.Eplan.Utilities;
using Vescon.Eplan.Utilities.Misc;

namespace PdfTreeFix
{
    public partial class ConfigureView
    {
        private readonly ConfigureViewModel _data;

        public ConfigureView(
            StructurePropertyId[] pageStructure,
            ConfigurationSettings configSettings,
            WindowSettings windowSettings, 
            ProjectStructureHelper projectStructureHelper)
        {
            InitializeComponent();

            Title = Addin.Title;

            _data = new ConfigureViewModel(
                pageStructure, 
                projectStructureHelper);

            DataContext = _data;

            Top = windowSettings.Top;
            Left = windowSettings.Left;
            Width = windowSettings.Width;
            Height = windowSettings.Height;
            columnProperty.Width = windowSettings.ColumnPropertyWidth;
            columnLimit.Width = windowSettings.ColumnLimitWidth;

            LoadData(configSettings);

            _data.SelectedNode = _data.NodeItems.SingleOrDefault(x => x.PropertyId == windowSettings.SelectedPropertyId) 
                ?? _data.NodeItems.FirstOrDefault();
        }

        private void LoadData(ConfigurationSettings configSettings)
        {
            _data.GenerateCompactDescriptions = configSettings.GenerateCompactDescriptions;

            _data.NodeItems
                .ForEach(x =>
                {
                    var setting = configSettings.NodeSettings.SingleOrDefault(y => y.PropertyId == x.PropertyId);
                    if (setting == null)
                        return;

                    LoadNodeData(x, setting);
                });
        }

        private void LoadNodeData(ConfigureViewModel.NodeItem nodeItem, StructurePropertySetting setting)
        {
            nodeItem.Enabled = setting.Enabled;
            nodeItem.SubItems
                .ForEach(x =>
                {
                    if (x is ConfigureViewModel.NodeItem.DummyNodeItem)
                    {
                        // nothing
                    }
                    else if (x is ConfigureViewModel.NodeItem.DescriptionNodeItem)
                    {
                        x.Enabled = setting.DescriptionIncluded;
                    }
                    else if (x is ConfigureViewModel.NodeItem.AlsoAtSubStructuresNodeItem)
                    {
                        x.Enabled = !setting.OnlyTopNode;
                    }
                    else
                    {
                        var subSetting = setting.SubSettings.SingleOrDefault(y => y.PropertyId == x.PropertyId);
                        if (subSetting == null)
                            return;

                        x.Enabled = subSetting.Enabled;
                        x.Limit = subSetting.Limit == 0 ? string.Empty : subSetting.Limit.ToString();
                    }
                });
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public class WindowSettings
        {
            public double Top { get; set; } = 10;
            public double Left { get; set; } = 10;
            public double Width { get; set; } = 750;
            public double Height { get; set; } = 200;

            public double ColumnPropertyWidth { get; set; } = 200;
            public double ColumnLimitWidth { get; set; } = 80;

            public StructurePropertyId SelectedPropertyId { get; set; }
        }

        public WindowSettings GetWindowSettings()
        {
            return new WindowSettings
            {
                Top = Top,
                Left = Left,
                Width = Width,
                Height = Height,

                ColumnPropertyWidth = columnProperty.Width,
                ColumnLimitWidth = columnLimit.Width,

                SelectedPropertyId = _data.SelectedNode?.PropertyId ?? 0
            };
        }

        public ConfigurationSettings GetConfigSettings()
        {
            var settings = new ConfigurationSettings
            {
                GenerateCompactDescriptions = _data.GenerateCompactDescriptions,

                NodeSettings = _data.NodeItems
                    .Select(x =>
                    {
                        var setting = new StructurePropertySetting
                        {
                            PropertyId = x.PropertyId,
                            Enabled = x.Enabled,

                            DescriptionIncluded = x.SubItems
                                .OfType<ConfigureViewModel.NodeItem.DescriptionNodeItem>()
                                .Single().Enabled,

                            OnlyTopNode = !x.SubItems
                                .OfType<ConfigureViewModel.NodeItem.AlsoAtSubStructuresNodeItem>()
                                .Single().Enabled
                        };

                        setting.SubSettings.AddRange(x.SubItems
                            .Where(y => y.PropertyId > 0)
                            .Select(y => new StructurePropertySetting.SubSetting
                            {
                                PropertyId = y.PropertyId,
                                Enabled = y.Enabled,
                                Limit = y.Limit.ToInt(0)
                            }));

                        return setting;
                    })
                    .ToList()
            };

            return settings;
        }
    }
}
