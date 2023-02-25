using System;
using System.Collections.Generic;

namespace tw.curses.Widgets
{
	public interface IWidget
	{
        IWidget         Parent { get; set; }
        List<IWidget>   Children { get; }

        bool IsDirty { get; set; }

        int ForegroundColor { get; set; }
        int BackgroundColor { get; set; }
        int X { get;  set; }
        int Y { get;  set; }
        int Width { get;  set; }
        int Height { get;  set; }

        void AddChild(IWidget child);
		void Draw(Curses curses);
	}
}

