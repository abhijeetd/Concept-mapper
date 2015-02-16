using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ConceptMapper.Framework.Adorners
{
    public class BorderAdorner : Adorner
    {
        VisualCollection visualChildren;

        public BorderAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            Rect adornedArea = new Rect(this.AdornedElement.DesiredSize);
            adornedArea.Inflate(new Size(5, 5));
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Transparent);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.LightGray), 2);

            drawingContext.DrawLine(renderPen, adornedArea.TopLeft, adornedArea.TopRight);
            drawingContext.DrawLine(renderPen, adornedArea.TopLeft, adornedArea.BottomLeft);
            drawingContext.DrawLine(renderPen, adornedArea.BottomLeft, adornedArea.BottomRight);
            drawingContext.DrawLine(renderPen, adornedArea.TopRight, adornedArea.BottomRight);
        }

        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }

    }
}
