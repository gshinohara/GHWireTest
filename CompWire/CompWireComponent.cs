using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Types;

namespace CompWire
{
    public class CompWireComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public CompWireComponent()
          : base("CompWire", "Nickname",
            "Description",
            "Category", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("X1", "X1", "Message Text", GH_ParamAccess.item);
            pManager.AddTextParameter("X2", "X2", "", GH_ParamAccess.item);
            pManager.AddTextParameter("X3", "X3", "", GH_ParamAccess.item);

            foreach (IGH_Param p in this.Params.Input)
                p.Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Y1", "Y1", "", GH_ParamAccess.item);
            pManager.AddTextParameter("Y2", "Y2", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string x1 = default;
            DA.GetData("X1", ref x1);

            this.Message = x1;
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("795BF394-B751-4551-9B97-1B255FC882B2");
        public override void CreateAttributes()
        {
            this.Attributes = new CompWireAttributes(this);
        }
        public override TimeSpan ProcessorTime => TimeSpan.Zero;
    }
}