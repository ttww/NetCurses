using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tw.curses
{
    internal class ScreenBuffer
    {
        internal int Width { get; set; }
        internal int Height { get; set; }
        internal int X { get; set; }
        internal int Y { get; set; }

        internal bool CursorVisible { get; set; }
        internal bool IsDirty { get; set; }

        internal ScreenLine[] Lines { get; set; }

        internal ScreenBuffer(int width, int height, char initChar, int foreground, int background)
        {
            Width = width;
            Height = height;
            CursorVisible = true;

            Lines = new ScreenLine[height];
            for (int y = 0; y < height; y++)
                Lines[y] = new ScreenLine(width, initChar, foreground, background);
        }

        internal void Write(char c, int foreground, int background)
        {
            ScreenLine line = Lines[Y];
            ScreenChar bufChar = line.Line[X];
            bool dirty = line.IsDirty;

            if (bufChar.Character != c)
            {
                bufChar.Character = c;
                dirty = true;
            }
            if (bufChar.ForegroundColor != foreground)
            {
                bufChar.ForegroundColor = foreground;
                dirty = true;
            }
            if (bufChar.BackgroundColor != background)
            {
                bufChar.BackgroundColor = background;
                dirty = true;
            }

            line.IsDirty = dirty;

            if (dirty) IsDirty = true;

            X++;
            if (X >= Width)
            {
                NextLine();
            }
        }

        internal void NextLine()
        {
            X = 0;        
            if (Y < Width)    // No scrolling, stay on the last line for now....
                Y++;
        }
    }
}
