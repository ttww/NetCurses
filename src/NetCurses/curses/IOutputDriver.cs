using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tw.curses
{
    internal interface IOutputDriver
    {
        void InitDriver(int width, int height, int foregroundColor, int backgroundColor, Curses.CursesOptions cursesOptions);
        void ExitDriver();

        void SyncScreen(ScreenBuffer virtuel);
        int ReadKey(bool wait);


    }
}
