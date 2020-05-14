using System;
using System.Collections.Generic;
using System.Text;
using Cepheus.DataStructures;
using CepheusProjectWpf;

namespace Cepheus
{
	public class Kruskal : IAlgorithm
	{
		public void Accept(Visitor visitor)
		{
			visitor.Visit(this);
		}
		public string Name => "Kruskal's algorithm";

		public string TimeComplexity => "O(m * log(n))";
		public TreeWithContextComponents<BoruvkaVertex> MinimumSpanningTree { get; private set; }
		public void Run(Graph<BoruvkaVertex> graph, BoruvkaVertex initialVertex)
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
		public Graph CreateGraph(List<MainWindow.EllipseVertex> vertices, List<MainWindow.ArrowEdge> edges)
		{
			Graph<BoruvkaVertex> graph = new Graph<BoruvkaVertex>();
			foreach (var vertex in vertices)
			{
				graph.AddVertex(new BoruvkaVertex(vertex.Name));
			}
			foreach (var edge in edges)
			{
				graph.AddEdge(graph.GetVertex(edge.FromVertex.Name), graph.GetVertex(edge.ToVertex.Name), edge.Length);
			}
			return graph;
		}

	}
}
