using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cepheus;

namespace UnitTestCepheusAlgorithms
{
	[TestClass]
	public class DijkstraUnitTests
	{
		Dijkstra dijkstra = new Dijkstra();
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

			graph.AddEdge( graph.GetVertex("A"), graph.GetVertex("B"),7);
			graph.AddEdge( graph.GetVertex("B"), graph.GetVertex("C"),3);
			graph.AddEdge(graph.GetVertex("A"), graph.GetVertex("D"),6);
			graph.AddEdge( graph.GetVertex("A"), graph.GetVertex("E"),4);
			graph.AddEdge( graph.GetVertex("E"), graph.GetVertex("F"),5);
			graph.AddEdge( graph.GetVertex("F"), graph.GetVertex("C"),2);
			graph.AddEdge(graph.GetVertex("G"), graph.GetVertex("C"),1);
			graph.AddEdge( graph.GetVertex("B"), graph.GetVertex("F"), 1);
			return graph;
		}

		[TestMethod]
		public void Run_Distances()
		{
			var graph = InitializeGraph();

			dijkstra.Run(graph, graph.GetVertex("A"));

			Assert.AreEqual(10, graph.GetVertex("C").Distance);
			Assert.AreEqual(8, graph.GetVertex("F").Distance);
			Assert.AreEqual(null, graph.GetVertex("G").Distance);
			Assert.AreEqual(6, graph.GetVertex("D").Distance);
			
		}

		[TestMethod]
		public void Run_ShortestPaths()
		{
			var graph = InitializeGraph();

			Assert.AreEqual(3, dijkstra.LengthOfShortestPathFromTo(graph, graph.GetVertex("B"), graph.GetVertex("C")));
			Assert.AreEqual(7, dijkstra.LengthOfShortestPathFromTo(graph, graph.GetVertex("E"), graph.GetVertex("C")));
			Assert.AreEqual(null, dijkstra.LengthOfShortestPathFromTo(graph, graph.GetVertex("C"), graph.GetVertex("B")));
		}
	}
}
