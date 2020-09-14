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
		/// <summary>
		/// All edges in the graph searchable by their unique name.
		/// </summary>
		public Dictionary<string, Edge<TVertex>> Edges { get; private set; }
		/// <summary>
		/// All vertices in the graph searchable by their unique ID.
		/// </summary>
		public Dictionary<int, TVertex> Vertices { get; private set; }
		/// <summary>
		/// Pairs of the edges used by the algorithm with those seen by the user
		/// </summary>
		public Dictionary<Edge<TVertex>, ArrowEdge> UltimateEdges { get; private set; }
		/// <summary>
		/// Pairs of the vertices used by the algorithm with those seen by the user.
		/// </summary>
		public Dictionary<TVertex, EllipseVertex> UltimateVertices { get; private set; }
		/// <summary>
		/// Creates a vertex suitable for the algorithm to run from the vertex that the user sees and returns it.
		/// </summary>
		/// <param name="ellipseVertex"></param>
		/// <returns></returns>
		public TVertex AddVertex(EllipseVertex ellipseVertex)
		{
			var vertex = new TVertex();
			vertex.UniqueId = ellipseVertex.UniqueId;
			vertex.Name = ellipseVertex.Name;
			vertex.txtName = ellipseVertex.txtName;
			Vertices.Add(ellipseVertex.UniqueId, vertex);
			UltimateVertices.Add(vertex, ellipseVertex);
			return vertex;
		}
		/// <summary>
		/// Creates an edge suitable for running the algorithm from an edge that the user sees. Adds it to the chart and returns it.
		/// It is necessary to call this method only after adding the vertex from which the edge comes out and into which it enters.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="name"></param>
		/// <param name="length"></param>
		public Edge<TVertex> AddEdge(ArrowEdge arrowEdge)
		{
			if (!Edges.ContainsKey(arrowEdge.Name))
			{
				Edge<TVertex> edge = new Edge<TVertex>();
				edge.Name = arrowEdge.Name;
				edge.From = GetVertex(arrowEdge.FromVertex.UniqueId);
				edge.To = GetVertex(arrowEdge.ToVertex.UniqueId);
				edge.Length = arrowEdge.Length;
				edge.From.OutEdges.Add(edge);
				edge.To.InEdges.Add(edge);
				Edges.Add(edge.Name, edge);
				UltimateEdges.Add(edge, arrowEdge);
				return edge;
			}
			else
			{
				Edges[arrowEdge.Name].Length += arrowEdge.Length; //if the user has added more edges in one direction, only one edge with a length equal to the sum of the lengths of these edges is added
				return Edges[arrowEdge.Name];
			}
			
		}
		/// <summary>
		/// Each vertex is initialized to a default state.
		/// </summary>
		public void InitializeVertices()
		{
			foreach (TVertex vertex in Vertices.Values)
			{
				vertex.Initialize();
			}

		}
		/// <summary>
		/// Returns a specific vertex from a graph, or null if such a vertex does not appear in the graph.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public TVertex GetVertex(int id)
		{
			if (Vertices.ContainsKey(id))
				return Vertices[id];
			else
				return null;
		}
		/// <summary>
		/// Returns a specific edge from a graph, or null if such a edge does not appear in the graph.
		/// </summary>
		/// <param name="fromNameToName"></param>
		/// <returns></returns>
		public Edge<TVertex> GetEdge(string fromNameToName)
		{
			if (Edges.ContainsKey(fromNameToName))
				return Edges[fromNameToName];
			else
				return null;
		}
		/// <summary>
		/// Returns a specific edge from a graph, or null if such a edge does not appear in the graph.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public Edge<TVertex> GetEdge(TVertex from, TVertex to)
		{
			string name = from.UniqueId +"->" + to.UniqueId;
			if (Edges.ContainsKey(name))
				return Edges[name];
			else
				return null;
		}
		/// <summary>
		/// Arranges the edges according to their length into a minimum binary heap, which it then returns.
		/// </summary>
		/// <returns></returns>
		public MinimumBinaryHeap<int, Edge<TVertex>> GetEdgesSortedByLength()
		{
			var edges = new MinimumBinaryHeap<int, Edge<TVertex>>(Edges.Count);
			foreach (Edge<TVertex> edge in Edges.Values)
				edges.Insert(edge.Length, edge);
			return edges;
		}
		/// <summary>
		/// Properly removes the vertex from the graph and with all edges that enter or leave it.
		/// </summary>
		/// <param name="vertex"></param>
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
		/// <summary>
		/// Properly removes an edge from the graph.
		/// </summary>
		/// <param name="edge"></param>
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
	
	public class FlowNetwork<TVertex> :Graph<TVertex> where TVertex : VertexBase<TVertex>, new() 
	{
		public FlowNetwork()
		{
		}
		/// <summary>
		/// Source vertex.
		/// </summary>
		public TVertex Source { get; set; }
		/// <summary>
		/// Sink vertex.
		/// </summary>
		public TVertex Sink { get; set; }
		/// <summary>
		/// Sets initial flow to zero for each edge. Creates artificial opposite edges so that the flow can flow in the opposite direction.
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
		/// <summary>
		/// Properly adds an edge to the graph. However, it does not add to UltimateEdges, as this could incorrectly add an artificial edge that the user does not see.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="name"></param>
		/// <param name="capacity"></param>
		/// <param name="txtLength"></param>
		/// <returns></returns>
		public FlowEdge<TVertex> AddEdge( TVertex from, TVertex to, string name, int capacity, TextBox txtLength)
		{
			if (!Edges.ContainsKey(name))
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
				((FlowEdge<TVertex>)Edges[name]).Capacity += capacity; //if the user has added more edges in one direction, only one edge with a capacity equal to the sum of the lengths of these edges is added
				return (FlowEdge<TVertex>)Edges[name];
			}
		}
		/// <summary>
		/// Sets the flow to all edges to the value passed in the method argument.
		/// </summary>
		/// <param name="flow"></param>
		public void SetFlowTo(int flow)
		{
			foreach (var edge in Edges.Values)
				((FlowEdge<TVertex>)edge).Flow = flow;
		}
		/// <summary>
		/// Returns the maximum flow in the graph according to how much flowed into the sink.
		/// </summary>
		/// <returns></returns>
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
