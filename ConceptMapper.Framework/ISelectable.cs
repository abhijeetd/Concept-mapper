using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ConceptMapper.Framework
{
    public interface ISelectable
    {
        Concept SelectedObject { get; }
        void Select();
        void Unselect();
    }
}
