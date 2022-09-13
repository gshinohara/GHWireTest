using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;

namespace CompWire
{
    public class Wire
    {
        public event EventHandler<CursorEventArgs> CursorEvent;
        public void OnCursorEvent(CursorEventArgs e) => CursorEvent?.Invoke(this, e);
        public static void AttachCursor(object sender, CursorEventArgs e) => Instances.CursorServer.AttachCursor(e.Canvas, e.CursorName);
    }
    public class CursorEventArgs : EventArgs
    {
        public string CursorName;
        public GH_Canvas Canvas;
    }
}