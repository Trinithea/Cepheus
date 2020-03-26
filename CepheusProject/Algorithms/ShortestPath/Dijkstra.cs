using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class Dijkstra : IAlgorithm
	{
		public static string Name => "Dijkstra's algorithm";

		public static void Run(Graph<BfsVertex> graph, BfsVertex initalVertex)
		{
			// We can use vertex class for BFS algortihm, Distance property will be considered as rating.

			var graphVertices = graph.GetVertices();
			foreach (BfsVertex vertex in graphVertices)
			{
				vertex.State = IStateVertex.States.Unvisited;
				vertex.Distance = null;
				vertex.Predecessor = null;
			}

			initalVertex.State = IStateVertex.States.Open;
			initalVertex.Distance = 0;

			SortedList<int?, BfsVertex> openVertcices = new SortedList<int?, BfsVertex>();
			openVertcices.Add(initalVertex.Distance, initalVertex);
			while(openVertcices.Count > 0)
			{
				var vertex = openVertcices[openVertcices.Keys[0]]; // vertex with minimal rating
				foreach(EdgeWithNaturalLength<BfsVertex> edge in vertex.OutEdges)
				{
					if(edge.To.Distance == null || edge.To.Distance > (vertex.Distance + edge.Length))
					{
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = IStateVertex.States.Open;
						openVertcices.Add(edge.To.Distance, edge.To);
						edge.To.Predecessor = vertex;
					}
				}
				vertex.State = IStateVertex.States.Closed;
				openVertcices.RemoveAt(0);
			}
		}

		public static int? LengthOfShortestPathFromTo(Graph<BfsVertex> graph, BfsVertex from, BfsVertex to)
		{

			Run(graph, from); 

			var currentVertex = to;
			int length = 0;

			while(currentVertex.Predecessor != null)
			{
				length += ((EdgeWithNaturalLength<BfsVertex>)graph.GetEdge(currentVertex.Predecessor, currentVertex)).Length;
				currentVertex = currentVertex.Predecessor;
			}

			if (currentVertex == from)
				return length;
			else
				return null;
		}
	}
}
