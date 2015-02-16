using ConceptMapper.Framework.Adorners;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace ConceptMapper.Framework
{
    public class Concept : Thumb, ISelectable//, IDraggable, IConnectable, IDiagramObject
    {
        bool hasAdorner = false;
        AdornerLayer adornerLayer;
        BorderAdorner adorner = default(BorderAdorner);

        public List<LineGeometry> EndLines { get; private set; }
        public List<LineGeometry> StartLines { get; private set; }

        public string ConceptTitle
        {
            get { return (string)GetValue(ConceptTitleProperty); }
            set
            {
                SetValue(ConceptTitleProperty, value);
                SetConceptTitle();
            }
        }

        public static readonly DependencyProperty ConceptTitleProperty =
            DependencyProperty.Register("ConceptTitle", typeof(string), typeof(Concept), new PropertyMetadata(string.Empty));

        public Concept()
            : base()
        {
            adorner = new BorderAdorner(this);

            MinWidth = 200;
            MinHeight = 50;
            StartLines = new List<LineGeometry>();
            EndLines = new List<LineGeometry>();
            Background = Brushes.LightGray;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            // Access the textblock element of template and assign it if Title property defined
            SetConceptTitle();
        }

        private void SetConceptTitle()
        {
            if (ConceptTitle != string.Empty && this.Template != null)
            {
                TextBlock text = this.Template.FindName("conceptText", this) as TextBlock;
                if (text != null)
                    text.Text = ConceptTitle;
            }
        }

        #region ISelectable Members
        public Concept SelectedObject { get { return this; } }
        public void Select()
        {
            if (adornerLayer == null)
                adornerLayer = AdornerLayer.GetAdornerLayer(this);

            adornerLayer.Add(adorner);
            hasAdorner = true;
        }

        public void Unselect()
        {
            hasAdorner = false;

            if (adornerLayer != null)
                adornerLayer.Remove(adorner);
        }

        public bool HasAdorner { get { return hasAdorner; } }

        #endregion


        #region Linking logic
        public LineGeometry LinkTo(Concept target)
        {
            LineGeometry line = new LineGeometry();
            LinkTo(target, line);
            return line;
        }

        public bool LinkTo(Concept target, LineGeometry line)
        {
            this.StartLines.Add(line);
            target.EndLines.Add(line);
            this.UpdateLayout();
            target.UpdateLayout();
            line.StartPoint = new Point(Canvas.GetLeft(this) + this.ActualWidth / 2, Canvas.GetTop(this) + this.ActualHeight / 2);
            line.EndPoint = new Point(Canvas.GetLeft(target) + target.ActualWidth / 2, Canvas.GetTop(target) + target.ActualHeight / 2);
            return true;
        }

        public void UpdateLinks()
        {
            double left = Canvas.GetLeft(this);
            double top = Canvas.GetTop(this);

            for (int i = 0; i < this.StartLines.Count; i++)
                this.StartLines[i].StartPoint = new Point(left + this.ActualWidth / 2, top + this.ActualHeight / 2);

            for (int i = 0; i < this.EndLines.Count; i++)
                this.EndLines[i].EndPoint = new Point(left + this.ActualWidth / 2, top + this.ActualHeight / 2);
        }
        #endregion
    }
}
