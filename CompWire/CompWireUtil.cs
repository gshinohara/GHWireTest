using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;


namespace CompWire
{
	class CompWireUtil
	{
		public static void DrawCustomeWire(Graphics graphics, PointF sourcePt, RectangleF target)
		{
			PointF cp = Grasshopper.GUI.GH_GraphicsUtil.BoxClosestPoint(sourcePt, target);
			Pen pen = new Pen(Color.Red);
			graphics.DrawLine(pen, cp, sourcePt);
			pen.Dispose();
		}
	}
}
