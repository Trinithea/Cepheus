using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cepheus;

namespace UnitTestCepheusAlgorithms
{
	

	[TestClass]
	public class DFSUnitTests
	{
		DFS dfs = new DFS();
		Graph<DfsVertex> InitializeGraph()
		{
			Graph<DfsVertex> graph = new Graph<DfsVertex>();
			graph.AddVertex(new DfsVertex("0"));
			graph.AddVertex(new DfsVertex("1"));
			graph.AddVertex(new DfsVertex("2"));
			graph.AddVertex(new DfsVertex("3"));
			graph.AddVertex(new DfsVertex("4"));
			graph.AddVertex(new DfsVertex("5"));
			graph.AddVertex(new DfsVertex("6"));
			graph.AddVertex(new DfsVertex("7"));
			graph.AddVertex(new DfsVertex("8"));
			graph.AddVertex(new DfsVertex("9"));

			graph.AddEdge("01", graph.GetVertex("0"), graph.GetVertex("1"),1); ;
			graph.AddEdge("12", graph.GetVertex("1"), graph.GetVertex("2"), 1);
			graph.AddEdge("13", graph.GetVertex("1"), graph.GetVertex("3"), 1);
			graph.AddEdge("34", graph.GetVertex("3"), graph.GetVertex("4"), 1);
			graph.AddEdge("53", graph.GetVertex("5"), graph.GetVertex("3"), 1);
			graph.AddEdge("03", graph.GetVertex("0"), graph.GetVertex("3"), 1);
			graph.AddEdge("63", graph.GetVertex("6"), graph.GetVertex("3"), 1);
			graph.AddEdge("60", graph.GetVertex("6"), graph.GetVertex("0"), 1);
			graph.AddEdge("67", graph.GetVertex("6"), graph.GetVertex("7"), 1);
			graph.AddEdge("86", graph.GetVertex("8"), graph.GetVertex("6"), 1);
			graph.AddEdge("09", graph.GetVertex("0"), graph.GetVertex("9"), 1);
			graph.AddEdge("96", graph.GetVertex("9"), graph.GetVertex("6"), 1);

			return graph;
		}

		[TestMethod]
		public void Run_InTime()
		{
			Graph<DfsVertex> graph = InitializeGraph();

			dfs.Run(graph, graph.GetVertex("0"));

			Assert.AreEqual(1, graph.GetVertex("0").InTime);
			Assert.AreEqual(2, graph.GetVertex("1").InTime);
			Assert.AreEqual(3, graph.GetVertex("2").InTime);
			Assert.AreEqual(5, graph.GetVertex("3").InTime);
			Assert.AreEqual(6, graph.GetVertex("4").InTime);
			Assert.AreEqual(null, graph.GetVertex("5").InTime);
			Assert.AreEqual(11, graph.GetVertex("6").InTime);
			Assert.AreEqual(12, graph.GetVertex("7").InTime);
			Assert.AreEqual(null, graph.GetVertex("8").InTime);
			Assert.AreEqual(10, graph.GetVertex("9").InTime);
		}

		[TestMethod]
		public void Run_OutTime()
		{
			Graph<DfsVertex> graph = InitializeGraph();

			dfs.Run(graph, graph.GetVertex("0"));

			Assert.AreEqual(16, graph.GetVertex("0").OutTime);
			Assert.AreEqual(9, graph.GetVertex("1").OutTime);
			Assert.AreEqual(4, graph.GetVertex("2").OutTime);
			Assert.AreEqual(8, graph.GetVertex("3").OutTime);
			Assert.AreEqual(7, graph.GetVertex("4").OutTime);
			Assert.AreEqual(null, graph.GetVertex("5").OutTime);
			Assert.AreEqual(14, graph.GetVertex("6").OutTime);
			Assert.AreEqual(13, graph.GetVertex("7").OutTime);
			Assert.AreEqual(null, graph.GetVertex("8").OutTime);
			Assert.AreEqual(15, graph.GetVertex("9").OutTime);
		}

		[TestMethod]
		public void Run_State()
		{
			Graph<DfsVertex> graph = InitializeGraph();

			dfs.Run(graph, graph.GetVertex("0"));

			Assert.AreEqual(IStateVertex.States.Closed, graph.GetVertex("0").State);
			Assert.AreEqual(IStateVertex.States.Closed, graph.GetVertex("1").State);
			Assert.AreEqual(IStateVertex.States.Closed, graph.GetVertex("2").State);
			Assert.AreEqual(IStateVertex.States.Closed, graph.GetVertex("3").State);
			Assert.AreEqual(IStateVertex.States.Closed, graph.GetVertex("4").State);
			Assert.AreEqual(IStateVertex.States.Unvisited, graph.GetVertex("5").State);
			Assert.AreEqual(IStateVertex.States.Closed, graph.GetVertex("6").State);
			Assert.AreEqual(IStateVertex.States.Closed, graph.GetVertex("7").State);
			Assert.AreEqual(IStateVertex.States.Unvisited, graph.GetVertex("8").State);
			Assert.AreEqual(IStateVertex.States.Closed, graph.GetVertex("9").State);
		}
	}
}
