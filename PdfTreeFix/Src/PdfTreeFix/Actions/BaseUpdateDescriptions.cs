using System;
using System.Collections.Generic;
// ReSharper disable once RedundantUsingDirective
using System.Diagnostics;
using System.Linq;

using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using PdfTreeFix.Actions.Data;
using Vescon.Common.Extensions;
using Vescon.Eplan.Utilities;
using Vescon.Eplan.Utilities.Action;
using Vescon.Eplan.Utilities.Misc;

namespace PdfTreeFix.Actions
{
    public abstract class BaseUpdateDescriptions : ActionBase
    {
        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                Project project;
                if (!GetCurrentProject(out project))
                    return true;

                var configSettings = Addin.ReadConfigSettings(project, Error);
                if (configSettings == null)
                    return true;

                CompactDescriptions = configSettings.GenerateCompactDescriptions;

                StructureHelper = new ProjectStructureHelper(project);

                var msg = "Descriptions of the following structure nodes will be generated:"
                    + Addin.GetEnabledNodesWarningMessage(configSettings.NodeSettings, StructureHelper);

                if (!Confirm(msg))
                    return true;

                using (new UndoManager().CreateStep(Addin.Title + ", " + GetMenuCaption()))
                using (new LockingStep())
                {
                    var enabledItems = StructureHelper.PagesNavigatorStructure
                        .Select(x => configSettings.NodeSettings.SingleOrDefault(y => y.PropertyId == x && y.Enabled))
                        .Where(x => x != null)
                        .ToList();

                    var projectInfo = new ProjectInfo(project, StructureHelper);

                    enabledItems.ForEach(x =>
                    {
                        var propertyId = x.PropertyId;

                        var currentStructurePropertyValues = projectInfo.Pages
                            .Select(y => y.NameParts[propertyId])
                            .ToSet();

                        var subPartSeparator = StructureHelper.GetStructurePropertySubPartSeparator(propertyId);

                        var nodeNames = GetDistinctNodeNames(
                            currentStructurePropertyValues,
                            subPartSeparator);

                        projectInfo.DistinctNodeNames[propertyId] = nodeNames;
                    });

                    using (var progress = new Progress("SimpleProgress"))
                    {
                        progress.SetTitle(Addin.Title);
                        progress.SetAllowCancel(true);
                        progress.SetAskOnCancel(true);

                        var steps = CountSteps(
                            projectInfo, 
                            projectInfo.Pages,
                            enabledItems, 
                            0);

                        progress.SetNeededSteps(steps + 1);
                        progress.ShowImmediately();

                        // first, restore nodes' descriptions for not enabled items
                        RestoreDescriptions(
                            projectInfo,
                            configSettings, 
                            progress);

                        // now, generate nodes' descriptions for enabled items
                        SetDescriptions(
                            projectInfo,
                            projectInfo.Pages,
                            enabledItems, 
                            0,
                            progress,
                            string.Empty);

                        if (progress.Canceled())
                        {
                            Info("Operation cancelled by user.");
                            return true;
                        }
                    }
                }

                Info("Done." 
                    + Environment.NewLine + "(Please, select the Pages Navigator and press F5 to refresh it.)");

