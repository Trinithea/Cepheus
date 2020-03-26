using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cepheus;

namespace UnitTestCepheusAlgorithms
{
	[TestClass]
	public class RelaxationUnitTests
	{
		Relaxation relaxation = new Relaxation();
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
		public void Run_Distances()
		{
			var graph = InitializeGraph();

			relaxation.Run(graph, graph.GetVertex("A"));

			Assert.AreEqual(1, graph.GetVertex("C").Distance);
			Assert.AreEqual(-1, graph.GetVertex("F").Distance);
			Assert.AreEqual(null, graph.GetVertex("G").Distance);
			Assert.AreEqual(6, graph.GetVertex("D").Distance);

		}

	}
}
