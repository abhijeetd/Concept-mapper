using ConceptMapper.Framework;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;

namespace ConceptMapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void txtLabel_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            mappingCanvas.SelectedConcept.SelectedObject.ConceptTitle = txtLabel.Text;
        }
    }
}
