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
		Tree<JarnikVertex> MinimalSpanningTree;
		public void Run(Graph<JarnikVertex> graph, JarnikVertex initialVertex)
		{
			graph.InitializeVertices();

			MinimalSpanningTree = new Tree<JarnikVertex>();
			initialVertex.State = JarnikVertex.States.Neighbour;
			initialVertex.Rating = 0;

			SortedList<int?, JarnikVertex> neighbours = new SortedList<int?, JarnikVertex>();
			neighbours.Add(initialVertex.Rating,initialVertex);

			while(neighbours.Count > 0)
			{
				var vertex = neighbours[neighbours.Keys[0]]; // neighbour with minimal rating
				vertex.State = JarnikVertex.States.Inside;
				neighbours.RemoveAt(0);
				if (vertex.Predecessor != null)
					MinimalSpanningTree.Edges.Add(graph.GetEdge(vertex.Predecessor, vertex));

				foreach(EdgeWithLength<JarnikVertex> edge in vertex.OutEdges)
				{
					if((edge.To.State == JarnikVertex.States.Neighbour || edge.To.State == JarnikVertex.States.Outside)
						&& (edge.To.Rating == null || edge.To.Rating > edge.Length))
					{
						edge.To.State = JarnikVertex.States.Neighbour;
						edge.To.Rating = edge.Length;
						edge.To.Predecessor = vertex;
						if(!neighbours.ContainsValue(edge.To))
							neighbours.Add(edge.To.Rating, edge.To);
					}
				}	

			}
		}

		public Tree<JarnikVertex> GetMinimalSpan() => MinimalSpanningTree; //TODO can't be null
		public int GetWeightOfMinimalSpan()
		{
			int sum = 0;
			foreach (EdgeWithLength<JarnikVertex> edge in MinimalSpanningTree.Edges)
				sum += edge.Length;
			return sum;
		}
	}
}
