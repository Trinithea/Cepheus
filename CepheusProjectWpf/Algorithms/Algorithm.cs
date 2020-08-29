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
			outputConsole.Text += "\n\nVertices are inicialized.";
			foreach (var vertex in graph.Vertices.Values)
				PrintVertex(vertex);
			outputConsole.Text += "\n";
		}
		protected void PrintVertexAddedToMinimumSpanningTree(Vertex vertex)
		{
			outputConsole.Text += "\nVertex " + vertex.Name + " added to minimum spanning tree";
		}

	}
	public abstract class Algorithm<TVertex> : Algorithm where TVertex : VertexBase<TVertex>, new()
	{
		public Graph<TVertex> graph;
		public TVertex initialVertex; //TODO kliděn by mohlo být i uvnitř grafu
		public void CreateGraph()
		{
			graph = new Graph<TVertex>();
			foreach (var vertex in MainWindow.Vertices.Keys)
			{
				graph.AddVertex(vertex.UniqueId, vertex.Name);
				graph.UltimateVertices.Add(graph.GetVertex(vertex.UniqueId), vertex);
			}
			foreach (var edge in MainWindow.Edges.Keys)
			{
				graph.AddEdge(graph.GetVertex(edge.FromVertex.UniqueId), graph.GetVertex(edge.ToVertex.UniqueId), edge.Name, edge.Length);
				graph.UltimateEdges.Add(graph.GetEdge(edge.Name), edge);
			}
			initialVertex = graph.GetVertex((int)MainWindow.initialVertex);
		}
		protected void ColorEdge(Edge<TVertex> edge)
		{
			graph.UltimateEdges[edge].SetMarkedLook();
		}
		protected void UncolorEdge(Edge<TVertex> edge)
		{
			graph.UltimateEdges[edge].SetDefaultLook();
		}
		protected void ColorVertex(TVertex vertex)
		{
			graph.UltimateVertices[vertex].SetMarkedLook();
		}
		protected void UncolorVertex(TVertex vertex)
		{
			graph.UltimateVertices[vertex].SetDefaultLook();
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
