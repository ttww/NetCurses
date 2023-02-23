using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tw.curses.Demos
{
    internal class WormDemo
    {
        private const int MAX_LEN = 50;

        private Curses c;
        private int w, h;

        private int[] xp = new int[MAX_LEN];
        private int[] yp = new int[MAX_LEN];

        private int head = 0;
        private int lastHead = -1;
        private int tail = 0;
        private int len = 1;

        private Random rnd = new Random();
        private int dir = 1;

        public WormDemo(Curses c, int width = -1, int height = -1)
        {
            this.c = c;
            this.w = width == -1 ? c.Width : width;
            this.h = height == -1 ? c.Height : height;

            CursesUtils.DrawGrafBox(c, 1, 1, w, h, Curses.GRAY, Curses.DARK_GRAY);

            xp[0] = rnd.Next(2, w - 1);
            yp[0] = rnd.Next(2, h - 1);

            dir = rnd.Next(1, 9);

            c.GotoXY(xp[0], yp[0]);
            c.Write("#");
        }

        private static long GetMillis()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        long lastMs = GetMillis();

        
        internal void Loop(ref bool quit)
        {
            int key = c.ReadKey(wait: false);

            if (key == 'q') quit = true;
            if (key == Curses.KEY_LEFT) dir--;
            if (key == Curses.KEY_RIGHT) dir++;
            if (dir > 8) dir = 1;
            if (dir < 1) dir = 8;

            c.GotoXY(8, h);
            c.SaveState();
            c.Foreground = Curses.RED;
            c.Write($"**** d = {dir}   len = {len}   head = {head}   tail = {tail} ****");
            c.RestoreState();

            long ms = GetMillis();
            if (ms - lastMs > 100)
            {
                lastMs = ms;

                int x = xp[head];
                int y = yp[head];

                CalcDir(ref x, ref y);
                if (x > 1 && x < w && y > 1 && y < h)
                {

                    int oldC = c.CharAt(x, y);
                    head++;
                    if (head >= MAX_LEN)
                        head = 0;


                    c.GotoXY(xp[tail], yp[tail]);
                    c.Write(".");

                    xp[head] = x;
                    yp[head] = y;

                    c.GotoXY(xp[head], yp[head]);
                    c.SaveState();
                    c.Foreground = Curses.DARK_RED;
                    c.Background = Curses.RED;
                    c.Write("H");
                    c.RestoreState();

                    if (lastHead != -1 && len > 1)
                    {
                        c.GotoXY(xp[lastHead], yp[lastHead]);

                        c.SaveState();
                        c.Background = Curses.GREEN;
                        c.Write(" ");
                        c.RestoreState();
                    }

                    if (len >= MAX_LEN || oldC != '.')
                    {
                        tail++;
                        if (tail >= MAX_LEN)
                            tail = 0;
                    }
                    else { 
                        len++;
                    }

                    lastHead = head;
                }
            }
        }

        private void CalcDir(ref int x, ref int y)
        {
            switch (dir)
            {
                case 1:
                    x--;
                    y--;
                    break;
                case 2:
                    y--;
                    break;
                case 3:
                    x++;
                    y--;
                    break;
                case 4:
                    x++;
                    break;
                case 5:
                    x++;
                    y++;
                    break;
                case 6:
                    y++;
                    break;
                case 7:
                    x--;
                    y++;
                    break;
                case 8:
                    x--;
                    break;
            }
        }

    }
}
