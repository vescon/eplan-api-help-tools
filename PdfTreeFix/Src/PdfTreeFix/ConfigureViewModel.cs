using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Vescon.Common.Extensions;
using Vescon.Eplan.Utilities;

namespace PdfTreeFix
{
    public class ConfigureViewModel : INotifyPropertyChanged
    {
        private NodeItem _selectedNode;

        public ConfigureViewModel(
            StructurePropertyId[] pageStructure, 
            ProjectStructureHelper projectStructureHelper)
        {
            ProjectStructureHelper = projectStructureHelper;

            NodeItems.AddRange(pageStructure
                .Select((x, i) => new NodeItem(
                    this,
                    x, 
                    pageStructure.Skip(i + 1))));
        }

        public ProjectStructureHelper ProjectStructureHelper { get; }

        public bool GenerateCompactDescriptions { get; set; }

        public List<NodeItem> NodeItems { get; } = new List<NodeItem>();

        public bool IsListEnabled => SelectedNode != null && SelectedNode.Enabled;

        public NodeItem SelectedNode
        {
            get
            {
                return _selectedNode;
            }

            set
            {
                _selectedNode = value;

                OnPropertyChanged(nameof(IsListEnabled));

                SubItems.Clear();

                if (value == null)
                    return;

                foreach (var i in value.SubItems)
                    SubItems.Add(i);
            }
        }

        public ObservableCollection<NodeItem.SubNodeItem> SubItems { get; } = new ObservableCollection<NodeItem.SubNodeItem>();

        public bool IsValid => SubItems.All(x => x.IsValid);

        public class NodeItem 
        {
            private readonly ConfigureViewModel _data;

            private bool enabled;

            public NodeItem(
                ConfigureViewModel data, 
                StructurePropertyId propertyId, 
                IEnumerable<StructurePropertyId> subPropertyIds)
            {
                _data = data;

                PropertyId = propertyId;

                Name = data.ProjectStructureHelper.GetStructurePropertyName(propertyId);

                SubItems = subPropertyIds
                    .Select(x => new SubNodeItem(data, x))
                    .Concat(new SubNodeItem[]
                    {
                        new DescriptionNodeItem(data),
                        new DummyNodeItem(),
                        new AlsoAtSubStructuresNodeItem(data)
                    })
                    .ToArray();
            }

            public StructurePropertyId PropertyId { get; }

            public bool Enabled
            {
                get
                {
                    return enabled;
                }

                set
                {
                    enabled = value;

                    _data.SelectedNode = this;
                    _data.OnPropertyChanged(nameof(SelectedNode));
                    _data.OnPropertyChanged(nameof(IsListEnabled));
                }
            }

            public string Name { get; set; }

            public SubNodeItem[] SubItems { get; }

            public class SubNodeItem : IDataErrorInfo
            {
                private readonly ConfigureViewModel _data;

                public SubNodeItem(
                    ConfigureViewModel data = null, 
                    StructurePropertyId propertyId = 0)
                {
                    _data = data;

                    if (data != null 
                        && propertyId > 0)
                    {
                        PropertyId = propertyId;
                        PropertyName = data.ProjectStructureHelper.GetStructurePropertyName(PropertyId);
                    }

                    IsValid = true;
                }

                public StructurePropertyId PropertyId { get; } = 0;

                public string PropertyName { get; set; }

                public bool Enabled { get; set; }

                public string Limit { get; set; }
                public virtual Visibility HasLimit => Visibility.Visible;

                public virtual Visibility HasCheckBox => Visibility.Visible;

                public bool IsValid { get; private set; }

                public string this[string columnName]
                {
                    get
                    {
                        if (HasLimit == Visibility.Visible
                            && columnName == "Limit")
                        {
                            int d;
                            IsValid = Limit.IsEmpty() || (int.TryParse(Limit, out d) && d > 0);

                            _data.OnPropertyChanged(nameof(ConfigureViewModel.IsValid));

                            if (!IsValid)
                                return "Must be empty or an integer bigger than 0.";
                        }

                        return null;
                    }
                }

                public string Error { get; }
            }

            public class DescriptionNodeItem : SubNodeItem
            {
                public DescriptionNodeItem(ConfigureViewModel data)
                    : base(data)
                {
                    PropertyName = "With original description";
                }

                public override Visibility HasLimit => Visibility.Collapsed;
            }

            public class DummyNodeItem : SubNodeItem
            {
                public DummyNodeItem()
                {
                    PropertyName = string.Empty;
                }

                public override Visibility HasLimit => Visibility.Collapsed;
                public override Visibility HasCheckBox => Visibility.Collapsed;
            }

            public class AlsoAtSubStructuresNodeItem : SubNodeItem
            {
                public AlsoAtSubStructuresNodeItem(ConfigureViewModel data)
                    : base(data)
                {
                    PropertyName = "Also at detached substructures";
                }

                public override Visibility HasLimit => Visibility.Collapsed;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}