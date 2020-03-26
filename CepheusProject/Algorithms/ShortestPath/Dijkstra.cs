using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class Dijkstra : IAlgorithm
	{
		public string Name => "Dijkstra's algorithm";
		public string TimeComplexity => "O((n + m) * log(n))";

		public void Run(Graph<BfsVertex> graph, BfsVertex initalVertex)
		{
			// We can use vertex class for BFS algortihm, Distance property will be considered as rating.

			graph.InitializeVertices();

			initalVertex.State = IStateVertex.States.Open;
			initalVertex.Distance = 0;

			SortedList<int?, BfsVertex> openVertices = new SortedList<int?, BfsVertex>();
			openVertices.Add(initalVertex.Distance, initalVertex);
			while(openVertices.Count > 0)
			{
				var vertex = openVertices[openVertices.Keys[0]]; // vertex with minimal rating
				foreach(EdgeWithLength<BfsVertex> edge in vertex.OutEdges)
				{
					if(edge.To.Distance == null || edge.To.Distance > (vertex.Distance + edge.Length))
					{
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = IStateVertex.States.Open;
						openVertices.Add(edge.To.Distance, edge.To);
						edge.To.Predecessor = vertex;
					}
				}
				vertex.State = IStateVertex.States.Closed;
				openVertices.RemoveAt(0);
			}
		}

		public int? LengthOfShortestPathFromTo(Graph<BfsVertex> graph, BfsVertex from, BfsVertex to)
		{

			Run(graph, from); 

			var currentVertex = to;
			int length = 0;

			while(currentVertex.Predecessor != null)
			{
				length += ((EdgeWithLength<BfsVertex>)graph.GetEdge(currentVertex.Predecessor, currentVertex)).Length;
				currentVertex = currentVertex.Predecessor;
			}

			if (currentVertex == from)
				return length;
			else
				return null;
		}
	}
}
