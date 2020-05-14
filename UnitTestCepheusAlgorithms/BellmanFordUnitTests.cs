using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cepheus;

namespace UnitTestCepheusAlgorithms
{
	[TestClass]
	public class BellmanFordUnitTests
	{
		Bellman_Ford bf = new Bellman_Ford();
		Graph<BfsVertex> InitializeGraph()
		{
			Graph<BfsVertex> graph = new Graph<BfsVertex>();
			graph.AddVertex(new BfsVertex("A"));
			graph.AddVertex(new BfsVertex("B"));
			graph.AddVertex(new BfsVertex("C"));
			graph.AddVertex(new BfsVertex("D"));
			graph.AddVertex(new BfsVertex("E"));
			graph.AddVertex(new BfsVertex("F"));
			graph.AddVertex(new BfsVertex("G"));

			graph.AddEdge(graph.GetVertex("A"), graph.GetVertex("B"), 7);
			graph.AddEdge(graph.GetVertex("B"), graph.GetVertex("C"), -3);
			graph.AddEdge(graph.GetVertex("A"), graph.GetVertex("D"), 6);
			graph.AddEdge(graph.GetVertex("A"), graph.GetVertex("E"), 4);
			graph.AddEdge( graph.GetVertex("E"), graph.GetVertex("F"), -5);
			graph.AddEdge( graph.GetVertex("F"), graph.GetVertex("C"), 2);
			graph.AddEdge( graph.GetVertex("G"), graph.GetVertex("C"), 1);
			graph.AddEdge(graph.GetVertex("B"), graph.GetVertex("F"), 1);
			return graph;
		}

		[TestMethod]
		public void Run_Distances()
		{
			var graph = InitializeGraph();

			bf.Run(graph, graph.GetVertex("A"));

			Assert.AreEqual(1, graph.GetVertex("C").Distance);
			Assert.AreEqual(-1, graph.GetVertex("F").Distance);
			Assert.AreEqual(null, graph.GetVertex("G").Distance);
			Assert.AreEqual(6, graph.GetVertex("D").Distance);

		}
	}
}
