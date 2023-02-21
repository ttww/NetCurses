using System;

namespace tw.curses
{
    internal class CursesUtils
    {
        internal static void DrawBox(Curses c, int xp, int yp, int width, int height, char d, int foregroundColor, int backgroundColor)
        {
            c.SaveState();

            string ds = d.ToString();

            int right = xp + width - 1;
            int bottom = yp + height - 1;

            c.Foreground = foregroundColor;
            c.Background = backgroundColor;

            for (int x = xp; x <= right; x++)
            {
                c.GotoXY(x, yp); c.Write(ds);
                c.GotoXY(x, bottom); c.Write(ds);
            }
            for (int y = yp; y <= bottom; y++)
            {
                c.GotoXY(xp, y); c.Write(ds);
                c.GotoXY(right, y); c.Write(ds);
            }

            c.RestoreState();
        }

        internal static void DrawGrafBox(Curses c, int xp, int yp, int width, int height, int foregroundColor, int backgroundColor)
        {
            c.SaveState();

            // from https://www.w3.org/TR/xml-entity-names/025.html
            const string topBottom = "─";
            const string leftRight = "│";
            const string tlEdge = "┌";
            const string trEdge = "┐";
            const string blEdge = "└";
            const string brEdge = "┘";

            int right = xp + width - 1;
            int bottom = yp + height - 1;

            c.Foreground = foregroundColor;
            c.Background = backgroundColor;

            c.GotoXY(xp, yp);        c.Write(tlEdge);
            c.GotoXY(xp, bottom);    c.Write(blEdge);
            c.GotoXY(right, yp);     c.Write(trEdge);
            c.GotoXY(right, bottom); c.Write(brEdge);

            for (int x = xp+1; x < right; x++)
            {
                c.GotoXY(x, yp); c.Write(topBottom);
                c.GotoXY(x, bottom); c.Write(topBottom);
            }
            for (int y = yp+1; y < bottom; y++)
            {
                c.GotoXY(xp, y); c.Write(leftRight);
                c.GotoXY(right, y); c.Write(leftRight);
            }

            c.RestoreState();
        }

    }
}