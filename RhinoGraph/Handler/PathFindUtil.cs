using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using QuickGraph;
using QuickGraph.Algorithms;
using Rhino.Geometry;
using RhinoGraph.Graph;

namespace RhinoGraph.Handler
{
  public class PathFindUtil
  {
    
    public static bool TryGetElectricNodes(IGH_DataAccess DA, Graph.GeometryGraph graph, Grasshopper.Kernel.Types.GH_Point elecSource, List<Grasshopper.Kernel.Types.GH_Point> elecTargets, out Node elecSourceNode, out HashSet<Node> elecTargetNodes)
    {
      elecTargetNodes = new HashSet<Node>();
      if (!graph.TryGetNode(elecSource.Value, out elecSourceNode))
      {
        return false;
      }
      
      foreach (var tar in elecTargets)
      {
        if (!graph.TryGetNode(tar.Value, out var elecTargetNode))
        {
          return false;
        }
        else
        {
          elecTargetNodes.Add(elecTargetNode);
        }
      }

      return true;
    }

    public static DataTree<Line> BuildTree(Dictionary<Node, IEnumerable<Edge>> edgePaths)
    {
      var tree = new DataTree<Line>();
      var i = 0;
      foreach (var path in edgePaths)
      {
        var ghpath = new GH_Path(i);
        foreach (var edge in path.Value)
        {
          tree.Add(new Line(edge.Source.Point, edge.Target.Point), ghpath);
        }

        i++;
      }

      return tree;
    }
  }
}
