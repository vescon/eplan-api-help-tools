using System;
using System.Linq;
using System.Xml.Linq;

using Eplan.EplApi.DataModel;
using Eplan.EplApi.DataModel.Graphics;

using EplPropertyPlacement = Eplan.EplApi.DataModel.Graphics.PropertyPlacement;

namespace Vescon.EplAddin.Qdt
{
    public class PropertyPlacement
    {
        public PropertyPlacement(XElement propertyElement)
        {
            var attrs = propertyElement.Attributes().ToList();

            Id = attrs.FirstOrDefault(x => x.Name == "A771").ToShortInt();

            Index = attrs.FirstOrDefault(x => x.Name == "A772").ToShortInt();

            var location = attrs.FirstOrDefault(x => x.Name == "A501").ToLocation();
            LocationX = location[0];
            LocationY = location[1];

            Visibility = GetVisibility(
                attrs.FirstOrDefault(x => x.Name == "A404"),
                attrs.FirstOrDefault(x => x.Name == "A406"));

            IsDocked = attrs.Any(x => x.Name == "A502");

            DockingDirection = GetDockingDirection(attrs.FirstOrDefault(x => x.Name == "A503"));
                
            var additionalElement = propertyElement.Elements()
                .FirstOrDefault(x => x.Name == "S54x505");

            if (additionalElement == null)
            {
                Angle = double.NaN;
                Alignment = 0;
            }
            else
            {
                Angle = additionalElement.Attributes().FirstOrDefault(x => x.Name == "A962").ToDouble();
                Alignment = (TextBase.JustificationType)additionalElement.Attributes().FirstOrDefault(x => x.Name == "A966").ToShortInt();
            }
        }

        public PropertyPlacement(EplPropertyPlacement propertyPlacement)
        {
            Id = propertyPlacement.DisplayedProperty.AsInt;
            Index = propertyPlacement.DisplayedProperty.Index;
            LocationX = propertyPlacement.Location.X;
            LocationY = propertyPlacement.Location.Y;
            Visibility = propertyPlacement.IsSetAsVisible;
            IsDocked = propertyPlacement.IsDocked;
            Angle = propertyPlacement.Rotation;
            Alignment = propertyPlacement.Justification;
            DockingDirection = propertyPlacement.DockingDirection;

            TextHeight = propertyPlacement.Height;
            AlignmentBoxActivated = propertyPlacement.HasBox;
        }

        private int Id { get; set; }
        private int Index { get; set; }
        private double LocationX { get; set; }
        private double LocationY { get; set; }
        private Placement.Visibility Visibility { get; set; }
        private bool IsDocked { get; set; }
        private double Angle { get; set; }
        private TextBase.JustificationType Alignment { get; set; }
        private TextBase.TextDockingDirection DockingDirection { get; set; }
        
        private double TextHeight { get; set; }
        private bool AlignmentBoxActivated { get; set; }

        public bool EqualsOld(PropertyPlacement other)
        {
            return Id == other.Id
                && Index == other.Index
                && LocationIsTheSame(other)
                && Visibility == other.Visibility
                && IsDocked == other.IsDocked
                && (IsDocked || AngleIsEqual(other))
                && (IsDocked || Alignment == other.Alignment)
                && (IsDocked || DockingDirection == other.DockingDirection);
        }

        public bool Equals(PropertyPlacement other)
        {
            return Id == other.Id
                && Index == other.Index
                && LocationIsTheSame(other)
                && Visibility == other.Visibility
                && TextHeightIsEqual(other)
                && AlignmentBoxActivated == other.AlignmentBoxActivated
                && IsDocked == other.IsDocked
                && (IsDocked || AngleIsEqual(other))
                && (IsDocked || Alignment == other.Alignment)
                && (IsDocked || DockingDirection == other.DockingDirection);
        }

        private bool LocationIsTheSame(PropertyPlacement other)
        {
            return Math.Abs(LocationX - other.LocationX) < .00001
                   && Math.Abs(LocationY - other.LocationY) < .00001;
        }

        public override string ToString()
        {
            return string.Format(
                "Id: {0}, Index: {1}, X: {2}, Y: {3}, Visible: {4}, IsDocked: {5}, Angle: {6}, Alignment: {7}, Docking: {8}",
                Id,
                Index,
                LocationX,
                LocationY,
                Visibility,
                IsDocked,
                Angle,
                Alignment,
                DockingDirection);
        }

        private bool AngleIsEqual(PropertyPlacement other)
        {
            return Math.Abs(Angle - other.Angle) < 0.000001;
        }

        private bool TextHeightIsEqual(PropertyPlacement other)
        {
            return Math.Abs(TextHeight - other.TextHeight) < 0.000001;
        }

        private Placement.Visibility GetVisibility(XAttribute a1, XAttribute a2)
        {
            if (a1 == null
                || a2 == null)
                return Placement.Visibility.Undetermined;

            int v1, v2;
            if (int.TryParse(a1.Value, out v1)
                && int.TryParse(a2.Value, out v2))
            {
                if (Math.Abs(v2 & 1) == 1)
                {
                    if (Math.Abs(v1 & 1) == 1)
                        return Placement.Visibility.Visible;
                    return Placement.Visibility.Invisible;
                }

                if (Math.Abs(v1 & 1) == 1)
                    return Placement.Visibility.ByLayer;
            }

            return Placement.Visibility.Undetermined;
        }

        private TextBase.TextDockingDirection GetDockingDirection(XAttribute a503)
        {
            if (a503 != null)
                switch (a503.ToShortInt() & 0x70) // bits 4, 5, 6
                {
                    case 0: return TextBase.TextDockingDirection.DockToBottom;
                    case 16: return TextBase.TextDockingDirection.DockToTop;
                    case 32: return TextBase.TextDockingDirection.DockToRight;
                    case 64: return TextBase.TextDockingDirection.DockToLeft;
                }

            return 0;
        }
    }
}