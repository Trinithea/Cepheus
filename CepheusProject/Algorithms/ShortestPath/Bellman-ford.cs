using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class Bellman_Ford : IAlgorithm
	{
		public string Name => "Bellman-Ford";
		public string TimeComplexity => "O(n * m)";
		public void Run(Graph<BfsVertex> graph, BfsVertex initialVertex)
		{
			graph.InitializeVertices();

			initialVertex.State = IStateVertex.States.Open;

			initialVertex.Distance = 0;

			Queue<BfsVertex> openVertices = new Queue<BfsVertex>();

			openVertices.Enqueue(initialVertex);

			while (openVertices.Count > 0)
			{
				var vertex = openVertices.Dequeue(); // some open vertex
				foreach (EdgeWithLength<BfsVertex> edge in vertex.OutEdges)
				{
					if (edge.To.Distance == null || edge.To.Distance > (vertex.Distance + edge.Length))
					{
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = IStateVertex.States.Open;
						openVertices.Enqueue(edge.To);
						edge.To.Predecessor = vertex;
					}
				}
				vertex.State = IStateVertex.States.Closed;
			}
		}
	}
}
