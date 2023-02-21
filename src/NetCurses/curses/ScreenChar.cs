using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tw.curses
{
    internal class ScreenChar
    {
        internal int Character { get; set; }    // unicode
        internal int ForegroundColor { get; set; }
        internal int BackgroundColor { get; set; }

        internal ScreenChar(int initChar, int foregroundColor, int backgroundColor)
        {
            Character = initChar;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
        }
    }
}
