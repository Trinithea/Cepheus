using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cepheus;
using Cepheus.DataStructures;

namespace UnitTestCepheusAlgorithms
{
	[TestClass]
	public class JarnikUnitTests
	{
		Jarnik jarnik = new Jarnik();
		Graph<JarnikVertex> CreateGraph()
		{
			Graph<JarnikVertex> graph = new Graph<JarnikVertex>();

			graph.AddVertex(new JarnikVertex("A"));
			graph.AddVertex(new JarnikVertex("B"));
			graph.AddVertex(new JarnikVertex("C"));
			graph.AddVertex(new JarnikVertex("D"));
			graph.AddVertex(new JarnikVertex("E"));
			graph.AddVertex(new JarnikVertex("F"));
			graph.AddVertex(new JarnikVertex("G"));
			graph.AddVertex(new JarnikVertex("H"));
			graph.AddVertex(new JarnikVertex("I"));


			//not oriented
			graph.AddEdgeWithLength("AB", graph.GetVertex("A"), graph.GetVertex("B"), 10);
			graph.AddEdgeWithLength("BA", graph.GetVertex("B"), graph.GetVertex("A"), 10);

			graph.AddEdgeWithLength("BC", graph.GetVertex("B"), graph.GetVertex("C"), 6);
			graph.AddEdgeWithLength("CB", graph.GetVertex("C"), graph.GetVertex("B"), 6);

			graph.AddEdgeWithLength("CF", graph.GetVertex("C"), graph.GetVertex("F"), 2);
			graph.AddEdgeWithLength("FC", graph.GetVertex("F"), graph.GetVertex("C"), 2);

			graph.AddEdgeWithLength("BE", graph.GetVertex("B"), graph.GetVertex("E"), 8);
			graph.AddEdgeWithLength("EB", graph.GetVertex("E"), graph.GetVertex("B"), 8);

			graph.AddEdgeWithLength("AD", graph.GetVertex("A"), graph.GetVertex("D"), 7);
			graph.AddEdgeWithLength("DA", graph.GetVertex("D"), graph.GetVertex("A"), 7);

			graph.AddEdgeWithLength("DE", graph.GetVertex("D"), graph.GetVertex("E"), 5);
			graph.AddEdgeWithLength("ED", graph.GetVertex("E"), graph.GetVertex("D"), 5);

			graph.AddEdgeWithLength("EF", graph.GetVertex("E"), graph.GetVertex("F"), 4);
			graph.AddEdgeWithLength("FE", graph.GetVertex("F"), graph.GetVertex("E"), 4);

			graph.AddEdgeWithLength("DG", graph.GetVertex("D"), graph.GetVertex("G"), 0);
			graph.AddEdgeWithLength("GD", graph.GetVertex("G"), graph.GetVertex("D"), 0);

			graph.AddEdgeWithLength("EH", graph.GetVertex("E"), graph.GetVertex("H"), 3);
			graph.AddEdgeWithLength("HE", graph.GetVertex("H"), graph.GetVertex("E"), 3);

			graph.AddEdgeWithLength("FI", graph.GetVertex("F"), graph.GetVertex("I"), 9);
			graph.AddEdgeWithLength("IF", graph.GetVertex("I"), graph.GetVertex("F"), 9);

			graph.AddEdgeWithLength("GH", graph.GetVertex("G"), graph.GetVertex("H"), 1);
			graph.AddEdgeWithLength("HG", graph.GetVertex("H"), graph.GetVertex("G"), 1);

			graph.AddEdgeWithLength("HI", graph.GetVertex("H"), graph.GetVertex("I"), 11);
			graph.AddEdgeWithLength("IH", graph.GetVertex("I"), graph.GetVertex("H"), 11);

			return graph;
		}

		[TestMethod]
		public void EdgesInMinimumSpan()
		{
			var graph = CreateGraph();
			
			jarnik.Run(graph, graph.GetVertex("E"));
			var minimumSpan = jarnik.GetMinimumSpan();

			Assert.IsTrue(minimumSpan.Edges.Contains(graph.GetEdge("EH")));
			Assert.IsTrue(minimumSpan.Edges.Contains(graph.GetEdge("HG")));
			Assert.IsTrue(minimumSpan.Edges.Contains(graph.GetEdge("GD")));
			Assert.IsTrue(minimumSpan.Edges.Contains(graph.GetEdge("EF")));
			Assert.IsTrue(minimumSpan.Edges.Contains(graph.GetEdge("FC")));
			Assert.IsTrue(minimumSpan.Edges.Contains(graph.GetEdge("CB")));
			Assert.IsTrue(minimumSpan.Edges.Contains(graph.GetEdge("DA")));
			Assert.IsTrue(minimumSpan.Edges.Contains(graph.GetEdge("FI")));

			Assert.AreEqual(8, minimumSpan.Edges.Count);
		}

		[TestMethod]
		public void WeightOfMinimumSpan()
		{
			var graph = CreateGraph();

			jarnik.Run(graph, graph.GetVertex("E"));

			Assert.AreEqual(32, jarnik.GetWeightOfMinimumSpan());
		}

		[TestMethod]
		public void AllVerticesAreInMinimumSpan()
		{
			var graph = CreateGraph();

			jarnik.Run(graph, graph.GetVertex("E"));
			

			foreach (KeyValuePair<string, JarnikVertex> vertex in graph.Vertices)
			{
				Assert.IsTrue(vertex.Value.State == JarnikVertex.States.Inside);
			}
		}
	}
}
