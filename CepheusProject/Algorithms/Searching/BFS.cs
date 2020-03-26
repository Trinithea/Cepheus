using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class BFS : IAlgorithm
	{
		public static string Name => "Breadth-first search";

		public static void Run (Graph<BfsVertex> graph, BfsVertex initialVertex)
		{
			foreach(BfsVertex vertex in graph.GetVertices())
			{
				vertex.State = IStateVertex.States.Unvisited;
				vertex.Distance = null;
				vertex.Predecessor = null;
			}

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
	}
}
