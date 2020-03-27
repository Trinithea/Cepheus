using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	sealed class Relaxation : IAlgorithm
	{
		public string Name => "Relaxation algorithm";
		public string TimeComplexity => "O(n^2)";

		public void Run(Graph<BfsVertex> graph, BfsVertex initialVertex)
		{
			graph.InitializeVertices();

			initialVertex.State = IStateVertex.States.Open;

			initialVertex.Distance = 0;

			List<BfsVertex> openVertices = new List<BfsVertex>();

			openVertices.Add(initialVertex);

			while(openVertices.Count > 0)
			{
				var vertex = openVertices[0]; // some open vertex
				foreach(EdgeWithLength<BfsVertex> edge in vertex.OutEdges)
				{
					if (edge.To.Distance == null || edge.To.Distance > (vertex.Distance + edge.Length))
					{
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = IStateVertex.States.Open;
						openVertices.Add(edge.To);
						edge.To.Predecessor = vertex;
					}
				}
				vertex.State = IStateVertex.States.Closed;
				openVertices.RemoveAt(0);
			}
		}
	}
}
