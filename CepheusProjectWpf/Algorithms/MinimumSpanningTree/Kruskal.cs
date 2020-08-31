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

		public override string Description => "Kruskal's algorithm is a minimum-spanning-tree algorithm which finds an edge of the least possible weight that connects any two trees in the forest. It is a greedy algorithm in graph theory as it finds a minimum spanning tree for a connected weighted graph adding increasing cost arcs at each step. This means it finds a subset of the edges that forms a tree that includes every vertex, where the total weight of all the edges in the tree is minimized. If the graph is not connected, then it finds a minimum spanning forest (a minimum spanning tree for each connected component). ";

		public async Task Run()
		{
			Graph.InitializeVertices(); // to get OutEdges sorted
			PrintVerticesInitialized(Graph);
			outputConsole.Text += "\nOutEdges are sorted from lightest to heaviest for each vertex."; //TODO asi každej neví, co je outedges...
			var edges = Graph.GetEdgesSortedByLength(); //minimum binary heap

			var minimumSpanningTree = new TreeWithContextComponents<BoruvkaVertex>();
			minimumSpanningTree.Vertices = new List<BoruvkaVertex>(Graph.Vertices.Values);
			List<int> ids = new List<int>(); //ID numbers of currents context components
			Boruvka boruvkaAlgorithm = new Boruvka();
			boruvkaAlgorithm.SetOutputConsole(outputConsole);
			boruvkaAlgorithm.Graph = Graph;
			boruvkaAlgorithm.Initialize(minimumSpanningTree, ids); //we can use the same method for initializing components nad IDs

			for (int i = 1; i <= edges.Count; i++)
			{
				if(edges.Heap[i].Item2.From.ComponentID != edges.Heap[i].Item2.To.ComponentID)
				{
					await Task.Delay(delay);
					ColorEdge(edges.Heap[i].Item2);
					minimumSpanningTree.Edges.Add(edges.Heap[i].Item2.Name, edges.Heap[i].Item2);
					PrintEdgeAddedToMinimumSpanningTree(edges.Heap[i].Item2.From, edges.Heap[i].Item2.To);
					minimumSpanningTree.NewEdges.Add(edges.Heap[i].Item2);
					boruvkaAlgorithm.MergeContextComponents(minimumSpanningTree, ids);
				}
			}

			MinimumSpanningTree = minimumSpanningTree;
		}
		

	}
}
