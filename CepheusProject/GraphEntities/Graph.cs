using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTestCepheusAlgorithms")]
namespace Cepheus
{
	abstract class Graph { }
	class Graph<TVertex> : Graph where TVertex : VertexBase<TVertex>
	{
		public Graph()
		{
			Edges = new Dictionary<string, Edge<TVertex>>();
			Vertices = new Dictionary<string, TVertex>();
		}
		public Dictionary<string, Edge<TVertex>> Edges { get; private set; }
		public Dictionary<string, TVertex> Vertices { get; private set; }
		public void AddVertex(TVertex vertex)
		{
			Vertices.Add(vertex.Name,vertex);
		}

		public void AddEdge(string name, TVertex from, TVertex to)
		{
			Edge<TVertex> edge = new Edge<TVertex>();
			edge.Name = name;
			edge.From = from;
			edge.To = to;
			from.OutEdges.Add(edge);
			to.InEdges.Add(edge);
			Edges.Add(from.Name+to.Name, edge);
		}
		public void AddEdgeWithLength(string name, TVertex from, TVertex to, int length)
		{
			EdgeWithLength<TVertex> edge = new EdgeWithLength<TVertex>();
			edge.Name = name;
			edge.From = from;
			edge.To = to;
			edge.Length = length;
			from.OutEdges.Add(edge);
			to.InEdges.Add(edge);
			Edges.Add(from.Name + to.Name, edge);
		}

		public void InitializeVertices()
		{
			foreach (TVertex vertex in Vertices.Values)
			{
				vertex.Initialize();
			}

		}
		public TVertex GetVertex(string name)
		{
			if (Vertices.ContainsKey(name))
				return Vertices[name];
			else
				return null;
		}

		public Edge<TVertex> GetEdge(string fromNameToName)
		{
			if (Edges.ContainsKey(fromNameToName))
				return Edges[fromNameToName];
			else
				return null;
		}

		public Edge<TVertex> GetEdge(TVertex from, TVertex to)
		{
			string name = from.Name + to.Name;
			if (Edges.ContainsKey(name))
				return Edges[name];
			else
				return null;
		}
		public SortedList<int,EdgeWithLength<TVertex>> GetEdgesSortedByLength()
		{
			var edges = new SortedList<int, EdgeWithLength<TVertex>>();
			foreach (EdgeWithLength<TVertex> edge in Edges.Values)
				edges.Add(edge.Length, edge);
			return edges;
		}
	}
	class FlowNetwork<TVertex> :Graph<TVertex> where TVertex : VertexBase<TVertex> //TODO inheritance with special type of Edge //TODO there was an implementation with BfsVertex, is that good?
	{
		public new Dictionary<string,FlowEdge<TVertex>> Edges { get; private set; }
		public new Dictionary<string,TVertex> Vertices { get; private set; }
		public FlowNetwork(TVertex source, TVertex sink)
		{
			Source = source;
			Sink = sink;
			Edges = new Dictionary<string, FlowEdge<TVertex>>();
			Vertices = new Dictionary<string, TVertex>();
		}
		public TVertex Source { get; }
		public TVertex Sink { get; }
		public void AugmentFlow(List<FlowEdge<TVertex>> path, int minDifference) 
		{
			foreach (var edge in path)
				edge.Flow += minDifference;
		}
		public int GetMinDifference(List<FlowEdge<TVertex>> path) 
		{
			int minDif = path[0].Capacity - path[0].Flow; //some first value
			foreach (var edge in path)
				if (minDif > (edge.Capacity - edge.Flow))
					minDif = edge.Capacity - edge.Flow;
			return minDif;
		}
		/// <summary>
		/// Set initial flow to zero for each edge.
		/// </summary>
		public void InitializeEdges()
		{
			foreach (var edge in Edges.Values)
				edge.Flow = 0;
		}
		public int GetMaximalFlow()
		{
			int flow = 0;
			foreach (FlowEdge<TVertex> edge in Sink.InEdges)
			{
				flow += edge.Flow;
			}
			return flow;
		}
	}
}
