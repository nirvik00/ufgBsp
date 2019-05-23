using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace ProjVan1
{
    public class ProjVan1Info : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "ProjVan1";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("5e4cb935-aa93-4ac4-882f-e4553ccf1d15");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
