using System;
using System.Collections.Generic;
using System.Text;
using CepheusProjectWpf;

namespace Cepheus
{
	public sealed class Relaxation : IAlgorithm
	{
		public void Accept(Visitor visitor)
		{
			visitor.Visit(this);
		}
		public string Name => "Relaxation algorithm";
		public string TimeComplexity => "O(n^2)";

		public void Run(Graph<BfsVertex> graph, BfsVertex initialVertex)
		{
			graph.InitializeVertices();

			initialVertex.State = States.Open;

			initialVertex.Distance = 0;

			List<BfsVertex> openVertices = new List<BfsVertex>();

			openVertices.Add(initialVertex);

			while(openVertices.Count > 0)
			{
				var vertex = openVertices[0]; // some open vertex
				foreach(Edge<BfsVertex> edge in vertex.OutEdges)
				{
					if (edge.To.Distance == null || edge.To.Distance > (vertex.Distance + edge.Length))
					{
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = States.Open;
						openVertices.Add(edge.To);
						edge.To.Predecessor = vertex;
					}
				}
				vertex.State = States.Closed;
				openVertices.RemoveAt(0);
			}
		}
		public Graph CreateGraph(List<MainWindow.EllipseVertex> vertices, List<MainWindow.ArrowEdge> edges)
		{
			Graph<BfsVertex> graph = new Graph<BfsVertex>();
			foreach (var vertex in vertices)
			{
				graph.AddVertex(new BfsVertex(vertex.Name));
			}
			foreach (var edge in edges)
			{
				graph.AddEdge(graph.GetVertex(edge.FromVertex.Name), graph.GetVertex(edge.ToVertex.Name), edge.Length);
			}
			return graph;
		}
	}
}
