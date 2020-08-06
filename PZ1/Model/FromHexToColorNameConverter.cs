using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Reflection;
using System.Windows.Media;

namespace PZ1.Model
{
    public static class FromHexToColorNameConverter
    {
        public static Color Convert(Brush brush)
        {
            Color clr = (brush as SolidColorBrush).Color;
            Color retColor = new Color();
            string selectedcolorname = null;
            foreach (var colorvalue in typeof(Colors).GetRuntimeProperties())
            {
                if ((Color)colorvalue.GetValue(null) == clr)
                {
                    selectedcolorname = colorvalue.Name;
                    retColor = (Color)colorvalue.GetValue(null);
                }
            }
            //return selectedcolorname;
            return retColor;
        }
    }
}
