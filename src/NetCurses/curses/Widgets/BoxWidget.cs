using System;
namespace tw.curses.Widgets
{
	public class BoxWidget : Widget
	{
		public BoxWidget()
		{
		}

        public override void Draw(Curses curses)
        {
            CursesUtils.DrawGrafBox(curses, X, Y, Width, Height, ForegroundColor, BackgroundColor);
        }
    }
}

