using System;
using System.Collections.Generic;
using System.Text;
using Cepheus.DataStructures;

namespace Cepheus
{
	class Jarnik : IAlgorithm
	{
		//TODO Jarník je na neorientovnaý grafy
		public string Name => "Jarnik's algorithm";

		public string TimeComplexity => "O(m * log(n))";
		public Tree<JarnikVertex> minimalSpan;
		public void Run(Graph<JarnikVertex> graph, JarnikVertex initialVertex)
		{
			graph.InitializeVertices();

			minimalSpan = new Tree<JarnikVertex>();
			minimalSpan.Vertices.Add(initialVertex);
			initialVertex.State = JarnikVertex.States.Neighbour;
			initialVertex.Rating = 0;

			SortedList<int?, JarnikVertex> neighbours = new SortedList<int?, JarnikVertex>();
			neighbours.Add(initialVertex.Rating,initialVertex);

			while(neighbours.Count > 0)
			{
				var vertex = neighbours[neighbours.Keys[0]]; // neighbour with minimal rating
				vertex.State = JarnikVertex.States.Inside;
				if (vertex.Predecessor != null)
					minimalSpan.Edges.Add(graph.GetEdge(vertex, vertex.Predecessor));

				foreach(EdgeWithLength<JarnikVertex> edge in vertex.OutEdges)
				{
					if((edge.To.State == JarnikVertex.States.Neighbour || edge.To.State == JarnikVertex.States.Outside)
						&& (edge.To.Rating == null || edge.To.Rating > edge.Length))
					{
						edge.To.State = JarnikVertex.States.Neighbour;
						edge.To.Rating = edge.Length;
						edge.To.Predecessor = vertex;

					}
				}	

			}
		}

		public Tree<JarnikVertex> GetMinimalSpan() => minimalSpan;
	}
}
