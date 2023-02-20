using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tw.curses
{
    internal class ScreenChar
    {
        internal char Character { get; set; }
        internal int ForegroundColor { get; set; }
        internal int BackgroundColor { get; set; }

        internal ScreenChar(char initChar, int foregroundColor, int backgroundColor)
        {
            Character = initChar;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
        }
    }
}
