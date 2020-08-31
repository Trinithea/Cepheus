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
		public abstract string Name { get; }
		public abstract string TimeComplexity { get; }
		public abstract void Accept(VisitorGraphCreator visitor);
		public abstract Task Accept(VisitorRunner visitor);
		public override string ToString() => Name;
		public abstract string Description { get; } //short description of the algorithm from Wikipedia.com
		protected TextBox outputConsole;
		public void SetOutputConsole(TextBox console) => outputConsole = console;
		protected int delay = 750;
		protected void PrintQueued(BfsVertex vertex)
		{
			outputConsole.Text += "\nVertex " + vertex.Name + " has been enqueued.";
		}
		protected void PrintDequeued(BfsVertex vertex)
		{
			outputConsole.Text += "\nVertex " + vertex.Name + " has been dequeued.";
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

			outputConsole.Text += "\nVertices are inicialized...";
		}
		protected void PrintEdgeAddedToMinimumSpanningTree(Vertex vertex, Vertex predecessor)
		{
			outputConsole.Text += "\nEdge " + vertex.Name + "->"+predecessor.Name+" added to minimum spanning tree.";
		}

	}
	public abstract class Algorithm<TVertex> : Algorithm where TVertex : VertexBase<TVertex>, new()
	{
		public Graph<TVertex> Graph { get; set; }
		public TVertex initialVertex; //TODO kliděn by mohlo být i uvnitř grafu
		public void CreateGraph()
		{
			Graph = new Graph<TVertex>();
			foreach (var vertex in MainWindow.Vertices.Keys)
			{
				Graph.AddVertex(vertex.UniqueId, vertex.Name);
				Graph.UltimateVertices.Add(Graph.GetVertex(vertex.UniqueId), vertex);
			}
			foreach (var edge in MainWindow.Edges.Keys)
			{
				Graph.AddEdge(Graph.GetVertex(edge.FromVertex.UniqueId), Graph.GetVertex(edge.ToVertex.UniqueId), edge.Name, edge.Length);
				Graph.UltimateEdges.Add(Graph.GetEdge(edge.Name), edge);
			}
			initialVertex = Graph.GetVertex((int)MainWindow.initialVertex);
		}
		protected void ColorEdge(Edge<TVertex> edge)
		{
			Graph.UltimateEdges[edge].SetMarkedLook();
		}

		protected void UncolorEdge(Edge<TVertex> edge)
		{
			Graph.UltimateEdges[edge].SetDefaultLook();
		}
		protected void ColorVertex(TVertex vertex)
		{
			Graph.UltimateVertices[vertex].SetMarkedLook();
		}
		protected void UncolorVertex(TVertex vertex)
		{
			Graph.UltimateVertices[vertex].SetDefaultLook();
		}

	}


	
	public abstract class FlowAlgorithm<TVertex> : Algorithm where TVertex : VertexBase<TVertex>, new()
	{
		public FlowNetwork<TVertex> graph;
		public void CreateGraph()
		{
			graph = new FlowNetwork<TVertex>();
			foreach (var vertex in MainWindow.Vertices.Keys)
			{
				graph.AddVertex(vertex.UniqueId, vertex.Name);

			}
			foreach (var edge in MainWindow.Edges.Keys)
			{
				graph.AddEdge(graph.GetVertex(edge.FromVertex.UniqueId), graph.GetVertex(edge.ToVertex.UniqueId), edge.Name, edge.Length);
			}
			graph.Source = graph.Vertices[(int)MainWindow.initialVertex];
			graph.Sink = graph.Vertices[(int)MainWindow.sinkVertex];
		}
	}
}
