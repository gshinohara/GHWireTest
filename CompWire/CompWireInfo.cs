using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace CompWire
{
    public class CompWireInfo : GH_AssemblyInfo
    {
        public override string Name => "CompWire";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("A68D7ED2-22B2-484F-A518-D9CD1326EA3D");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}