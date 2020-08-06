using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PZ1.Model
{
    public static class GetCoordsFromExistingObject
    {
        public static Rect GetBoundingBox(FrameworkElement child, FrameworkElement parent)
        {
            GeneralTransform transform = child.TransformToAncestor(parent);
            Point topLeft = transform.Transform(new Point(0, 0));
            Point bottomRight = transform.Transform(new Point(child.ActualWidth, child.ActualHeight));
            return new Rect(topLeft, bottomRight);
            //return topLeft;
        }
    }
}
