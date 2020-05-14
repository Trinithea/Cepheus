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
		Graph<FloydWarshallVertex> InitializeGraph()
		{
			Graph<FloydWarshallVertex> graph = new Graph<FloydWarshallVertex>();
			graph.AddVertex(new FloydWarshallVertex("A"));
			graph.AddVertex(new FloydWarshallVertex("B"));
			graph.AddVertex(new FloydWarshallVertex("C"));
			graph.AddVertex(new FloydWarshallVertex("D"));
			graph.AddVertex(new FloydWarshallVertex("E"));
			graph.AddVertex(new FloydWarshallVertex("F"));
			graph.AddVertex(new FloydWarshallVertex("G"));

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
		public void GetMinimum()
		{
			Assert.AreEqual(-3,fw.GetMinimum(-3, null));
			Assert.AreEqual(-3, fw.GetMinimum(-3, 7));
			Assert.AreEqual(null, fw.GetMinimum(null, null));
			Assert.AreEqual(-3, fw.GetMinimum(-3, 7));
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
