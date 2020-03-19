using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class Dijkstra : IAlgorithm
	{
		public static string Name => "Dijkstra's algorithm";

		public void Run(Graph<BfsVertex> graph, BfsVertex initalVertex)
		{
			// We can use vertex class for BFS algortihm, Distance property will be considered as rating.
			// Vertices are initialized by default.

			initalVertex.State = IStateVertex.States.Open;
			initalVertex.Distance = 0;

			SortedDictionary<int?, BfsVertex> openVertcices = new SortedDictionary<int?, BfsVertex>();
			openVertcices.Add(initalVertex.Distance, initalVertex);
			while(openVertcices.Count > 0)
			{
				var vertex = openVertcices[0]; // vertex with minimal rating
				foreach(EdgeWithNaturalLength<BfsVertex> edge in vertex.OutEdges)
				{
					if(edge.To.Distance == null || edge.To.Distance > vertex.Distance + edge.Length)
					{
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = IStateVertex.States.Open;
						openVertcices.Add(edge.To.Distance, edge.To);
						edge.To.Predecessor = vertex;
					}
				}
				vertex.State = IStateVertex.States.Closed;
			}
		}
	}
}
