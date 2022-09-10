using System;
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;

namespace CompWire
{
    public class CompWireAttributes : GH_ComponentAttributes
    {
        public CompWireAttributes(CompWireComponent owner) : base(owner) { }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            GH_CapsuleRenderEngine.RenderInputGrip(graphics, canvas.Viewport.Zoom, this.InputGrip, true);
            base.Render(canvas, graphics, channel);
            //graphics.DrawLine(new Pen(Color.Red), new Point(0, 0), this.InputGrip);

            this.MouseEvent(canvas, out string name);
            Instances.CursorServer.AttachCursor(canvas, name);
        }
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            base.RespondToMouseDown(sender, e);
            this.MouseEvent(sender, out string name);
            Instances.CursorServer.AttachCursor(sender, name);
            return GH_ObjectResponse.Handled;
        }
        public void MouseEvent(GH_Canvas canvas,out string name)
        {
            Point cursorInClient = canvas.PointToClient(Cursor.Position);
            PointF cursorInCanvas = cursorInClient - (Size)canvas.Viewport.Target;
            PointF cursorInZoomCanvas = new PointF(cursorInCanvas.X / canvas.Viewport.Zoom, cursorInCanvas.Y / canvas.Viewport.Zoom);

            PointF subtraction = this.InputGrip - new SizeF(cursorInZoomCanvas.X, cursorInZoomCanvas.Y);
            if (Math.Sqrt(Math.Pow(subtraction.X, 2) + Math.Pow(subtraction.Y, 2)) < 10)
                name = "GH_Pencil";
            else
                name = "";
        }

        public override bool HasInputGrip => true;
        public override bool HasOutputGrip => true;
        public override PointF InputGrip => new PointF((this.Bounds.Left + this.Bounds.Right) / 2, this.Bounds.Bottom);
        
    }
}
