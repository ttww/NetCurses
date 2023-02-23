using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static tw.curses.Curses;

namespace tw.curses.Drivers
{
    internal class AnsiOutputDriver : IOutputDriver
    {

        public const string VT_CSI = "\u001b[";
        public const string VT_CLS = VT_CSI + "H\u001b[2J";

        private int foregroundColor;
        private int backgroundColor;

        private CursesOptions cursesOptions;

        private ScreenBuffer screen;


        public void InitDriver(
            int width, int height,
            int foregroundColor, int backgroundColor,
            Curses.CursesOptions cursesOptions)
        {
            this.foregroundColor = foregroundColor;
            this.backgroundColor = backgroundColor;
            this.cursesOptions = cursesOptions;
            
            if (Utils.IsWindows) {

                System.Console.OutputEncoding = System.Text.Encoding.UTF8;

                if (cursesOptions.HasFlag(CursesOptions.ResizeScreen))
                {
                   Console.SetWindowSize(width, height);
                   Console.SetBufferSize(width, height);
                }

                if (cursesOptions.HasFlag(CursesOptions.DeleteCloseBox))
                    Utils.DeleteCloseBox();
                if (cursesOptions.HasFlag(CursesOptions.DisableQuickEdit))
                    Utils.DisableQuickEdit();

                Utils.SetTerminalEmulation();
            }
            if (Utils.IsUnix) {
                Utils.SetupUnixTerminal();

                Console.Write("\u001b%G");
                if (cursesOptions.HasFlag(CursesOptions.ResizeScreen))
                    Console.Write($"{VT_CSI}8;{height};{width}t");  // Resize Terminal
            }

            screen = new ScreenBuffer(
                width, height,
                ' ',
                foregroundColor, backgroundColor);

            Console.Write(VT_CLS);
        }

        public void ExitDriver()
        {
            if (Utils.IsUnix) {
                Utils.CleanupUnixTerminal();
            }
        }

        public void SyncScreen(ScreenBuffer virtuel)
        {
            for (int y = 0; y < screen.Lines.Length; y++)
            {
                ScreenLine virtuelLine = virtuel.Lines[y];
                if (!virtuelLine.IsDirty) continue;

                ScreenLine screenLine = screen.Lines[y];

                int xb = -1;
                int width = screenLine.Line.Length;

                for (int x=0; x < width; x++)
                {
                    if (!HasDiff(screenLine.Line[x], virtuelLine.Line[x]))
                    {
                        // Same character, output changed characters
                        // from xb to x, if we have xb
                        if (xb != -1)
                        {
                            OutputLine(screenLine.Line, virtuelLine.Line, y, xb, x);
                            xb = -1;
                        }
                        continue;
                    }

                    if (xb == -1)
                        xb = x;
                } // for

                if (xb != -1)
                    OutputLine(screenLine.Line, virtuelLine.Line, y, xb, width);
  
                virtuelLine.IsDirty = false;
            }

            GotoXY(virtuel.X, virtuel.Y);
            CursorOnOff(virtuel.CursorVisible);

            virtuel.IsDirty = false;
        }

        private void OutputLine(ScreenChar[] screenLine, ScreenChar[] virtuelLine, int y, int xb, int x)
        {
            GotoXY(xb, y);
            for (int xc = xb; xc < x; xc++)
            {
                if (foregroundColor != virtuelLine[xc].ForegroundColor || screenLine[xc].ForegroundColor != virtuelLine[xc].ForegroundColor)
                {
                    SetForegroundColor(virtuelLine[xc].ForegroundColor);
                    screenLine[xc].ForegroundColor = virtuelLine[xc].ForegroundColor;
                }

                if (backgroundColor != virtuelLine[xc].BackgroundColor || screenLine[xc].BackgroundColor != virtuelLine[xc].BackgroundColor)
                {
                    SetBackgroundColor(virtuelLine[xc].BackgroundColor);
                    screenLine[xc].BackgroundColor = virtuelLine[xc].BackgroundColor;
                }

                screenLine[xc].Character = virtuelLine[xc].Character;
                Write(screenLine[xc].Character);
            }
        }


