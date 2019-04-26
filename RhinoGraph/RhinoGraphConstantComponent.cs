using System;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using QuickGraph;
using Rhino.Geometry;
using RhinoGraph.Graph;
using RhinoGraph.Handler;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace RhinoGraph
{
  public class RhinoGraphConstantComponent : GH_Component
  {
    /// <summary>
    /// Each implementation of GH_Component must provide a public 
    /// constructor without any arguments.
    /// Category represents the Tab in which the component will appear, 
    /// Subcategory the panel. If you use non-existing tab or panel names, 
    /// new tabs/panels will automatically be created.
    /// </summary>
    public RhinoGraphConstantComponent()
      : base("RhinoGraphConstant", "Constant",
          "Description",
          "Category", "Subcategory")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddLineParameter("Floor Edges", "F", "Edges for the Floors", GH_ParamAccess.list);
      pManager.AddLineParameter("Wall Edges", "W", "Edges for the Walls", GH_ParamAccess.list);
      pManager.AddLineParameter("Ceiling Edges", "C", "Edges for the Ceilings", GH_ParamAccess.list);
      pManager.AddPointParameter("Electric Devices", "D", "Electrical Devices", GH_ParamAccess.list);
      pManager.AddPointParameter("Electric Source", "S", "Electrical Devices", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Algo Type", "T", "Source of Electricity", GH_ParamAccess.item);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddLineParameter("Path Edges", "P", "Shortest Paths", GH_ParamAccess.tree);
      pManager.AddTextParameter("Message", "M", "Message", GH_ParamAccess.item);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
    /// to store data in output parameters.</param>
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      try
      {
        var floors = new List<Line>();
        var walls = new List<Line>();
        var ceilings = new List<Line>();

        var elecTargets = new List<Grasshopper.Kernel.Types.GH_Point>();
        var elecSource = new Grasshopper.Kernel.Types.GH_Point();
        var type = 0;

        if (!DA.GetDataList(0, floors))
        {
          return;
        }

        if (!DA.GetDataList(1, walls))
        {
          return;
        }

        if (!DA.GetDataList(2, ceilings))
        {
          return;
        }

        if (!DA.GetDataList(3, elecTargets))
        {
          return;
        }

        if (!DA.GetData(4, ref elecSource))
        {
          return;
        }

        if (!DA.GetData(5, ref type))
        {
          return;
        }

        //build the graph along with a geometry -> edge/node map
        var graph = new GeometryGraph(floors, 1.0, walls, 1.0, ceilings, 1.0);

        //get the corresponding node for the electric source
        if (!PathFindUtil.TryGetElectricNodes(DA, graph, elecSource, elecTargets, out var elecSourceNode, out var elecTargetNodes))
        {
          DA.SetDataList(0, null);
          DA.SetData(1, "Failed to find source on graph");
        }

        //find the shortest pats
        var fpt = FindPathType.Dijkstra;
        switch (type)
        {
          case 1:
            fpt = FindPathType.Bellman;
            break;
          case 2:
            fpt = FindPathType.AStar;
            break;
          default:
            fpt = FindPathType.Dijkstra;
            break;
        }
        var res = graph.SolvePathFind(fpt, elecSourceNode, elecTargetNodes);

        //build the data tree with all of the paths
        var tree = PathFindUtil.BuildTree(res);

        DA.SetDataTree(0, tree);
        DA.SetData(1, "Success!");
      }
      catch (Exception x)
      {
        DA.SetDataList(0, null);
        DA.SetData(1, $"M: {x.Message} - S: {x.StackTrace}");
      }
    }

    /// <summary>
    /// Provides an Icon for every component that will be visible in the User Interface.
    /// Icons need to be 24x24 pixels.
    /// </summary>
    protected override System.Drawing.Bitmap Icon {
      get {
        // You can add image files to your project resources and access them like this:
        //return Resources.IconForThisComponent;
        return null;
      }
    }

    /// <summary>
    /// Each component must have a unique Guid to identify it. 
    /// It is vital this Guid doesn't change otherwise old ghx files 
    /// that use the old ID will partially fail during loading.
    /// </summary>
    public override Guid ComponentGuid {
      get { return new Guid("c81bb838-0d9c-42c8-b3f4-f69077963030"); }
    }
  }
}
