﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class DFS : IAlgorithm
	{
		public string Name => "Depth-first search";
		public string TimeComplexity => "O(n + m)";
		static int Time = 0;
		public void Run(Graph<DfsVertex> graph,DfsVertex initialVertex)
		{
			graph.InitializeVertices();

			Time = 0;
			
			Recursion(initialVertex);
		}
		static void Recursion(DfsVertex vertex)
		{
			vertex.State = IStateVertex.States.Open;
			Time++;
			vertex.InTime = Time;
			foreach(Edge<DfsVertex> edge in vertex.OutEdges)
			{
				if (edge.To.State == IStateVertex.States.Unvisited)
					Recursion(edge.To);
			}
			vertex.State = IStateVertex.States.Closed;
			Time++;
			vertex.OutTime = Time;
		}
	}
}
