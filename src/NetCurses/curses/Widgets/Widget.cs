using System;
using System.Collections.Generic;

namespace tw.curses.Widgets
{
    abstract public class Widget : IWidget
	{
        public List<IWidget> Children { get; } = new List<IWidget>();
        public IWidget Parent { get; set; }
        public bool IsDirty { get; set; }
        public int ForegroundColor { get; set; }
        public int BackgroundColor { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Widget()
		{

	    }

        public void AddChild(IWidget child)
		{
            child.Parent = this;
            Children.Add(child);
		}

        abstract public void Draw(Curses curses);
    }
}

