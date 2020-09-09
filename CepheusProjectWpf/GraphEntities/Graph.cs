using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CepheusProjectWpf;
using CepheusProjectWpf.DataStructures;
using CepheusProjectWpf.GraphShapes;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTestCepheusAlgorithms")]
namespace Cepheus
{
	public abstract class Graph { }
	public class Graph<TVertex> : Graph where TVertex : VertexBase<TVertex>, new()
	{
		public Graph()
		{
			Edges = new Dictionary<string, Edge<TVertex>>();
			Vertices = new Dictionary<int, TVertex>();
			UltimateEdges = new Dictionary<Edge<TVertex>, ArrowEdge>();
			UltimateVertices = new Dictionary<TVertex, EllipseVertex>();
		}
		public Dictionary<string, Edge<TVertex>> Edges { get; private set; }
		public Dictionary<int, TVertex> Vertices { get; private set; }
		public Dictionary<Edge<TVertex>, ArrowEdge> UltimateEdges { get; set; }
		public Dictionary<TVertex, EllipseVertex> UltimateVertices { get; set; }
		public TVertex AddVertex(int uniqueId, string name)
		{
			var vertex = new TVertex();
			vertex.UniqueId = uniqueId;
			vertex.Name = name;
			Vertices.Add(uniqueId, vertex);
			return vertex;
		}//TODO ther eshould be same implementation as in AddEdge, just create the vertex inside this method

		public void AddEdge(TVertex from, TVertex to, string name, int length)
		{
			Edge<TVertex> edge = new Edge<TVertex>();
			edge.Name = name;
			edge.From = from;
			edge.To = to;
			edge.Length = length;
			from.OutEdges.Add(edge);
			to.InEdges.Add(edge);
			Edges.Add(edge.Name, edge);
		}

		public void InitializeVertices()
		{
			foreach (TVertex vertex in Vertices.Values)
			{
				vertex.Initialize();
			}

		}
		public TVertex GetVertex(int id)
		{
			if (Vertices.ContainsKey(id))
				return Vertices[id];
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
			string name = from.UniqueId +"->" + to.UniqueId;
			if (Edges.ContainsKey(name))
				return Edges[name];
			else
				return null;
		}
		public BinaryHeap<int, Edge<TVertex>> GetEdgesSortedByLength()
		{
			var edges = new BinaryHeap<int, Edge<TVertex>>(Edges.Count);
			foreach (Edge<TVertex> edge in Edges.Values)
				edges.Insert(edge.Length, edge);
			return edges;
		}
		public void RemoveVertex(TVertex vertex)
		{
			List<FlowEdge<TVertex>> removable = new List<FlowEdge<TVertex>>();
			foreach (var edge in vertex.InEdges)
				removable.Add((FlowEdge<TVertex>)edge);
			foreach (var edge in vertex.OutEdges)
				removable.Add((FlowEdge<TVertex>)edge);
			foreach (var edge in removable)
				RemoveEdge(edge);
			removable.Clear();
			Vertices.Remove(vertex.UniqueId);
			UltimateVertices[vertex].Delete();
			UltimateVertices.Remove(vertex);
		}
		public void RemoveEdge(Edge<TVertex> edge)
		{
			Edges.Remove(edge.Name);
			edge.From.OutEdges.Remove(edge);
			edge.To.InEdges.Remove(edge);
			if(edge is FlowEdge<TVertex>)
			{
				if (((FlowEdge<TVertex>)edge).currentFlowInfo != null) //opposite edge
				{
					UltimateEdges[edge].Delete();
					UltimateEdges.Remove(edge);
				}
			}
			else
			{
				UltimateEdges[edge].Delete();
				UltimateEdges.Remove(edge);
			}
				
			
		}
	}
	public class FlowNetwork<TVertex> :Graph<TVertex> where TVertex : VertexBase<TVertex>, new() //TODO inheritance with special type of Edge //TODO there was an implementation with BfsVertex, is that good?
	{
		public FlowNetwork()
		{
		}
		public TVertex Source { get; set; }
		public TVertex Sink { get; set; }
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
			var needToCreateOppositeEdge = new List<FlowEdge<TVertex>>();
			foreach (FlowEdge<TVertex> edge in Edges.Values)
			{
				edge.Flow = 0;
				edge.OppositeEdge = (FlowEdge<TVertex>)GetEdge(edge.To, edge.From);
				if (edge.OppositeEdge == null)
					needToCreateOppositeEdge.Add(edge);
				edge.UpdateCurrentFlowInfo();
			}
			foreach (var edge in needToCreateOppositeEdge)
			{
				AddEdge( edge.To, edge.From, edge.To.UniqueId+"->" + edge.From.UniqueId, 0,null);
				edge.OppositeEdge = (FlowEdge<TVertex>)Edges[edge.To.UniqueId + "->" + edge.From.UniqueId];
				edge.OppositeEdge.OppositeEdge = edge;
			}
			needToCreateOppositeEdge.Clear();
		}
		//TODO be able to add edge only through this method, not with the Graph method
		public FlowEdge<TVertex> AddEdge( TVertex from, TVertex to, string name, int capacity, TextBox txtLength) //TODO má vracet tu hranu
		{
			if (!Edges.ContainsKey(name)) //TODO unique names!!!!
			{
				FlowEdge<TVertex> edge = new FlowEdge<TVertex>(capacity,txtLength);
				edge.Name = name;
				edge.From = from;
				edge.To = to;
				from.OutEdges.Add(edge);
				to.InEdges.Add(edge);
				Edges.Add(from.UniqueId+"->" + to.UniqueId, edge);
				return edge;
			}
			else
			{
				((FlowEdge<TVertex>)Edges[name]).Capacity += capacity;
				return (FlowEdge<TVertex>)Edges[name];
			}
				
			
		}
		public void SetFlowTo(int flow)
		{
			foreach (var edge in Edges.Values)
				((FlowEdge<TVertex>)edge).Flow = flow;
		}

		public int GetMaximumFlow()
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
