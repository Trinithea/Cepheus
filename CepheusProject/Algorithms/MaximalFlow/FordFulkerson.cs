using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class FordFulkerson : IAlgorithm
	{
		public string Name => "Ford-Fulkerson's algorithm";

		public string TimeComplexity => "O(m * f)";

		public void Run(FlowNetwork<BfsVertex> graph, BfsVertex initialVertex)
		{
			graph.InitializeEdges();
			BFS bfs = new BFS();
			var path = bfs.GetPath(graph, graph.Source, graph.Sink);
			while(path != null || )
			{

			}
		}

	}
}
