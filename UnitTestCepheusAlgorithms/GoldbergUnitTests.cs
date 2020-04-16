using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cepheus;

namespace UnitTestCepheusAlgorithms
{
	[TestClass]
	public class GoldbergUnitTests
	{
		Goldberg goldberg = new Goldberg();
		FlowNetwork<GoldbergVertex> CreateNetwork()
		{
			var source = new GoldbergVertex("Z");
			var sink = new GoldbergVertex("S");
			var graph = new FlowNetwork<GoldbergVertex>(source, sink);
			List<string> verticesNames = new List<string>() { "A", "B", "C", "D" };
			for (int i = 0; i < verticesNames.Count; i++)
				graph.AddVertex(new GoldbergVertex(verticesNames[i]));
			graph.AddVertex(source);
			graph.AddVertex(sink);

			var edgesNames = new List<string>() { "Z", "A", "A", "C", "C", "S", "Z", "B", "B", "D", "A", "D", "D", "S", "B", "C" };
			var capacities = new List<int>() { 10, 7, 10, 10, 3, 5, 10, 9 };

			for (int i = 0; i < capacities.Count; i++)
				graph.AddEdge(edgesNames[2 * i] + edgesNames[2 * i + 1], graph.GetVertex(edgesNames[2 * i]), graph.GetVertex(edgesNames[2 * i + 1]), capacities[i]);

			return graph;
		}

		[TestMethod]
		public void Run_MaximalFlow()
		{
			var graph = CreateNetwork();
			goldberg.Run(graph, graph.Source);

			Assert.AreEqual(18, goldberg.MaximalFlow);
		}
		//TODO more tests
	}
}
