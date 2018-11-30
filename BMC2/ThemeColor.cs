using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BMC2
{
    class ThemeColor
    {
        public static SolidColorBrush BackGround { get; set; } = new SolidColorBrush(Colors.Black);
        public static SolidColorBrush TextColor { get; set; } = new SolidColorBrush(Colors.LightGray);
        public static SolidColorBrush NameColor { get; set; } = new SolidColorBrush(Colors.Orange);
        public static SolidColorBrush MouseOver { get; set; } = new SolidColorBrush(Colors.DimGray);
        public static SolidColorBrush IDColor { get; set; } = new SolidColorBrush(Colors.IndianRed);
        public static SolidColorBrush RetweeterColor { get; set; } = new SolidColorBrush(Colors.DarkGray);
    }
}
