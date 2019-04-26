using QuickGraph;
using RhinoGraph.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using QuickGraph.Algorithms;
using Rhino.Geometry;

namespace RhinoGraph.Graph
{
  public enum FindPathType
  {
    Dijkstra,
    AStar,
    Bellman,
    Dag
  }

  public class GeometryGraph
  {
    private AdjacencyGraph<Node, Edge> graph;
    private GeometryMapper map;

    public GeometryGraph(IEnumerable<Line> floors, double floorWeightMulti, IEnumerable<Line> walls, double wallWeightMulti, IEnumerable<Line> ceilings, double ceilingWeightMulti)
    {
      this.map = new GeometryMapper(floors, floorWeightMulti, walls, wallWeightMulti, ceilings, ceilingWeightMulti);
      this.graph = new AdjacencyGraph<Node, Edge>();
      this.graph.AddVerticesAndEdgeRange(map.Floors);
      this.graph.AddVerticesAndEdgeRange(map.Walls);
      this.graph.AddVerticesAndEdgeRange(map.Ceilings);
    }
    
    public Dictionary<Node, IEnumerable<Edge>> SolvePathFind(FindPathType type, Node rootNode, HashSet<Node> endNodes)
    {
      TryFunc<Node, IEnumerable<Edge>> paths;
      switch (type)
      {
        case FindPathType.Dijkstra:
          paths = this.graph.ShortestPathsDijkstra(e => e.Weight, rootNode);
          break;
        case FindPathType.AStar:
          paths = this.graph.ShortestPathsAStar(e => e.Weight, n => n.Point.DistanceTo(rootNode.Point), rootNode);
          break;
        case FindPathType.Bellman:
          paths = this.graph.ShortestPathsBellmanFord(e => e.Weight, rootNode);
          break;
        default:
          return new Dictionary<Node, IEnumerable<Edge>>();
      }

      var result = new Dictionary<Node, IEnumerable<Edge>>();
      foreach (var target in endNodes)
      {
        if (paths(target, out var path))
        {
          result.Add(target, path);
        }
      }

      return result;
    }

    public bool TryGetNode(Point3d pt, out Node node)
    {
      return map.TryGetNode(pt, out node);
    }
  }
}
