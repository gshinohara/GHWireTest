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
    public class CompWireAttributes : GH_ComponentAttributes
    {
        public CompWireAttributes(CompWireComponent owner) : base(owner) { }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            canvas.MouseMove -= this.Canvas_MouseMove;
            canvas.KeyDown -= this.Canvas_KeyDown;
            canvas.KeyUp -= this.Canvas_KeyUp;
            canvas.MouseDown -= this.Canvas_MouseDown;
            canvas.MouseUp -= this.Canvas_MouseUp;
            canvas.Document_ObjectsDeleted -= this.Document_ObjectsDeleted;

            canvas.MouseMove += this.Canvas_MouseMove;
            canvas.KeyDown += this.Canvas_KeyDown;
            canvas.KeyUp += this.Canvas_KeyUp;
            canvas.MouseDown += this.Canvas_MouseDown;
            canvas.MouseUp += this.Canvas_MouseUp;
            canvas.Document_ObjectsDeleted += this.Document_ObjectsDeleted;

            if (this.ConnectedAttributes != null)
            {
                foreach (IGH_Attributes att in this.ConnectedAttributes)
                    graphics.DrawLine(new Pen(Color.Blue), att.Pivot, this.Grip);
            }

            base.Render(canvas, graphics, channel);
        }
        protected virtual void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!(sender is GH_Canvas))
                return;

            GH_Canvas canvas = (GH_Canvas)sender;

            PointF cursor = new GH_CanvasMouseEvent(canvas.Viewport, e).CanvasLocation;
            double distance = Math.Sqrt(Math.Pow(this.Grip.X - cursor.X, 2) + Math.Pow(this.Grip.Y - cursor.Y, 2));

            if (distance < 10)
            {
                if (this.CursorName == null)
                    this.CursorName = "GH_NewWire";
                Instances.CursorServer.AttachCursor(canvas, this.CursorName);
                this.CursorName_OnAddTag = this.CursorName;
            }
            else
            {
                this.CursorName = null;
            }

            if (this.IsGripWire)
            {
                Graphics graphics = canvas.GetGraphicsObject(true);
                graphics.DrawLine(new Pen(Color.Red), cursor, this.Grip);
            }
        }
        protected virtual void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is GH_Canvas))
                return;

            GH_Canvas canvas = (GH_Canvas)sender;

            if (this.CursorName == null)
                return;

            if (e.KeyCode == Keys.ShiftKey)
                this.CursorName = "GH_AddWire";
            else if(e.KeyCode == Keys.ControlKey)
                this.CursorName = "GH_RemoveWire";
            
            Instances.CursorServer.AttachCursor(canvas, this.CursorName);
        }
        protected virtual void Canvas_KeyUp(object sender, KeyEventArgs e)
        {
            this.CursorName = "GH_NewWire";
        }
        protected virtual void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (!(sender is GH_Canvas && e.Button == MouseButtons.Left && this.CursorName != null))
                return;

            GH_Canvas canvas = (GH_Canvas)sender;
            Graphics graphics = canvas.GetGraphicsObject(true);

            PointF cursor = new GH_CanvasMouseEvent(canvas.Viewport, e).CanvasLocation;
            if(e.Button == MouseButtons.Left)
                this.IsGripWire = true;
        }
        protected virtual void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (!(sender is GH_Canvas))
                return;

            GH_Canvas canvas = (GH_Canvas)sender;
            Graphics graphics = canvas.GetGraphicsObject(true);

            PointF cursor = new GH_CanvasMouseEvent(canvas.Viewport, e).CanvasLocation;
            IGH_Attributes targetAttribute = canvas.Document.FindAttribute(cursor, false);
            if (targetAttribute != null && this.IsGripWire)
            {
                switch (this.CursorName_OnAddTag)
                {
                    case "GH_NewWire":
                        this.ConnectedAttributes = new List<IGH_Attributes> { targetAttribute };
                        break;
                    case "GH_AddWire":
                        this.ConnectedAttributes.Add(targetAttribute);
                        break;
                    case "GH_RemoveWire":
                        this.ConnectedAttributes.Remove(targetAttribute);
                        break;
                }
            }

            if (this.ConnectedAttributes != null)
            {
                foreach (IGH_Attributes att in this.ConnectedAttributes)
                    graphics.DrawLine(new Pen(Color.Blue), att.Pivot, this.Grip);
            }

            this.IsGripWire = false;
        }
        protected virtual void Document_ObjectsDeleted(GH_Document sender, GH_DocObjectEventArgs e)
        {
            foreach (IGH_Attributes att in e.Attributes)
            {
                //Source(本体)が削除されたとき
                if (this.ConnectedAttributes != null)
                    this.ConnectedAttributes.Remove(att);

                //Target(ワイヤーの先)が削除されたとき
                if (att is CompWireAttributes)
                    ((CompWireAttributes)att).ConnectedAttributes = null;
            }
        }
        private string CursorName { get; set; }
        private string CursorName_OnAddTag { get; set; }//カーソルが離れたらCursorNameがnullになってワイヤーをつなげられないのを回避
        private bool IsGripWire { get; set; }
        private PointF Grip => new PointF((this.Bounds.Left + this.Bounds.Right) / 2, this.Bounds.Bottom);
        public List<IGH_Attributes> ConnectedAttributes { get; set; }
    }
}
