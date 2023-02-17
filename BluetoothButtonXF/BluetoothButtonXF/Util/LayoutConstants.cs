using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BluetoothButtonXF.Util
{
    public class LayoutConstants
    {
        public static double PC_SCREEN_WIDTH = 1600; // If a page is this size or wider, we assume it is not a mobile view, but is on a large monitor.
        public static double PC_SCREEN_HEIGHT = 1600; // If a page is this size or taller, we assume it is not a mobile view, but is on a large monitor.

        public static Color LIST_SELECTED_TEXT_DARK = Color.Black;
        public static Color LIST_SELECTED_BG_DARK = Color.Orange;
        public static Color LIST_UNSELECTED_TEXT_DARK = Color.AntiqueWhite;
        public static Color LIST_UNSELECTED_BG_DARK = Color.Black;

        public static Color LIST_SELECTED_TEXT_LIGHT = Color.White;
        public static Color LIST_SELECTED_BG_LIGHT = Color.Orange;
        public static Color LIST_UNSELECTED_TEXT_LIGHT = Color.Black;
        public static Color LIST_UNSELECTED_BG_LIGHT = Color.LightGray;
    }
}
