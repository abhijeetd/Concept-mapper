using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ConceptMapper.Framework
{
    public class ConceptCanvas : Canvas, ISurface
    {
        //public ISelectable SelectedConcept { get; set; }

        private bool addingNewConcept = false;
        private bool isMoving = false;



        public bool IsConceptSelected
        {
            get { return (bool)GetValue(IsConceptSelectedProperty); }
            set { SetValue(IsConceptSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsConceptSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsConceptSelectedProperty =
            DependencyProperty.Register("IsConceptSelected", typeof(bool), typeof(ConceptCanvas), new PropertyMetadata(false));

        
        public ISelectable SelectedConcept
        {
            get { return (ISelectable)GetValue(SelectedConceptProperty); }
            set { SetValue(SelectedConceptProperty, value); }
        }
        // Using a DependencyProperty as the backing store for SelectedConcept.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedConceptProperty =
            DependencyProperty.Register("SelectedConcept", typeof(ISelectable), typeof(ConceptCanvas), new PropertyMetadata(null));

        public Ellipse DrawingPoint { get; set; }
        public GeometryGroup connectors = new GeometryGroup();

        public ConceptCanvas()
        {
            this.PreviewMouseMove += ConceptCanvas_PreviewMouseMove;
            this.PreviewMouseLeftButtonUp += ConceptCanvas_PreviewMouseLeftButtonUp;

            this.PreviewMouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                isMoving = false;
                if (SelectedConcept != null)
                {
                    if (e.Source.Equals(SelectedConcept.SelectedObject) == false)
                    {
                        SelectedConcept.Unselect();
                        SelectedConcept = null;
                    }
                }
            };
            this.Loaded += (object sender, RoutedEventArgs e) => Clear();
        }

        private void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            Concept concept = e.Source as Concept;
            if (SelectedConcept == null)
            {
                Canvas.SetLeft(concept, Canvas.GetLeft(concept) + e.HorizontalChange);
                Canvas.SetTop(concept, Canvas.GetTop(concept) + e.VerticalChange);

                concept.UpdateLinks();
            }
        }

        void ConceptCanvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && SelectedConcept != null)
            {
                isMoving = true;
                var source = e.Source as Concept;
                if (source != null && addingNewConcept == false 
                    //&& currentSelection != null && source.Equals(currentSelection.SelectedObject)
                    )
                {
                    AddNewConcept(e.GetPosition(this), onDragDelta);
                    var line = source.LinkTo(SelectedConcept.SelectedObject);
                    connectors.Children.Add(line);
                }

                var position = e.GetPosition(this);
                ConceptCanvas.SetLeft(SelectedConcept.SelectedObject, position.X - SelectedConcept.SelectedObject.ActualWidth / 2);
                ConceptCanvas.SetTop(SelectedConcept.SelectedObject, position.Y - SelectedConcept.SelectedObject.ActualHeight / 2);
                SelectedConcept.SelectedObject.UpdateLinks();
            }
        }

        void ConceptCanvas_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.Source is Concept)
            {
                if (isMoving == false)
                {
                    if (SelectedConcept != null)
                        SelectedConcept.Unselect();
                    SelectedConcept = e.Source as ISelectable;
                    SelectedConcept.Select();
                    IsConceptSelected = true;
                }
            }
            else
            {
                if (SelectedConcept != null)
                {
                    SelectedConcept.Unselect();
                    IsConceptSelected = false;
                    SelectedConcept = null;
                }
            }

            addingNewConcept = false;
        }

        private static int conceptInstance = 0;
        private void AddNewConcept(Point position, DragDeltaEventHandler onDragDelta)
        {
            addingNewConcept = true;
            var newConcept = new Concept();
            newConcept.DragDelta += onDragDelta;

            newConcept.ConceptTitle = string.Format("Concept  {0}", ++conceptInstance);
            newConcept.Template = Application.Current.Resources["ConceptTemplate"] as ControlTemplate;
            newConcept.UpdateLayout();

            Canvas.SetLeft(newConcept, position.X - newConcept.ActualWidth / 2);
            Canvas.SetTop(newConcept, position.Y - newConcept.ActualHeight / 2);

            this.Children.Add(newConcept);

            if (SelectedConcept != null)
            {
                SelectedConcept.Unselect();
                IsConceptSelected = false;
            }
            SelectedConcept = newConcept;
            SelectedConcept.Select();
            IsConceptSelected = true;

        }

        public void Clear()
        {
            conceptInstance = 0;
            Children.Clear();
            AddGeometryGroup();
            AddNewConcept(new Point(ActualWidth / 2, ActualHeight / 2), onDragDelta);
            DrawingPoint = new Ellipse { Stroke = Brushes.DarkGray };
            this.Children.Add(DrawingPoint);
        }

        private void AddGeometryGroup()
        {
            connectors.Children.Clear();
            this.Children.Add(new Path { StrokeThickness = 4, Stroke = Brushes.LightGray, Data = connectors });
        }
    }
}
