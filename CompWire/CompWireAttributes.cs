using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public CompWireAttributes(CompWireComponent owner) : base(owner)
        {
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            //GH_CapsuleRenderEngine.RenderInputGrip(graphics, canvas.Viewport.Zoom, this.InputGrip, true);
            base.Render(canvas, graphics, channel);
            //graphics.DrawLine(new Pen(Color.Red), new Point(0, 0), this.InputGrip);
            this.DrawWire(canvas);
        }
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            this.Pivot = new PointF((this.Bounds.Right + this.Bounds.Left) / 2, (this.Bounds.Top + this.Bounds.Bottom) / 2);
            GH_ObjectResponse response = base.RespondToMouseDown(sender, e);
            //this.DrawWire(sender);
            return response;
        }
        private void DrawWire(GH_Canvas canvas)
        {
            Wire wire = new Wire();
            CursorEventArgs cursorEventArgs = new CursorEventArgs { Canvas = canvas };

            PointF grip = new PointF((this.Bounds.Left + this.Bounds.Right) / 2, this.Bounds.Bottom);

            Point cursorInClient = canvas.PointToClient(Cursor.Position);
            PointF cursorInCanvas = cursorInClient - (Size)canvas.Viewport.Target;
            PointF cursorInZoomCanvas = new PointF(cursorInCanvas.X / canvas.Viewport.Zoom, cursorInCanvas.Y / canvas.Viewport.Zoom);
            PointF direction = grip - new SizeF(cursorInZoomCanvas.X, cursorInZoomCanvas.Y);

            if (Math.Sqrt(Math.Pow(direction.X, 2) + Math.Pow(direction.Y, 2)) < 10)
            {
                this.Pivot = new PointF(float.NaN, float.NaN);
                wire.CursorEvent += Wire.AttachCursor;
                cursorEventArgs.CursorName = "GH_NewWire";
                if (Control.MouseButtons == MouseButtons.Left)
                    switch (Control.ModifierKeys)
                    {
                        case Keys.Control:
                            cursorEventArgs.CursorName = "GH_RemoveWire";
                            break;
                        case Keys.Shift:
                            cursorEventArgs.CursorName = "GH_AddWire";
                            break;
                    }
            }
            else if (Control.MouseButtons == MouseButtons.Right)
            {
                cursorEventArgs.CursorName = "GH_Pencil";
                wire.CursorEvent += Wire.AttachCursor;
            }

            wire.OnCursorEvent(cursorEventArgs);
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override bool HasInputGrip => true;
        public override bool HasOutputGrip => true;
        //public override PointF InputGrip => new PointF((this.Bounds.Left + this.Bounds.Right) / 2, this.Bounds.Bottom);
        public override PointF InputGrip => base.InputGrip;
    }
}
