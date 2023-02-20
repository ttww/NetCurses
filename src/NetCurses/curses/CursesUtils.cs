using System;

namespace tw.curses
{
    internal class CursesUtils
    {
        internal static void DrawBox(Curses c, int xp, int yp, int width, int height, char d, int foregroundColor, int backgroundColor)
        {
            c.SaveState();

            string ds = d.ToString();

            int right =  xp + width -1;
            int bottom = yp + height-1;

            //c.Foreground = Curses.GREEN;
            //c.Background = Curses.BLACK;

            //for (int x = xp; x <= right; x++)
            //    for (int y = yp; y <= bottom; y++)
            //    {
            //        c.GotoXY(x, y); c.Write("?");
            //    }

            c.Foreground = foregroundColor;
            c.Background = backgroundColor;

            for (int x = xp; x <= right; x++)
            {
                c.GotoXY(x, yp);        c.Write(ds);
                c.GotoXY(x, bottom);    c.Write(ds);
            }
            for (int y = yp; y <= bottom; y++)
            {
                c.GotoXY(xp, y);    c.Write(ds);
                c.GotoXY(right, y); c.Write(ds);
            }

            c.RestoreState();
        }
    }
}