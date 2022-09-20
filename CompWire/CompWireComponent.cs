using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Undo;
using GH_IO.Serialization;

namespace CompWire
{
    public class CompWireComponent : GH_Component
    {
		#region General
		public CompWireComponent(): base("CompWire", "Nickname","Description","Category", "Subcategory")
        {
            
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("X1", "X1", "Message Text", GH_ParamAccess.item);
            pManager.AddTextParameter("X2", "X2", "", GH_ParamAccess.item);
            pManager.AddTextParameter("X3", "X3", "", GH_ParamAccess.item);

            foreach (IGH_Param p in this.Params.Input)
                p.Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Y1", "Y1", "", GH_ParamAccess.list);
            pManager.AddTextParameter("Y2", "Y2", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var attList = ConnectedGuids;
            if (attList != null)
                DA.SetDataList("Y1", attList.Select(k => k.ToString()));
        }

        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("795BF394-B751-4551-9B97-1B255FC882B2");
        public override TimeSpan ProcessorTime => TimeSpan.Zero;

		#endregion

		#region Attributes
		public override void CreateAttributes()
        {
            CompWireAttributes att = new CompWireAttributes(this);
            this.Attributes = att;
        }
		#endregion

		#region ConnectedGuids
		private HashSet<Guid> ConnectedGuids = new HashSet<Guid>();
       
        public void Add_ConnectedGuids(Guid guid, bool doRecalc = true)
        {
            ConnectedGuids.Add(guid);
            if(doRecalc) Recalculate();
        }

        public void Remove_ConnectedGuids(Guid guid, bool doRecalc = true)
        {
            ConnectedGuids.Remove(guid);
            if (doRecalc) Recalculate();
        }

        public void Replace_ConnectedGuids(HashSet<Guid> guids)
		{
            ConnectedGuids = guids;
            Recalculate();
		}

        public HashSet<Guid> Get_ConnectedGuids()
        {
            return ConnectedGuids;
        }
		#endregion

		#region DocEvents
		public override void AddedToDocument(GH_Document document)
        {
            //set events
            document.ObjectsDeleted += DocumentObjectsDeleted;

            base.AddedToDocument(document);
        }

		public override void RemovedFromDocument(GH_Document document)
		{
            //clear events
            document.ObjectsDeleted -= DocumentObjectsDeleted; 

            //clear connected attributes
            if (this.Attributes is CompWireAttributes att)
            {
                ConnectedGuids.Clear();
            }                    

            base.RemovedFromDocument(document);
		}

        private void DocumentObjectsDeleted(object sender, GH_DocObjectEventArgs e)
        {
            bool recalc = false;

            GH_UndoServer undoServer = e.Document.UndoServer;
            MethodInfo methodInfo = typeof(GH_UndoServer).GetRuntimeMethods().First(m => m.Name.Contains("get_UndoRecords"));
            List<GH_UndoRecord> records = methodInfo.Invoke(undoServer, new object[] { }) as List<GH_UndoRecord>;
            GH_UndoRecord record_Delete = records[0];
            undoServer.RemoveRecord(record_Delete.Guid);

            e.Document.UndoUtil.RecordEvent(new GH_UndoRecord("CustomWireAction", new CompWireUndoAction(this)));
            e.Document.UndoUtil.RecordEvent(record_Delete);
            undoServer.MergeRecords(2);

            foreach (IGH_DocumentObject docObj in e.Objects)
            {
                Guid guid = docObj.InstanceGuid;
                if (ConnectedGuids.Contains(guid))
                {
                    Remove_ConnectedGuids(guid, false);
                    recalc = true;
                }
            }
            
            if (recalc) Recalculate();
        }

        public void Recalculate()
        {
            this.ExpireSolution(true);
        }
		
		public override void MovedBetweenDocuments(GH_Document oldDocument, GH_Document newDocument)
		{
            //AddedToDocumentの引数(document)がコピー時では、空のドキュメントが渡されるため、それの回避策としてこれを使う。
            //詳細→https://www.grasshopper3d.com/forum/topics/gh-document-enabledchanged-event
            oldDocument.ObjectsDeleted -= DocumentObjectsDeleted;
            newDocument.ObjectsDeleted += DocumentObjectsDeleted;
            base.MovedBetweenDocuments(oldDocument, newDocument);
		}
        #endregion

        #region Serialize
        public override bool Write(GH_IWriter writer)
		{
            int idCount = ConnectedGuids.Count;
            writer.SetInt32("ID_Count", idCount);

            int i = 0;
            foreach (Guid guid in ConnectedGuids)
            {
                writer.SetGuid("ID", i++, guid);
            }  

			return base.Write(writer);
		}

		public override bool Read(GH_IReader reader)
		{
            int idCount = reader.GetInt32("ID_Count");
            for (int i = 0; i < idCount; ++i)
            {
                Guid guid = reader.GetGuid("ID", i);
                ConnectedGuids.Add(guid);
            }
			return base.Read(reader);
		}
		#endregion
	}
}