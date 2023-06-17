using Rhino.Geometry;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Undo;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI.Canvas.Interaction;

namespace CompWire
{
	public class CompWireInteraction : GH_AbstractInteraction
	{
		
		private RectangleF _drawingBox;
		private string cursorName = "GH_NewWire";
		private CompWireAttributes compWireAttributes;
		
		public CompWireInteraction(GH_Canvas _canvas, GH_CanvasMouseEvent mouseEvent, CompWireAttributes compWireAttributes) : base(_canvas, mouseEvent)
		{
			//set drawing custom wire event 
			_canvas.CanvasPostPaintObjects += CanvasPostPaintObjects;

			_drawingBox = new RectangleF(mouseEvent.CanvasLocation, new SizeF(0, 0));

			this.compWireAttributes = compWireAttributes;
		}
		public override void Destroy()
		{
			//clear drawing custom wire event 
			this.Canvas.CanvasPostPaintObjects -= CanvasPostPaintObjects;
			
			base.Destroy();
		}

		void CanvasPostPaintObjects(GH_Canvas sender)
		{	
			//draw custom wire
			CompWireUtil.DrawCustomeWire(sender.Graphics, compWireAttributes.Grip, _drawingBox);
		}

		public override GH_ObjectResponse RespondToKeyDown(GH_Canvas sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ShiftKey)
			{
				cursorName = "GH_AddWire";
				Instances.CursorServer.AttachCursor(sender, cursorName);
				
			}
			else if (e.KeyCode == Keys.ControlKey)
			{
				cursorName = "GH_RemoveWire";
				Instances.CursorServer.AttachCursor(sender, cursorName);
			}
			
			return GH_ObjectResponse.Handled;
		}

		public override GH_ObjectResponse RespondToKeyUp(GH_Canvas sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey)
			{
				cursorName = "GH_NewWire";
				Instances.CursorServer.AttachCursor(sender, cursorName);
			}
			return GH_ObjectResponse.Handled;
		}


		public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			_drawingBox = new RectangleF(e.CanvasLocation, new SizeF(0, 0));

			GH_Document doc = sender.Document;
			if (doc != null)
			{
				IGH_Attributes att = doc.FindAttribute(e.CanvasLocation, true);
				if (att != null && att != compWireAttributes)
				{
					_drawingBox = att.Bounds;
				}
			}
			this.Canvas.Invalidate();
			return GH_ObjectResponse.Handled;
		}

		public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
		{
			GH_Document doc = sender.Document;
			if (doc != null)
			{
				IGH_Attributes att = doc.FindAttribute(e.CanvasLocation, true);		
				if (att != null && att != compWireAttributes)
				{
					Guid guid = att.InstanceGuid;

					//record UndoEvent
					this.Canvas.Document.UndoUtil.RecordEvent(new GH_UndoRecord("CustomWireAction", new CompWireUndoAction(compWireAttributes._owner)));

					switch (cursorName)
					{
						case "GH_NewWire":
							compWireAttributes._owner.Replace_ConnectedGuids(new HashSet<Guid> { guid });			
							break;
						case "GH_AddWire":
							compWireAttributes._owner.Add_ConnectedGuids(guid);
							
							break;
						case "GH_RemoveWire":
							compWireAttributes._owner.Remove_ConnectedGuids(guid);
							break;
					}
				}
			}
					
			return GH_ObjectResponse.Release;
		}



	}
}
