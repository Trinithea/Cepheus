using CepheusProjectWpf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	public class BFS : IAlgorithm
	{
		public string Name => "Breadth-first search";
		public string TimeComplexity => "O(n + m)";

		public void Run (Graph<BfsVertex> graph, BfsVertex initialVertex)
		{
			graph.InitializeVertices();

			initialVertex.State = States.Open;
			initialVertex.Distance = 0;

			Queue<BfsVertex> queue = new Queue<BfsVertex>();
			queue.Enqueue(initialVertex);

			while(queue.Count > 0)
			{
				BfsVertex vertex = queue.Dequeue();
				foreach(Edge<BfsVertex> edge in vertex.OutEdges)
				{
					if (edge.To.State ==States.Unvisited)
					{
						edge.To.State = States.Open;
						edge.To.Distance = vertex.Distance + 1;
						edge.To.Predecessor = vertex;
						queue.Enqueue(edge.To);
					}
				}
				vertex.State = States.Closed;
			}

		}

		//TODO udělat generický před edge, ať to neni tak hnusně nakopírovaný ve FF
		public List<Edge<BfsVertex>> GetPath(Graph<BfsVertex> graph, BfsVertex from, BfsVertex to)
		{
			if (to.Predecessor == null) //'to' is not reachable from 'from'
				return null;
			else
			{
				var currentVertex = to;
				var path = new List<Edge<BfsVertex>>();
				while (currentVertex.Predecessor != null)
				{
					path.Insert(0, (Edge<BfsVertex>)graph.GetEdge(currentVertex.Predecessor, currentVertex));
					currentVertex = currentVertex.Predecessor;
				}
				return path;
			}
		}

		public Graph CreateGraph(List<MainWindow.EllipseVertex> vertices, List<MainWindow.ArrowEdge> edges)
		{
			Graph<BfsVertex> graph = new Graph<BfsVertex>();
			foreach(var vertex in vertices)
			{
				graph.AddVertex(new BfsVertex(vertex.Name));
			}
			foreach(var edge in edges)
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