                return true;
            });
        }

        protected override string Title => Addin.Title;

        protected ProjectStructureHelper StructureHelper { get; private set; }
        protected bool CompactDescriptions { get; private set; }

        protected abstract string GetMenuCaption();

        protected abstract string GetInfo(IEnumerable<PageInfo> pages, StructurePropertyId propertyId, int limit);
        protected abstract string GetInfoCompact(IEnumerable<PageInfo> pages, StructurePropertyId propertyId, int limit);

        private void RestoreDescriptions(
            ProjectInfo projectInfo,
            ConfigurationSettings configSettings, 
            Progress p)
        {
            p.SetActionText("Restoring descriptions...");
            p.Step(1);

            var notEnabled = StructureHelper.PagesNavigatorStructure
                .Where(x => configSettings.NodeSettings.SingleOrDefault(y => y.PropertyId == x && y.Enabled) == null)
                .ToList();

            notEnabled
                .ForEach(x =>
                {
                    var locations = projectInfo.Project.GetLocationObjects(x.GetLocationType());

                    locations
                        .ForEach(l =>
                        {
                            l.LockObject();
                                
                            var mls = !l.Properties.LOCATION_DESCRIPTION_2.IsEmpty
                                  ? l.Properties.LOCATION_DESCRIPTION_2.ToMultiLangString()
                                  : new MultiLangString();

                            l.Properties.LOCATION_DESCRIPTION = mls;
                        });
                });
        }

        private void SetDescriptions(
            ProjectInfo project,
            IList<PageInfo> pages,
            IList<StructurePropertySetting> items, 
            int index,
            Progress p,
            string parentNodeName)
        {
            p.SetActionText(parentNodeName);
            p.Step(1);
            if (p.Canceled())
                return;

            var item = items[index];
            var propertyId = item.PropertyId;

            var nodeNames = project.DistinctNodeNames[propertyId];

            var hierarchyId = propertyId.GetLocationType();
            var subPartSeparator = StructureHelper.GetStructurePropertySubPartSeparator(propertyId);

            nodeNames
                .Concat(string.Empty.AsEnumerable())
                .ForEach(nodeName =>
                {
                    if (p.Canceled())
                        return;

                    var containedPages = GetContainedPages(
                        nodeName, 
                        pages,
                        items,
                        index,
                        subPartSeparator);

                    if (containedPages.IsEmpty())
                        return;

                    if (nodeName.IsNotEmpty())
                    {
                        var location = project.Locations
                            .Single(x => x.HierarchyId == hierarchyId && x.Name == nodeName);
                        
                        SetDescription(
                            location,
                            containedPages,
                            item);
                    }

                    if (index == items.Count - 1)
                        return;

                    SetDescriptions(
                        project,
                        containedPages,
                        items, 
                        index + 1,
                        p,
                        parentNodeName + (nodeName.IsNotEmpty() ? "\\" + nodeName : string.Empty));
                });
        }

        private void SetDescription(
            LocationInfo location,
            IList<PageInfo> pages,
            StructurePropertySetting item)
        {
            if (location.CalculatedText == LocationInfo.Undetermined)
                return;

            var info = item.SubSettings
                .Where(x => x.Enabled)
                .Select(x =>
                {
                    if (x.PropertyId == 0)
                        return null;

                    return CompactDescriptions 
                       ? GetInfoCompact(pages, x.PropertyId, x.Limit) 
                       : GetInfo(pages, x.PropertyId, x.Limit);
                })
                .Where(x => x.IsNotEmpty())
                .ToList()
                .Concatenate(", ");

            if (info.IsNotEmpty() 
                && location.CalculatedText != null
                && info != location.CalculatedText)
            {
                info = "?";
            }

            location.SetDescription(
                info,
                item.DescriptionIncluded);
        }

        private int CountSteps(
            ProjectInfo project,
            IList<PageInfo> pages,
            IList<StructurePropertySetting> items, 
            int index)
        {
            if (index >= items.Count)
                return 0;

            var item = items[index];
            var propertyId = item.PropertyId;

            var nodeNames = project.DistinctNodeNames[propertyId];
            
            var subPartSeparator = StructureHelper.GetStructurePropertySubPartSeparator(propertyId);

            var result = 1;

            nodeNames
                .ForEach(nodeName =>
                {
                    var containedPages = GetContainedPages(
                        nodeName, 
                        pages,
                        items,
                        index,
                        subPartSeparator);

                    if (containedPages.IsEmpty())
                        return;
                    
                    result += CountSteps(
                        project,
                        containedPages,
                        items, 
                        index + 1);
                });

            return result;
        }

        private List<string> GetDistinctNodeNames(IEnumerable<string> values, string subPartSeparator)
        {
            return values
                .Distinct()
                .Where(x => x.IsNotEmpty())
                .SelectMany(x =>
                {
                    var parts = subPartSeparator.IsNotEmpty()
                        ? x.Split(subPartSeparator.AsArray(), StringSplitOptions.None)
                        : x.AsArray();

                    return parts
                        .Select((y, i) => parts.Take(i + 1).Concatenate(subPartSeparator));
                })
                .Distinct()
                .ToList();
        }

        private IList<PageInfo> GetContainedPages(
            string nodeName, 
            IList<PageInfo> pages, 
            IList<StructurePropertySetting> items, 
            int index,
            string subPartSeparator)
        {
            var item = items[index];

            if (item.OnlyTopNode
                && subPartSeparator.IsNotEmpty() 
                && nodeName.Contains(subPartSeparator))
            {
                return new List<PageInfo>();
            }

            var containedPages = pages
                .Where(x => x.NameParts[item.PropertyId] == nodeName)
                .ToList();

            if (subPartSeparator.IsNotEmpty())
            {
                // add those which are deeper in hierarchy
                containedPages.AddRange(pages
                    .Where(x => x.NameParts[item.PropertyId].StartsWith(nodeName + subPartSeparator)));
            }

            return containedPages;
        }
    }
}