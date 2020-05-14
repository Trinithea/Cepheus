using System;
using System.Collections.Generic;
using System.Text;
using CepheusProjectWpf;

namespace Cepheus
{
	public class Bellman_Ford : IAlgorithm
	{
		public void Accept(Visitor visitor)
		{
			visitor.Visit(this);
		}
		public string Name => "Bellman-Ford";
		public string TimeComplexity => "O(n * m)";
		public void Run(Graph<BfsVertex> graph, BfsVertex initialVertex)
		{
			graph.InitializeVertices();

			initialVertex.State = States.Open;

			initialVertex.Distance = 0;

			Queue<BfsVertex> openVertices = new Queue<BfsVertex>();

			openVertices.Enqueue(initialVertex);

			while (openVertices.Count > 0)
			{
				var vertex = openVertices.Dequeue(); // some open vertex
				foreach (Edge<BfsVertex> edge in vertex.OutEdges)
				{
					if (edge.To.Distance == null || edge.To.Distance > (vertex.Distance + edge.Length))
					{
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = States.Open;
						openVertices.Enqueue(edge.To);
						edge.To.Predecessor = vertex;
					}
				}
				vertex.State = States.Closed;
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
