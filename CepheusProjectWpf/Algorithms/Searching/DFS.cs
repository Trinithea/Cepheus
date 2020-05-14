using System;
using System.Collections.Generic;
using System.Text;
using CepheusProjectWpf;

namespace Cepheus
{
	public class DFS : IAlgorithm
	{
		public string Name => "Depth-first search";
		public string TimeComplexity => "O(n + m)";
		static int Time = 0;
		public void Run(Graph<DfsVertex> graph,DfsVertex initialVertex)
		{
			graph.InitializeVertices();

			Time = 0;
			
			Recursion(initialVertex);
		}
		static void Recursion(DfsVertex vertex)
		{
			vertex.State = States.Open;
			Time++;
			vertex.InTime = Time;
			foreach(Edge<DfsVertex> edge in vertex.OutEdges)
			{
				if (edge.To.State == States.Unvisited)
					Recursion(edge.To);
			}
			vertex.State = States.Closed;
			Time++;
			vertex.OutTime = Time;
		}
		public Graph CreateGraph(List<MainWindow.EllipseVertex> vertices, List<MainWindow.ArrowEdge> edges)
		{
			Graph<DfsVertex> graph = new Graph<DfsVertex>();
			foreach (var vertex in vertices)
			{
				graph.AddVertex(new DfsVertex(vertex.Name));
			}
			foreach (var edge in edges)
			{
				graph.AddEdge(graph.GetVertex(edge.FromVertex.Name), graph.GetVertex(edge.ToVertex.Name), edge.Length);
			}
			return graph;
		}
		public void Accept(Visitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
