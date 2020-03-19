using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cepheus;

namespace UnitTestCepheusAlgorithms
{
	[TestClass]
	public class BFSUnitTests
	{
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

			graph.AddEdge("ab", graph.GetVertex("A"), graph.GetVertex("B"));
			graph.AddEdge("bc", graph.GetVertex("B"), graph.GetVertex("C"));
			graph.AddEdge("ad", graph.GetVertex("A"), graph.GetVertex("D"));
			graph.AddEdge("ae", graph.GetVertex("A"), graph.GetVertex("E"));
			graph.AddEdge("ef", graph.GetVertex("E"), graph.GetVertex("F"));
			graph.AddEdge("fc", graph.GetVertex("F"), graph.GetVertex("C"));
			graph.AddEdge("gc", graph.GetVertex("G"), graph.GetVertex("C"));
			return graph;
		}

		[TestMethod]
		public void Run_Distances()
		{
			var graph = InitializeGraph();

			BFS.Run(graph, graph.GetVertex("A"));

			Assert.AreEqual(2, graph.GetVertex("C").Distance);
			Assert.AreEqual(0, graph.GetVertex("A").Distance);
			Assert.AreEqual(2, graph.GetVertex("F").Distance);
			Assert.AreEqual(1, graph.GetVertex("D").Distance);
		}

		[TestMethod]
		public void Run_Predecessors()
		{
			var graph = InitializeGraph();

			BFS.Run(graph, graph.GetVertex("A"));

			Assert.AreEqual(graph.GetVertex("B"), graph.GetVertex("C").Predecessor);
			Assert.AreEqual(null, graph.GetVertex("A").Predecessor);
			Assert.AreEqual(graph.GetVertex("E"), graph.GetVertex("F").Predecessor);
			Assert.AreEqual(null, graph.GetVertex("G").Predecessor);
		}

		[TestMethod]
		public void Run_States()
		{
			var graph = InitializeGraph();

			BFS.Run(graph, graph.GetVertex("A"));

			Assert.AreEqual(IStateVertex.States.Closed, graph.GetVertex("A").State);
			Assert.AreEqual(IStateVertex.States.Closed, graph.GetVertex("B").State);
			Assert.AreEqual(IStateVertex.States.Closed, graph.GetVertex("C").State);
			Assert.AreEqual(IStateVertex.States.Closed, graph.GetVertex("D").State);
			Assert.AreEqual(IStateVertex.States.Closed, graph.GetVertex("E").State);
			Assert.AreEqual(IStateVertex.States.Closed, graph.GetVertex("F").State);

			Assert.AreEqual(IStateVertex.States.Unvisited, graph.GetVertex("G").State);
		}
	}

	
}
