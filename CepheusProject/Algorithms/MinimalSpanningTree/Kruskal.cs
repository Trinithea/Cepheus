using System;
using System.Collections.Generic;
using System.Text;
using Cepheus.DataStructures;

namespace Cepheus
{
	class Kruskal : IAlgorithm
	{
		public string Name => "Kruskal's algorithm";

		public string TimeComplexity => "O(m * log(n))";
		public TreeWithContextComponents<BoruvkaVertex> MinimalSpanningTree { get; private set; }
		public void Run(Graph<BoruvkaVertex> graph, BoruvkaVertex initialVertex)
		{
			var edges = graph.GetEdgesSortedByLength();

			var minimalSpanningTree = new TreeWithContextComponents<BoruvkaVertex>();
			minimalSpanningTree.Vertices = new List<BoruvkaVertex>(graph.Vertices.Values);
			List<int> ids = new List<int>(); //ID numbers of currents context components
			Boruvka boruvkaAlgorithm = new Boruvka();
			boruvkaAlgorithm.Initialize(minimalSpanningTree, ids); //we can use the same method for initializing components nad IDs

			for (int i = 0; i < edges.Count; i++)
			{
				if(edges[i].From.ComponentID != edges[i].To.ComponentID)
				{
					minimalSpanningTree.Edges.Add(edges[i].Name, edges[i]);
					minimalSpanningTree.NewEdges.Add(edges[i]);
					boruvkaAlgorithm.MergeContextComponents(minimalSpanningTree, ids);
				}
			}

			MinimalSpanningTree = minimalSpanningTree;
		}

	}
}
