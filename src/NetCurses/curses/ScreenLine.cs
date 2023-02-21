using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tw.curses
{
    internal class ScreenLine
    {
        internal bool IsDirty { get; set; }
        internal ScreenChar[] Line { get; set; }
        internal ScreenLine(int width, int initChar, int foregroundColor, int backgroundColor)
        {
            Line = new ScreenChar[width];
            for (int x = 0; x < width; x++)
                Line[x] = new ScreenChar(initChar, foregroundColor, backgroundColor);
        }
    }
}
