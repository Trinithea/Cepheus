using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class BFS : IAlgorithm
	{
		public string Name => "Breadth-first search";
		public string TimeComplexity => "O(n + m)";

		public void Run (Graph<BfsVertex> graph, BfsVertex initialVertex)
		{
			graph.InitializeVertices();

			initialVertex.State = IStateVertex.States.Open;
			initialVertex.Distance = 0;

			Queue<BfsVertex> queue = new Queue<BfsVertex>();
			queue.Enqueue(initialVertex);

			while(queue.Count > 0)
			{
				BfsVertex vertex = queue.Dequeue();
				foreach(Edge<BfsVertex> edge in vertex.OutEdges)
				{
					if (edge.To.State == IStateVertex.States.Unvisited)
					{
						edge.To.State = IStateVertex.States.Open;
						edge.To.Distance = vertex.Distance + 1;
						edge.To.Predecessor = vertex;
						queue.Enqueue(edge.To);
					}
				}
				vertex.State = IStateVertex.States.Closed;
			}

		}
		public List<Edge<BfsVertex>> GetPath(Graph<BfsVertex> graph, BfsVertex from, BfsVertex to)
		{
			Run(graph, from);
			if (to.Predecessor == null) //'to' is not reachable from 'from'
				return null;
			else
			{
				var currentVertex = to;
				var edges = new List<Edge<BfsVertex>>();
				var vertices = new List<BfsVertex>();
				while (currentVertex.Predecessor != null)
				{
					vertices.Add(currentVertex);
					currentVertex = currentVertex.Predecessor;
				}
				vertices.Add(currentVertex);

				for (int i = vertices.Count - 1; i > 0; i--)
					edges.Add(graph.GetEdge(vertices[i], vertices[i - 1]));

				return edges;
			}
		}
	}
}
