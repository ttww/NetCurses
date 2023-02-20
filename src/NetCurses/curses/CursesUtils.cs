using System;

namespace tw.curses
{
    internal class CursesUtils
    {
        internal static void DrawBox(Curses c, int top, int left, int width, int height, char d, int foregroundColor, int backgroundColor)
        {
            c.SaveState();
            c.Foreground = foregroundColor;
            c.Background = backgroundColor;
            string ds = "" + d;

            int right = left + width -1;
            int bottom = top + height-1;

            for (int x = left; x <= right; x++)
            {
                c.GotoXY(x, top);       c.Write(ds);
                c.GotoXY(x, height);    c.Write(ds);
            }
            for (int y = top; y <= height; y++)
            {
                c.GotoXY(left, y); c.Write(ds);
                c.GotoXY(right, y); c.Write(ds);
            }
            c.RestoreState();
        }
    }
}