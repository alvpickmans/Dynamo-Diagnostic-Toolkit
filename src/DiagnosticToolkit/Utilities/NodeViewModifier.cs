using Dynamo.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

        public static NodeView AddTime(this NodeView nodeView, int time)
        {
            //Label label = new Label();
            //label.Content = time + " ms";
            //nodeView.inputGrid.Children.Add(label);
            return nodeView;
        }

        public static Point GetLocation(this NodeView nodeView)
        {
            Point offset = new Point(0, 0);

            nodeView.Dispatcher.Invoke(() =>
            {
                Point point = nodeView.PointToScreen(new Point(0, 0));

                PresentationSource source = PresentationSource.FromVisual(nodeView);

                offset = source.CompositionTarget.TransformFromDevice.Transform(point);

                //Vector offset = VisualTreeHelper.GetOffset(nodeView);
            });
            return new Point(offset.X, offset.Y);

            
        }

        public static double Map(this double value, double min, double max, double newMin, double newMax)
        {
            double normal = (value - min) / (max - min);
            return (normal * (newMax - newMin)) + newMin;
        }

    }
}
