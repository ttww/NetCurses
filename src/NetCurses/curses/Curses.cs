using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tw.curses
{
    public class Curses
    {
        private const bool DIRECT_MODE = true;

        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public int Foreground { get; internal set; } = WHITE;
        public int Background { get; internal set; } = BLACK;

        public const int BLACK          = 0;
        public const int DARK_RED       = 1;
        public const int DARK_GREEN     = 2;
        public const int DARK_YELLOW    = 3;
        public const int DARK_BLUE      = 4;
        public const int DARK_MAGENTA   = 5;
        public const int DARK_CYAN      = 6;
        public const int DARK_GRAY      = 8;
        public const int GRAY           = 7;
        public const int RED            = 9;
        public const int GREEN          = 10;
        public const int YELLOW         = 11;
        public const int BLUE           = 12;
        public const int MAGENTA        = 13;
        public const int CYAN           = 14;

        public const int WHITE          = 255;

        public const int KEY_UP    = 1000;
        public const int KEY_DOWN  = 1001;
        public const int KEY_LEFT  = 1002;
        public const int KEY_RIGHT = 1003;

        public enum CursesOptions
        {
            None             = 0,
            ResizeScreen     = 1,   // Resize Window (iTerm option may prevent this)
            DisableQuickEdit = 2,   // Windows
            DeleteCloseBox   = 4,   // Windoes, remove close box
        };

        private ScreenBuffer virtuel;

        private IOutputDriver output;

        public Curses(
            int width, int height,
            int foregroundColor = Curses.WHITE,
            int backgroundColor = Curses.BLACK,
            CursesOptions cursesOptions = CursesOptions.None
            )
        {
            //if (width < 16)
            //    throw new ArgumentException("width must be >= 16");

            Width = width;
            Height = height;


            output = new AnsiOutputDriver();

            output.InitDriver(width, height, foregroundColor, backgroundColor, cursesOptions);

            virtuel = new ScreenBuffer(Width, Height, ' ', Foreground, Background);
        }

        public void ClearScreen()
        {
            virtuel.X = 0;
            virtuel.Y = 0;
        }

        public void GotoXY(int x, int y)
        {
            if (x < 1)
                throw new Exception($"GotoXY: x < 1:  {x}");
            if (x > Width)
                throw new Exception($"GotoXY: x > {Width}:  {x}");

            if (y < 1)
                throw new Exception($"GotoXY: y < 1:  {y}");
            if (y > Height)
                throw new Exception($"GotoXY: y > {Height}:  {y}");

            virtuel.X = x - 1;
            virtuel.Y = y - 1;
            virtuel.IsDirty = true;
        }

        public void Write(string s)
        {

            foreach (char c in s)
            {
                if (c == '\n')
                {
                    virtuel.NextLine();
                    continue;
                }
                virtuel.Write(c, Foreground, Background);
            }
        }

        public void ExitCurses() {
            GotoXY(1, Height);
            Write("\n");
            CursorOn();
            Refresh();

            output.ExitDriver();
        }

        public void InsertLine(int y)
        {

        }

        public void SetScrollBox(int top, int left, int bottom, int right)
        {

        }

        public void CursorOn()
        {
            virtuel.CursorVisible = true;
            virtuel.IsDirty = true;
        }

        public void CursorOff()
        {
            virtuel.CursorVisible = false;
            virtuel.IsDirty = true;
        }

        public void Refresh()
        {
            if (virtuel.IsDirty) output.SyncScreen(virtuel);
        }

        public int ReadKey(bool wait = true)
        {
            Refresh();

            return output.ReadKey(wait);
        }

        public int CharAt(int x, int y)
        {
            return virtuel.Lines[y - 1].Line[x - 1].Character;
        }

        private const int STATE_STACK_SIZE = 10;
        private int[] foregroundStack = new int[STATE_STACK_SIZE];
        private int[] backgroundStack = new int[STATE_STACK_SIZE];
        private int statePointer = 0;

        public void SaveState()
        {
            if (statePointer >= STATE_STACK_SIZE)
                throw new Exception($"Save state stack overflow, max is {STATE_STACK_SIZE}");
            foregroundStack[statePointer] = Foreground;
            backgroundStack[statePointer] = Background;
            statePointer++;
        }

        public void RestoreState()
        {
            if (statePointer <= 0)
                throw new Exception($"Restore state stack is empty");

            statePointer--;
            Foreground = foregroundStack[statePointer];
            Background = backgroundStack[statePointer];
        }
    }
}
