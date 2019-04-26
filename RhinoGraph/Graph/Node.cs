using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSharpx.Collections.Experimental;
using Rhino.Geometry;

namespace RhinoGraph.Graph
{
  public class Node
  {
    public Node(Point3d pt)
    {
      this.Point = pt;
    }

    public Point3d Point { get; }
  }
}