        private void SetForegroundColor(int col)
        {
            //Console.ForegroundColor = MapColorWindowsConsole(col);
            foregroundColor = col;
            Console.Write(MapColorAnsi(col, false));

        }

        private void SetBackgroundColor(int col)
        {
            //Console.BackgroundColor = MapColorWindowsConsole(col);
            backgroundColor = col;
            Console.Write(MapColorAnsi(col, true));
        }

        private ConsoleColor MapColorWindowsConsole(int col)
        {
            switch (col)
            {
                case Curses.BLACK:              return ConsoleColor.Black;
                case Curses.DARK_RED:           return ConsoleColor.DarkRed;
                case Curses.DARK_GREEN:         return ConsoleColor.DarkGreen;
                case Curses.DARK_YELLOW:        return ConsoleColor.DarkYellow;
                case Curses.DARK_BLUE:          return ConsoleColor.DarkBlue;
                case Curses.DARK_MAGENTA:       return ConsoleColor.DarkMagenta;
                case Curses.DARK_CYAN:          return ConsoleColor.DarkCyan;
                case Curses.DARK_GRAY:          return ConsoleColor.DarkGray;
                case Curses.RED:                return ConsoleColor.Red;
                case Curses.GREEN:              return ConsoleColor.Green;
                case Curses.YELLOW:             return ConsoleColor.Yellow;
                case Curses.BLUE:               return ConsoleColor.Blue;
                case Curses.MAGENTA:            return ConsoleColor.Magenta;
                case Curses.CYAN:               return ConsoleColor.Cyan;
                case Curses.WHITE:              return ConsoleColor.White;
                default: throw new Exception("Unknown color constant");
            }
        }
        private string MapColorAnsi(int col, bool isBg)
        {
            const string VT_COL_EXIT = "m";
            if (isBg)
            {
                const string VT_BG_INI = VT_CSI + "48;5;";

                switch (col)
                {
                    case Curses.BLACK:          return VT_BG_INI + "0" + VT_COL_EXIT;
                    case Curses.DARK_RED:       return VT_BG_INI + "1" + VT_COL_EXIT;
                    case Curses.DARK_GREEN:     return VT_BG_INI + "2" + VT_COL_EXIT;
                    case Curses.DARK_YELLOW:    return VT_BG_INI + "3" + VT_COL_EXIT;
                    case Curses.DARK_BLUE:      return VT_BG_INI + "4" + VT_COL_EXIT;
                    case Curses.DARK_MAGENTA:   return VT_BG_INI + "5" + VT_COL_EXIT;
                    case Curses.DARK_CYAN:      return VT_BG_INI + "6" + VT_COL_EXIT;
                    case Curses.DARK_GRAY:      return VT_BG_INI + "8" + VT_COL_EXIT;
                    case Curses.GRAY:           return VT_BG_INI + "7" + VT_COL_EXIT;
                    case Curses.RED:            return VT_BG_INI + "9" + VT_COL_EXIT;
                    case Curses.GREEN:          return VT_BG_INI + "10" + VT_COL_EXIT;
                    case Curses.YELLOW:         return VT_BG_INI + "11" + VT_COL_EXIT;
                    case Curses.BLUE:           return VT_BG_INI + "12" + VT_COL_EXIT;
                    case Curses.MAGENTA:        return VT_BG_INI + "13" + VT_COL_EXIT;
                    case Curses.CYAN:           return VT_BG_INI + "14" + VT_COL_EXIT;
                    case Curses.WHITE:          return VT_BG_INI + "15" + VT_COL_EXIT;
                    default: throw new Exception("Unknown color constant");
                }
            }

            const string VT_FG_INI = VT_CSI + "38;5;";

            switch (col)
            {
                case Curses.BLACK:          return VT_FG_INI + "0" + VT_COL_EXIT;
                case Curses.DARK_RED:       return VT_FG_INI + "1" + VT_COL_EXIT;
                case Curses.DARK_GREEN:     return VT_FG_INI + "2" + VT_COL_EXIT;
                case Curses.DARK_YELLOW:    return VT_FG_INI + "3" + VT_COL_EXIT;
                case Curses.DARK_BLUE:      return VT_FG_INI + "4" + VT_COL_EXIT;
                case Curses.DARK_MAGENTA:   return VT_FG_INI + "5" + VT_COL_EXIT;
                case Curses.DARK_CYAN:      return VT_FG_INI + "6" + VT_COL_EXIT;
                case Curses.DARK_GRAY:      return VT_FG_INI + "8" + VT_COL_EXIT;
                case Curses.GRAY:           return VT_FG_INI + "7" + VT_COL_EXIT;
                case Curses.RED:            return VT_FG_INI + "9" + VT_COL_EXIT;
                case Curses.GREEN:          return VT_FG_INI + "10" + VT_COL_EXIT;
                case Curses.YELLOW:         return VT_FG_INI + "11" + VT_COL_EXIT;
                case Curses.BLUE:           return VT_FG_INI + "12" + VT_COL_EXIT;
                case Curses.MAGENTA:        return VT_FG_INI + "13" + VT_COL_EXIT;
                case Curses.CYAN:           return VT_FG_INI + "14" + VT_COL_EXIT;
                case Curses.WHITE:          return VT_FG_INI + "15" + VT_COL_EXIT;
                default: throw new Exception("Unknown color constant");
            }
        }

