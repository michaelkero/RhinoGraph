using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace RhinoGraph.Graph
{
  //The geometry mapper is responsible for mapping rhino geometry to graph datum
  public class GeometryMapper
  {

    public HashSet<Node> AllNodes { get; }
    public HashSet<Edge> Floors { get; }
    public HashSet<Edge> Walls { get; }
    public HashSet<Edge> Ceilings { get; }

    public GeometryMapper(IEnumerable<Line> floors, double floorWeightMulti, IEnumerable<Line> walls, double wallWeightMulti, IEnumerable<Line> ceilings, double ceilingWeightMulti)
    {
      this.AllNodes = new HashSet<Node>();
      this.Floors = new HashSet<Edge>();
      this.Walls = new HashSet<Edge>();
      this.Ceilings = new HashSet<Edge>();

      foreach (var floor in floors)
      {
        this.Floors.Add(CreateEdge(floor, EdgeType.Floor, floorWeightMulti));
      }
      foreach (var wall in walls)
      {
        this.Walls.Add(CreateEdge(wall, EdgeType.Wall, wallWeightMulti));
      }
      foreach (var ceiling in ceilings)
      {
        this.Ceilings.Add(CreateEdge(ceiling, EdgeType.Ceiling, ceilingWeightMulti));
      }
    }

    private Edge CreateEdge(Line l, EdgeType type, double weight)
    {
      if (!this.TryGetNode(l.From, out var src))
      {
        src = new Node(l.From);
        this.AllNodes.Add(src);
      }
      if (!this.TryGetNode(l.To, out var tar))
      {
        tar = new Node(l.To);
        this.AllNodes.Add(tar);
      }
      return new Edge(src, tar, type, weight);
    }

    public bool TryGetNode(Point3d pt, out Node node)
    {
      //an improvement to this would be to generate a 3D octree over the points
      //we would then use the octree to reduce the search space 
      //for now though, we loop over all the nodes
      foreach (var n in this.AllNodes)
      {
        if (pt.DistanceTo(n.Point) < 0.5)
        {
          node = n;
          return true;
        }
      }

      node = null;
      return false;
    }
  }
}
