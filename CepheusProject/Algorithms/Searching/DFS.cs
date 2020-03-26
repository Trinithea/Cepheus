using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class DFS : IAlgorithm
	{
		public static string Name => "Depth-first search";
		static int Time = 0;
		public static void Run(Graph<DfsVertex> graph,DfsVertex initialVertex)
		{
			foreach(DfsVertex vertex in graph.GetVertices())
			{
				vertex.State = IStateVertex.States.Unvisited;
				vertex.InTime = null;
				vertex.OutTime = null;
			}
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
