using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cepheus.DataStructures;
using CepheusProjectWpf;

namespace Cepheus
{
	public class Kruskal : Algorithm<BoruvkaVertex>
	{
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override string Name => "Kruskal's algorithm";

		public override string TimeComplexity => "O(m * log(n))";
		public TreeWithContextComponents<BoruvkaVertex> MinimumSpanningTree { get; private set; }
		public async Task Run()
		{
			var edges = graph.GetEdgesSortedByLength();

			var minimumSpanningTree = new TreeWithContextComponents<BoruvkaVertex>();
			minimumSpanningTree.Vertices = new List<BoruvkaVertex>(graph.Vertices.Values);
			List<int> ids = new List<int>(); //ID numbers of currents context components
			Boruvka boruvkaAlgorithm = new Boruvka();
			boruvkaAlgorithm.Initialize(minimumSpanningTree, ids); //we can use the same method for initializing components nad IDs

			for (int i = 0; i < edges.Count; i++)
			{
				if(edges[i].From.ComponentID != edges[i].To.ComponentID)
				{
					minimumSpanningTree.Edges.Add(edges[i].Name, edges[i]);
					minimumSpanningTree.NewEdges.Add(edges[i]);
					boruvkaAlgorithm.MergeContextComponents(minimumSpanningTree, ids);
				}
			}

			MinimumSpanningTree = minimumSpanningTree;
		}
		

	}
}
