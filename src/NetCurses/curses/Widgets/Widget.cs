using System;
using System.Collections.Generic;

namespace tw.curses.Widgets
{
    abstract public class Widget : IWidget
	{
        public List<IWidget> Children { get; } = new List<IWidget>();

        IWidget IWidget.Parent { get; set; }
        bool IWidget.IsDirty { get; set; }
        int IWidget.X { get; set; }
        int IWidget.Y { get; set; }
        int IWidget.Width { get; set; }
        int IWidget.Height { get; set; }

        public Widget()
		{

	    }

        public void AddChild(IWidget child)
		{
            child.Parent = this;
            Children.Add(child);
		}

        abstract public void Draw();
    }
}

