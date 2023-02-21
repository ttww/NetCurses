

using tw.curses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace NetCurses
{
    internal class Program
    {

        static void Main(string[] args)
        {
            int w = 60;
            int h = 20;
            Curses c = null;

            try
            {


                c = new Curses(w, h, Curses.WHITE, Curses.BLACK);

                //c.GotoXY(1, 1);
                //c.Write("1234567890");
                //c.GotoXY(5, 1);
                //c.Background = Curses.RED;
                //c.Write("r");
                //c.Background = Curses.WHITE;
                //c.Write("w");
                //c.GotoXY(5, 1);
                //c.ReadKey();

                ////return;

                //c.GotoXY(1, 1); c.Write("1,1");
                //c.GotoXY(2, 2); c.Write("2,2");
                //c.GotoXY(3, 3); c.Write("3,3");
                //c.GotoXY(4, 4); c.Write("4,4");

                //c.ReadKey();

                //c.Refresh();

                //Console.Write("aaa");
                //Console.Write("\u001b[48;5;1m");
                //Console.Write("bbb");
                //Console.Write("\u001b[48;5;0m");
                //Console.Write("ccc");


                //c.ReadKey();

                c.CursorOff();

                CursesUtils.DrawGrafBox(c, 1, 1, c.Width , c.Height, Curses.RED, Curses.RED);

                WormTest wormTest = new WormTest(c, height: c.Height - 10);

                CursesUtils.DrawGrafBox(c, 2, 11 , c.Width - 2 , c.Height - 11, Curses.DARK_GRAY, Curses.GRAY);

                int i = 0;
                bool quit = false;
                
                // quit = true;
                while (!quit)
                {
                    c.GotoXY(6, 7);

                    c.Background = Curses.GREEN;
                    c.Write($"- {i} - 😉  ");
                    c.Background = Curses.BLACK;

                    i++;
                    wormTest.Loop(ref quit);

                    Thread.Sleep(50);
                }

            }
            catch (Exception)
            {
                if (c != null)
                    c.ExitCurses();
                throw;
            }
            finally {
                if (c != null)
                    c.ExitCurses();
            }
        }
    }
}
