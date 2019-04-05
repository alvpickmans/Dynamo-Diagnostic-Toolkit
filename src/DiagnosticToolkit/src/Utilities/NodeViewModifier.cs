using Dynamo.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DiagnosticToolkit.Utilities
{
    public static class NodeViewModifier
    {
        public static NodeView ChangeBackground(this NodeView nodeView, int r, int g, int b)
        {
            Rectangle background = ((Rectangle)nodeView.grid.FindName("nodeBackground"));
            background.Fill = new SolidColorBrush(Color.FromArgb(100, Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b)));
            return nodeView;
        }
    }
}
