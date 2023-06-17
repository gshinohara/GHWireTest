using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;

namespace CompWire
{
    public class CompWireAttributes : GH_ComponentAttributes
    {
        public PointF Grip { 
            get { return new PointF((this.Bounds.Left + this.Bounds.Right) / 2, this.Bounds.Bottom); }
            private set { }
        }
		private RectangleF GripBounds => new RectangleF(Grip.X - 5, Grip.Y - 5, 10, 10);
		public override bool HasInputGrip => true;
		public override PointF InputGrip => new PointF(0,0);
		public CompWireComponent _owner { get; private set; }

        public override void AppendToAttributeTree(List<IGH_Attributes> attributes)
		{
            base.AppendToAttributeTree(attributes);
		}

		public CompWireAttributes(CompWireComponent owner) : base(owner) 
        {
            _owner = owner;
        }

		public override bool IsPickRegion(PointF point)
		{
            bool isRegion = base.IsPickRegion(point) || Grasshopper.GUI.GH_GraphicsUtil.IsPointInEllipse(GripBounds, point);
            return isRegion;
		}

		protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {

            switch (channel)
            {
                case GH_CanvasChannel.Wires:
                    GH_CapsuleRenderEngine.RenderOutputGrip(graphics, canvas.Viewport.Zoom, Grip, true);
                    HashSet<Guid> ConnectedGuids = _owner.Get_ConnectedGuids();
                    if (ConnectedGuids != null)
                    {
                        foreach (Guid guid in ConnectedGuids)
                        {
                            IGH_DocumentObject docObj = _owner.OnPingDocument().FindObject(guid, false);
                            CompWireUtil.DrawCustomeWire(graphics, Grip, docObj.Attributes.Bounds);
                        }
                    }
                    break;
            }

            base.Render(canvas, graphics, channel);            
        }
        
		public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
		{

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                bool onGrip = Grasshopper.GUI.GH_GraphicsUtil.IsPointInEllipse(GripBounds, e.CanvasLocation);
                
                if (onGrip)
                {                   
                    //change cursor
                    Instances.CursorServer.AttachCursor(sender, "GH_NewWire");

                    //set interaction
                    sender.ActiveInteraction = new CompWireInteraction(sender, e, this);
                    
                    return GH_ObjectResponse.Handled;
                }
            }          
            return base.RespondToMouseDown(sender, e);
		}

        private string cursorName = "GH_NewWire";
        
        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
            bool onGrip = Grasshopper.GUI.GH_GraphicsUtil.IsPointInEllipse(GripBounds, e.CanvasLocation);
            if (onGrip)
            {
                Instances.CursorServer.AttachCursor(sender, cursorName);

                return GH_ObjectResponse.Handled;
            }

            return base.RespondToMouseMove(sender, e);           
		}

	}
}
