using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace tw.curses
{

    // Hide/Show console window....
    //  https://stackoverflow.com/questions/472282/show-console-in-windows-application/15079092#15079092
    //
    public class Utils
    {
        public static bool IsUnix => Environment.OSVersion.Platform == PlatformID.Unix;
        public static bool IsWindows => Environment.OSVersion.Platform == PlatformID.Win32NT;



        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_CLOSE = 0xF060;

        private const uint ENABLE_QUICK_EDIT = 0x0040;

        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;


        private const int STD_OUTPUT_HANDLE = -11;
        private const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("user32.dll")]
        private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();


        /// <summary>
        /// Disable the close button of the console window.
        /// </summary>
        internal static void DeleteCloseBox()
        {
            try
            {
               DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);
             }
            catch (Exception e)
            {
                Console.WriteLine($"\r\nWARNING: The close button of the console window can't be disabled.\r\n --> Error: {e.InnerException?.Message ?? e.Message}\r\n");
            }
        }


        internal static void DisableQuickEdit()
        {

            IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);

            // get current console mode
            if (!GetConsoleMode(inHandle, out uint consoleMode))
                throw new Exception("Can't get ConsoleMode()");

            // Clear the quick edit bit in the mode flags
            consoleMode &= ~ENABLE_QUICK_EDIT;

            // set the new mode
            if (!SetConsoleMode(inHandle, consoleMode))
                throw new Exception("Can't set ConsoleMode()");

            Console.TreatControlCAsInput = false;
        }

        internal static void SetTerminalEmulation()
        {
            IntPtr outHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (!GetConsoleMode(outHandle, out uint consoleMode))
                throw new Exception("Can't get ConsoleMode()");

            consoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            if (!SetConsoleMode(outHandle, consoleMode))
                throw new Exception("Can't set ConsoleMode()");
        }

        internal static void SetupUnixTerminal() {
            Process p = new Process();
            p.StartInfo.FileName = "/bin/stty";
            p.StartInfo.Arguments = "raw";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.Start();
            p.WaitForExit();
            //System.Diagnostics.Process.Start("/bin/stty", "raw");
        }
        internal static void CleanupUnixTerminal() {
            Process p = new Process();
            p.StartInfo.FileName = "/bin/stty";
            p.StartInfo.Arguments = "sane";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.Start();
            p.WaitForExit();
            //System.Diagnostics.Process.Start("/bin/stty", "sane");
        }
    }
}
