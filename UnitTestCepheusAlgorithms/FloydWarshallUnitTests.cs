using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cepheus;

namespace UnitTestCepheusAlgorithms
{
	[TestClass]
	public class FloydWarshallUnitTests
	{
		Floyd_Warshall fw = new Floyd_Warshall();
		Graph<FloydWarschallVertex> InitializeGraph()
		{
			Graph<FloydWarschallVertex> graph = new Graph<FloydWarschallVertex>();
			graph.AddVertex(new FloydWarschallVertex("A"));
			graph.AddVertex(new FloydWarschallVertex("B"));
			graph.AddVertex(new FloydWarschallVertex("C"));
			graph.AddVertex(new FloydWarschallVertex("D"));
			graph.AddVertex(new FloydWarschallVertex("E"));
			graph.AddVertex(new FloydWarschallVertex("F"));
			graph.AddVertex(new FloydWarschallVertex("G"));

			graph.AddEdgeWithLength("ab", graph.GetVertex("A"), graph.GetVertex("B"), 7);
			graph.AddEdgeWithLength("bc", graph.GetVertex("B"), graph.GetVertex("C"), -3);
			graph.AddEdgeWithLength("ad", graph.GetVertex("A"), graph.GetVertex("D"), 6);
			graph.AddEdgeWithLength("ae", graph.GetVertex("A"), graph.GetVertex("E"), 4);
			graph.AddEdgeWithLength("ef", graph.GetVertex("E"), graph.GetVertex("F"), -5);
			graph.AddEdgeWithLength("fc", graph.GetVertex("F"), graph.GetVertex("C"), 2);
			graph.AddEdgeWithLength("gc", graph.GetVertex("G"), graph.GetVertex("C"), 1);
			graph.AddEdgeWithLength("bf", graph.GetVertex("B"), graph.GetVertex("F"), 1);
			return graph;
		}

		[TestMethod]
		public void GetMinimum()
		{
			Assert.AreEqual(-3,fw.GetMinimum(-3, null));
			Assert.AreEqual(-3, fw.GetMinimum(-3, 7));
			Assert.AreEqual(null, fw.GetMinimum(null, null));
			Assert.AreEqual(-3, fw.GetMinimum(-3, 7));
		}

		[TestMethod]
		public void AddDistances()
		{
			Assert.AreEqual(-1, fw.AddDistances(-5, 4));
			Assert.AreEqual(null, fw.AddDistances(-3, null));
			Assert.AreEqual(null, fw.AddDistances(null, 7));
			Assert.AreEqual(null, fw.AddDistances(null, null));
		}

		[TestMethod]
		public void Run_Distances()
		{
			var graph = InitializeGraph();

			Assert.AreEqual(-3, fw.GetDistance(graph, "E","C")); // from E to C
			Assert.AreEqual(1, fw.GetDistance(graph, "A", "C")); // from A to C
			Assert.AreEqual(null, fw.GetDistance(graph, "A", "G")); // from A to G
			Assert.AreEqual(-3, fw.GetDistance(graph, "B", "C")); // from B to C
			//TODO retrun matrix in fw
		}
	}
}