        private bool HasDiff(ScreenChar c1, ScreenChar c2)
        {
            if (c1.Character != c2.Character) return true;
            if (c1.ForegroundColor != c2.ForegroundColor) return true;
            if (c1.BackgroundColor != c2.BackgroundColor) return true;
            return false;
        }

        public int ReadKey(bool wait)
        {
            if (!wait && !Console.KeyAvailable) return 0;

            ConsoleKeyInfo key = Console.ReadKey(true);  // true: --> no echo

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:    return Curses.KEY_UP;
                case ConsoleKey.DownArrow:  return Curses.KEY_DOWN;
                case ConsoleKey.LeftArrow:  return Curses.KEY_LEFT;
                case ConsoleKey.RightArrow: return Curses.KEY_RIGHT;
            }

            return key.KeyChar;
        }

        private void Write(int unicode)
        {
            //Console.Write('-');
            Console.Write(Char.ConvertFromUtf32(unicode));

            //Console.Write('-');
            screen.X++;
            if (screen.X == screen.Width)
            {
                if (screen.Y < screen.Height)    // No scrolling, stay on the last line for now....
                    screen.Y++;

                if (cursesOptions.HasFlag(CursesOptions.ResizeScreen))
                {
                    screen.X = 0;   // Window schould wrap
                }
                else
                {
                    GotoXY(0, screen.Y); // Window may be larger, explicit wrap
                }
            }

        }

        private void GotoXY(int x, int y)
        {
            if (screen.X != x || screen.Y != y)
            {
                screen.X = x;
                screen.Y = y;

                x++;
                y++;
                //Console.SetCursorPosition(x, y);
                Console.Write(VT_CSI);
                if (y != 1) Console.Write(y.ToString());
                if (x != 1)
                {
                    Console.Write(';');
                    Console.Write(x.ToString());
                }
                Console.Write('H');
            }
        }

        private void CursorOnOff(bool visible)
        {
            if (screen.CursorVisible != visible)
            {
                screen.CursorVisible = visible;


                //Console.CursorVisible = visible;

                if (visible)
                    Console.Write(VT_CSI + "?25h");
                else
                    Console.Write(VT_CSI + "?25l");
            }
        }

    }
}
