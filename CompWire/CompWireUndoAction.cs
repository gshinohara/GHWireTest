using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.Kernel.Undo;

namespace CompWire
{
	class CompWireUndoAction : GH_UndoAction
	{
		private CompWireComponent comp;
		private HashSet<Guid> ConnectedGuids;
		private HashSet<Guid> _ConnectedGuids;
		public CompWireUndoAction(CompWireComponent comp)
		{
			this.comp = comp;
			this.ConnectedGuids = new HashSet<Guid>(comp.Get_ConnectedGuids());
		}
		protected override void Internal_Undo(GH_Document doc)
		{
			_ConnectedGuids = new HashSet<Guid>(comp.Get_ConnectedGuids());
			comp.Replace_ConnectedGuids(ConnectedGuids);
		}

		protected override void Internal_Redo(GH_Document doc)
		{
			comp.Replace_ConnectedGuids(_ConnectedGuids);
		}
	}
}
