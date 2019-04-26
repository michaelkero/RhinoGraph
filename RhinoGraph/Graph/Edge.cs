using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoGraph.Graph
{
  public class Edge : QuickGraph.IEdge<Node>
  {
    private EdgeType type;

    public Edge(Node source, Node target, EdgeType type, double weight)
    {
      this.Source = source;
      this.Target = target;
      this.type = type;
      this.Weight = weight;
    }

    public Node Source { get; }
    public Node Target { get; }
    public double Weight { get; }
  }

  public enum EdgeType
  {
    Wall,
    Floor,
    Ceiling
  }
}
