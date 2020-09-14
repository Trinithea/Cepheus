using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cepheus.DataStructures;
using CepheusProjectWpf;
using CepheusProjectWpf.DataStructures;

namespace Cepheus
{
	public class Kruskal : Algorithm<BoruvkaVertex>
	{
		public override bool IsFlowAlgorithm => false;
		public override bool NeedsOnlyNonNegativeEdgeLenghts => false;
		public override bool DontNeedInitialVertex => true;
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override string Name => CepheusProjectWpf.Properties.Resources.KruskalAlgo;

		public override string TimeComplexity => CepheusProjectWpf.Properties.Resources.KruskalTime;
		/// <summary>
		/// Gradually formed minimum spanning tree.
		/// </summary>
		public TreeWithContextComponents<BoruvkaVertex> MinimumSpanningTree { get; private set; }

		public override string Description => CepheusProjectWpf.Properties.Resources.KruskalDesc;
		/// <summary>
		/// The main method of Kruskal's algorithm. This is where the whole calculation takes place.
		/// </summary>
		/// <returns></returns>
		public async Task Run()
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.ComponentId;
			Graph.InitializeVertices(); // to get OutEdges from each vertex sorted from lightest to heaviest
			PrintVerticesInitialized(Graph);
			outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.OutEdgesSorted; 
			MinimumBinaryHeap<int, Edge<BoruvkaVertex>> edges = Graph.GetEdgesSortedByLength(); 

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
