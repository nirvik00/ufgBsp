using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace ProjVan1
{
    public class ProjVan1Component : GH_Component
    {
        public ProjVan1Component()
          : base("component-0", "c0",
              "Generate site extrusion based on setbacks from various street- types",
              "ProjVan", "parcels")
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // 0.
            pManager.AddTextParameter("Input-Streets", "inp.A.0", "Input for streets: Layer names (CAPITALIZED) separated by comma \nPreferred Input Type: write text in PANEL", GH_ParamAccess.item);
            // 1.
            pManager.AddTextParameter("Input-Setbacks", "inp.A.1", "Corresponding Input for setbacks: Enter numbers separated by comms\nPreferred Input Type: write text in PANEL", GH_ParamAccess.item);
            // 2.
            pManager.AddCurveParameter("Input-Site-Boundaries", "inp.B.0", "Select all site boundaries: closed planar curves (polylines, nurbs, etc)\nPreferred Input Type: CRV & select multiple curves", GH_ParamAccess.list);
            // 3. 
            pManager.AddNumberParameter("Input-FSR", "inp.C.0", "Default Value=2.5\nEnter a number for FSR calculations\nPreferred Input Type: NUMERIC SLIDER", GH_ParamAccess.item);
            // 4.
           // pManager.AddNumberParameter("Input-Max-Height", "inp.C.1", "Default Value=10.0\nEnter the maximum height\nPreferred Input Type: NUMERIC SLIDER", GH_ParamAccess.item);
            // 5.
           // pManager.AddNumberParameter("Input-Min-Height", "inp.C.2", "Default Value=0.0\nEnter the minimum height\nPreferred Input Type: NUMERIC SLIDER", GH_ParamAccess.item);
            // 6.
           // pManager.AddNumberParameter("Input-Magnitude-Rays", "inp.D.1", "Default Value= 100, type: double ", GH_ParamAccess.item);

            // intersection inputs
            // 6.
            // pManager.AddIntegerParameter("Input-Number-Rays", "inp.D.0", "Default Value= 8, type: integer", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // 0.
            // pManager.AddTextParameter("Output-Street-Names", "out.A.0.0", "street layer names", GH_ParamAccess.list);
            // 1.
            // pManager.AddTextParameter("Output-Setback-Numbers", "out.A.0.1", "setback numbers", GH_ParamAccess.list);
            // 2.
            // pManager.AddCurveParameter("Output-Street-Curve", "out.A.2", "List of street curves from layer names for streets", GH_ParamAccess.list);
            // 3.
            // pManager.AddTextParameter("Output-Proc-Object", "out.A.3", "Process object ready for geometric processing", GH_ParamAccess.list);
            // 4.
            // pManager.AddLineParameter("Intx-Rays", "Intx-Rays", "Intx-Rays", GH_ParamAccess.list);
            // 5.
            // pManager.AddPointParameter("intx", "intx", "intx", GH_ParamAccess.list);
            // 6.
            // pManager.AddCurveParameter("site", "site", "site", GH_ParamAccess.list);
            // 7.
            pManager.AddGeometryParameter("Output Solids", "output A.0.0", "outputs solid from the process: brep, surface, mesh ", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string streetLayerNames = "";
            string setbacks = "";

            DA.GetData(0, ref streetLayerNames); // GET THE STRING : LAYER NAMES
            DA.GetData(1, ref setbacks); // GET THE STRING : SETBACK DISTANCES

            InputProc inputproc = new InputProc(streetLayerNames, setbacks);

            List<string> STREET_LAYER_NAMES = inputproc.GetStreetNames();
            List<double> SETBACKS = inputproc.GetSetbacks();
            // DA.SetDataList(0, STREET_LAYER_NAMES); // SET THE STRING : LAYER NAMES
            // DA.SetDataList(1, SETBACKS); // SET THE DOUBLE - SETBACKS

            inputproc.GetLayerGeom();
            List<LineCurve> STREETLINES = inputproc.GetLinesFromLayers();
            // DA.SetDataList(2, STREETLINES); // SET THE STREET LINE : CURVES

            List<ProcObj> PROCOBJLI = inputproc.GetProcObjs();
            List<string> PROCOBJLIStr = inputproc.GetProcObjLiString();
            // DA.SetDataList(3, PROCOBJLIStr); // SET THE PROC-OBJ-LI-STRING : STRING 

            List<Curve> SITES = new List<Curve>();
            double FSR = 2.5;
            double MAXHT = 10.0;
            double MINHT = 0.0;
            DA.GetDataList(2, SITES);
            DA.GetData(3, ref FSR);
           // DA.GetData(4, ref MAXHT);
           // DA.GetData(5, ref MINHT);

            int NUMRAYS = 4; // DA.GetData(6, ref NUMRAYS);

            double MAGNITUDERAYS = 2.0;
           // DA.GetData(6, ref MAGNITUDERAYS); // DA.GetData(7, ref MAGNITUDERAYS);

            ProcessIntx processintx = new ProcessIntx(
                PROCOBJLI,
                SITES,
                FSR, MINHT, MAXHT,
                NUMRAYS, MAGNITUDERAYS);

            processintx.GenRays();

            List<SiteObj> SITEOBJ = processintx.GetSiteObjList();
            List<Curve> crvSite = new List<Curve>();
            List<Point3d> intxpts = new List<Point3d>();
            List<Line> rays = new List<Line>();
            rays = processintx.GetRays();
            List<Extrusion> solids = new List<Extrusion>();
            for (int i = 0; i < SITEOBJ.Count; i++)
            {
                intxpts.Add(SITEOBJ[i].GetIntxPt());
                crvSite.Add(SITEOBJ[i].GetSite());
                solids.Add(SITEOBJ[i].GetOffsetExtrusion());
            }

            // DA.SetDataList(4, rays);
            // DA.SetDataList(5, intxpts);
            // DA.SetDataList(6, crvSite);
            DA.SetDataList(0, solids);

        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("34fad908-f1a6-4035-9f2f-8c2e48d35a14"); }
        }
    }
}
