using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cepheus;

namespace UnitTestCepheusAlgorithms
{
	[TestClass]
	public class KruskalUnitTests
	{
		Kruskal kruskal = new Kruskal();
		Graph<BoruvkaVertex> CreateGraph()
		{
			Graph<BoruvkaVertex> graph = new Graph<BoruvkaVertex>();

			graph.AddVertex(new BoruvkaVertex("A"));
			graph.AddVertex(new BoruvkaVertex("B"));
			graph.AddVertex(new BoruvkaVertex("C"));
			graph.AddVertex(new BoruvkaVertex("D"));
			graph.AddVertex(new BoruvkaVertex("E"));
			graph.AddVertex(new BoruvkaVertex("F"));
			graph.AddVertex(new BoruvkaVertex("G"));
			graph.AddVertex(new BoruvkaVertex("H"));
			graph.AddVertex(new BoruvkaVertex("I"));


			//not oriented
			graph.AddEdge(graph.GetVertex("A"), graph.GetVertex("B"), 10);

			graph.AddEdge(graph.GetVertex("B"), graph.GetVertex("C"), 6);

			graph.AddEdge(graph.GetVertex("C"), graph.GetVertex("F"), 2);

			graph.AddEdge(graph.GetVertex("B"), graph.GetVertex("E"), 8);

			graph.AddEdge( graph.GetVertex("A"), graph.GetVertex("D"), 7);

			graph.AddEdge(graph.GetVertex("D"), graph.GetVertex("E"), 5);

			graph.AddEdge(graph.GetVertex("E"), graph.GetVertex("F"), 4);

			graph.AddEdge( graph.GetVertex("D"), graph.GetVertex("G"), 0);

			graph.AddEdge( graph.GetVertex("E"), graph.GetVertex("H"), 3);

			graph.AddEdge(graph.GetVertex("F"), graph.GetVertex("I"), 9);

			graph.AddEdge(graph.GetVertex("G"), graph.GetVertex("H"), 1);

			graph.AddEdge(graph.GetVertex("H"), graph.GetVertex("I"), 11);

			return graph;
		}

		[TestMethod]
		public void Run()
		{
			var graph = CreateGraph();

			kruskal.Run(graph, graph.GetVertex("A"));

			var mst = kruskal.MinimumSpanningTree;

			Assert.AreEqual(9, mst.Vertices.Count);
			Assert.AreEqual(8, mst.Edges.Count);
			Assert.AreEqual(1, mst.ContextComponents.Count);
			Assert.AreEqual(0, mst.NewEdges.Count);
			Assert.AreEqual(32, mst.GetWeight());
		}
	}
}
