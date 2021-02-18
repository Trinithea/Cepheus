using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using CepheusProjectWpf;

namespace Cepheus
{
	public abstract class Algorithm 
	{
		/// <summary>
		/// Determines if this is a maximum flow search algorithm that is special in many ways.
		/// </summary>
		public abstract bool IsFlowAlgorithm { get; }
		/// <summary>
		/// Specifies whether the algorithm requires only non-negative length edges.
		/// </summary>
		public abstract bool NeedsOnlyNonNegativeEdgeLenghts { get; }
		/// <summary>
		/// Determines if the algorithm needs a specific initial vertex.
		/// </summary>
		public abstract bool DontNeedInitialVertex { get; }
		/// <summary>
		/// Algorithm name
		/// </summary>
		public abstract string Name { get; }
		/// <summary>
		/// Time complexity of the algorithm, where m is the number of edges and n is the number of vertices.
		/// </summary>
		public abstract string TimeComplexity { get; }
		/// <summary>
		/// Using the design pattern, Visitor creates a graph suitable for the algorithm.
		/// </summary>
		/// <param name="visitor"></param>
		public abstract void Accept(VisitorGraphCreator visitor);
		/// <summary>
		/// Using the Visitor design pattern, the algorithm is started.
		/// </summary>
		/// <param name="visitor"></param>
		/// <returns></returns>
		public abstract Task Accept(VisitorRunner visitor);
		public override string ToString() => Name; // For the menu of algorithms in the main window
		/// <summary>
		/// Short description of the algorithm from Wikipedia.com
		/// </summary>
		public abstract string Description { get; }
		/// <summary>
		/// Console in the main window where the individual steps of the algorithm are listed.
		/// </summary>
		protected TextBox outputConsole;
		/// <summary>
		/// Sets the outputConsole from the outside. It must be done before running the algorithm.
		/// </summary>
		/// <param name="console"></param>
		public void SetOutputConsole(TextBox console) => outputConsole = console;
		/// <summary>
		/// Central length of algorithm run due to animations.
		/// </summary>
		protected int delay = 750;
		protected async Task Delay(int delay)
		{
			if(MainWindow.StopEverything != true)
				await Task.Delay(delay);

		}
		protected void PrintQueued(BfsVertex vertex)
		{
			outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.NLVertexSpace + vertex.Name + CepheusProjectWpf.Properties.Resources.Enqued;
		}
		protected void PrintDequeued(BfsVertex vertex)
		{
			outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.NLVertexSpace + vertex.Name + CepheusProjectWpf.Properties.Resources.Dequed;
		}
		protected void PrintVertex(Vertex vertex)
		{
			outputConsole.Text += vertex.Informations;
		}
		protected void PrintVerticesInitialized<T>(Graph<T> graph) where T: VertexBase<T>, new()
		{
			outputConsole.Text += "\n\n";
			foreach (var vertex in graph.Vertices.Values)
				PrintVertex(vertex);
			outputConsole.Text += "\n";

			outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.VerticesInicialized;
		}
		protected void PrintEdgesAreInitialized()
		{
			outputConsole.Text += "\n\n"+ CepheusProjectWpf.Properties.Resources.EdgesInicialized;
		}
		protected void PrintEdgeAddedToMinimumSpanningTree(Vertex vertex, Vertex predecessor)
		{
			outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.NLEdgeSpace + vertex.Name + "->"+predecessor.Name+ CepheusProjectWpf.Properties.Resources.AddedToMinSpTree;
		}
		/// <summary>
		/// Prints "In brackets is the state of the vertex and its distance from inital vertex."
		/// </summary>
		protected void PrintInfoStateDistance()
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.StateDistanceInfo;
		}
		
	}
	public abstract class Algorithm<TVertex> : Algorithm where TVertex : VertexBase<TVertex>, new()
	{
		/// <summary>
		/// Graph on which the algorithm runs.
		/// </summary>
		public Graph<TVertex> Graph { get; set; }
		/// <summary>
		/// User-selected initial vertex.
		/// </summary>
		public TVertex initialVertex;
		/// <summary>
		/// Creating a graph to run the algorithm from a graph created by the user.
		/// </summary>
		public void CreateGraph()
		{
			Graph = new Graph<TVertex>();
			foreach (var vertex in MainWindow.Vertices.Keys)
			{
				Graph.AddVertex(vertex);
			}
			foreach (var edge in MainWindow.Edges.Keys)
			{
				Graph.AddEdge(edge);
			}
			if(MainWindow.initialVertex != null)
				initialVertex = Graph.GetVertex((int)MainWindow.initialVertex);
		}
		/// <summary>
		/// Highlights a specific edge in a user-created graph.
		/// </summary>
		/// <param name="edge"></param>
		public void ColorEdge(Edge<TVertex> edge)
		{
			//Although there is a different version of the ColorEdge (...) method for FlowEdge<TVertex> in the FlowAlgorithm<TVertex> class, 
			//it is possible that this method is called from an algorithm that is not used to calculate the maximum flow and still contains FlowEdges
			if (edge is FlowEdge<TVertex>)
				if (((FlowEdge<TVertex>)edge).currentFlowInfo != null) //if null, then edge is artificially created opposite edge
					Graph.UltimateEdges[edge].SetMarkedLook();
				else { }
			else
			{
				Canvas.SetZIndex(Graph.UltimateEdges[edge].MainLine, 2);
				Graph.UltimateEdges[edge].SetMarkedLook();
			}
		}
		/// <summary>
		/// The specific edge is set to its default appearance in a user-created graph.
		/// </summary>
		/// <param name="edge"></param>
		protected void UncolorEdge(Edge<TVertex> edge)
		{
			//Although there is a different version of the ColorEdge (...) method for FlowEdge<TVertex> in the FlowAlgorithm<TVertex> class, 
			//it is possible that this method is called from an algorithm that is not used to calculate the maximum flow and still contains FlowEdges
			if (edge is FlowEdge<TVertex>)
				if (((FlowEdge<TVertex>)edge).currentFlowInfo != null) //if null, then edge is artificially created opposite edge
					Graph.UltimateEdges[edge].SetDefaultLook();					
				else { }
			else
			{
				Canvas.SetZIndex(Graph.UltimateEdges[edge].MainLine, 1);
				Graph.UltimateEdges[edge].SetDefaultLook();
			}
		}
		/// <summary>
		/// Highlights the vertex in a user-created graph.
		/// </summary>
		/// <param name="vertex"></param>
		protected void ColorVertex(TVertex vertex)
		{
			Graph.UltimateVertices[vertex].SetMarkedLook();
		}
		/// <summary>
		/// Returns the vertex to the default appearance in the user-created graph.
		/// </summary>
		/// <param name="vertex"></param>
		protected void UncolorVertex(TVertex vertex)
		{
			Graph.UltimateVertices[vertex].SetDefaultLook();
		}
		/// <summary>
		/// Highlight the shortest path.
		/// </summary>
		protected void ColorShortestPaths(Algorithm<BfsVertex> algorithm)
		{
			foreach (var vertex in algorithm.Graph.Vertices.Values)
			{
				if (vertex.Predecessor != null)
					vertex.ColorEdgeWithPredecessor(algorithm);
			}
		}
	}


	
	public abstract class FlowAlgorithm<TVertex> : Algorithm where TVertex : VertexBase<TVertex>, new()
	{
		/// <summary>
		/// Graph on which the algorithm runs.
		/// </summary>
		public FlowNetwork<TVertex> graph;
		/// <summary>
		/// Maximum flow in the graph
		/// </summary>
		public int MaximumFlow { get; protected set; }
		/// <summary>
		/// Creating a graph to run the algorithm from a graph created by the user.
		/// </summary>
		public void CreateGraph()
		{
			graph = new FlowNetwork<TVertex>();
			foreach (var vertex in MainWindow.Vertices.Keys)
			{
				graph.AddVertex(vertex);
			}
			foreach (var edge in MainWindow.Edges.Keys)
			{
				graph.AddEdge(graph.GetVertex(edge.FromVertex.UniqueId), graph.GetVertex(edge.ToVertex.UniqueId), edge.Name, edge.Length, edge.txtLength);
				graph.UltimateEdges.Add(graph.GetEdge(edge.Name), edge);
			}
			graph.Source = graph.Vertices[(int)MainWindow.initialVertex];
			graph.Sink = graph.Vertices[(int)MainWindow.sinkVertex];
		}
		protected void PrintMaximumFlow()
		{
			outputConsole.Text += "\n\n"+ CepheusProjectWpf.Properties.Resources.MaxFlow + MaximumFlow;
		}
		/// <summary>
		/// Highlights a specific edge in a user-created graph.
		/// </summary>
		/// <param name="graph"></param>
		/// <param name="edge"></param>
		protected void ColorEdge(FlowNetwork<TVertex> graph,FlowEdge<TVertex> edge)
		{
			if (edge.currentFlowInfo != null) //if null, then edge is artificially created opposite edge
				graph.UltimateEdges[edge].SetMarkedLook();
		}
		/// <summary>
		/// The specific edge is set to its default appearance in a user-created graph.
		/// </summary>
		/// <param name="graph"></param>
		/// <param name="edge"></param>
		protected void UncolorEdge(FlowNetwork<TVertex> graph, FlowEdge<TVertex> edge)
		{
			if (edge.currentFlowInfo != null) //if null, then edge is artificially created opposite edge
				graph.UltimateEdges[edge].SetDefaultLook();
		}
		/// <summary>
		/// Highlights the vertex in a user-created graph.
		/// </summary>
		/// <param name="graph"></param>
		/// <param name="vertex"></param>
		protected void ColorVertex(FlowNetwork<TVertex> graph, TVertex vertex) => graph.UltimateVertices[vertex].SetMarkedLook();
		/// <summary>
		/// Returns the vertex to the default appearance in the user-created graph.S
		/// </summary>
		/// <param name="graph"></param>
		/// <param name="vertex"></param>
		protected void UncolorVertex(FlowNetwork<TVertex> graph, TVertex vertex) => graph.UltimateVertices[vertex].SetDefaultLook();

		
	}
}
