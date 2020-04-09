using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class Kruskal : IAlgorithm
	{
		public string Name => "Kruskal's algorithm";

		public string TimeComplexity => "O(m * log(n))";
		public void Run(Graph<BoruvkaVertex> graph, BoruvkaVertex initialVertex)
		{
			var edges = new List<EdgeWithLength<BoruvkaVertex>>(graph.Edges.Values);
		}
	}
}
