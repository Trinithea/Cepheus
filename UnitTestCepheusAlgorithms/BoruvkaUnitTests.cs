using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cepheus;
using Cepheus.DataStructures;

namespace UnitTestCepheusAlgorithms
{
	[TestClass]
	public class BoruvkaUnitTests
	{
		Boruvka boruvkaAlgorithm = new Boruvka();
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
		public void FindLightestEdge_fromInitialVertices() //TODO is Initial correct?
		{
			var graph = CreateGraph();
			graph.InitializeVertices(); // to get OutEdges sorted

			TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree = new TreeWithContextComponents<BoruvkaVertex>();
			minimumSpanningTree.Vertices = new List<BoruvkaVertex>(graph.Vertices.Values);
			boruvkaAlgorithm.graph = graph;
			List<int> ids = new List<int>();
			boruvkaAlgorithm.Initialize(minimumSpanningTree,ids);

			Assert.AreEqual(graph.GetEdge("AD"),boruvkaAlgorithm.FindLightestEdgeFromComponent(minimumSpanningTree, minimumSpanningTree.ContextComponents[0]));
			Assert.AreEqual(graph.GetEdge("BC"), boruvkaAlgorithm.FindLightestEdgeFromComponent(minimumSpanningTree, minimumSpanningTree.ContextComponents[1]));
			Assert.AreEqual(graph.GetEdge("CF"), boruvkaAlgorithm.FindLightestEdgeFromComponent(minimumSpanningTree, minimumSpanningTree.ContextComponents[2]));
			Assert.AreEqual(graph.GetEdge("DG"), boruvkaAlgorithm.FindLightestEdgeFromComponent(minimumSpanningTree, minimumSpanningTree.ContextComponents[3]));
			Assert.AreEqual(graph.GetEdge("EH"), boruvkaAlgorithm.FindLightestEdgeFromComponent(minimumSpanningTree, minimumSpanningTree.ContextComponents[4]));
			Assert.AreEqual(graph.GetEdge("HG"), boruvkaAlgorithm.FindLightestEdgeFromComponent(minimumSpanningTree, minimumSpanningTree.ContextComponents[7]));
			Assert.AreEqual(graph.GetEdge("IF"), boruvkaAlgorithm.FindLightestEdgeFromComponent(minimumSpanningTree, minimumSpanningTree.ContextComponents[8]));
		}

		void CreateComponents(Graph<BoruvkaVertex> graph, TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree,List<int> ids)
		{
			minimumSpanningTree.ContextComponents.Add(0,new ComponentTree<BoruvkaVertex>());
			minimumSpanningTree.ContextComponents.Add(1,new ComponentTree<BoruvkaVertex>());
			ids.Add(0);
			ids.Add(1);
			//First component
			//Edges
			List<string> namesEdges = new List<string>() { "AD", "DG", "GD", "HG", "EH" };
			for (int i = 0; i <namesEdges.Count; i++)
				minimumSpanningTree.ContextComponents[0].Edges.Add(graph.GetEdge(namesEdges[i]));
			//Vertices
			List<string> names = new List<string>() { "A", "D", "G", "E", "H" };
			for (int i = 0; i <names.Count; i++)
			{
				graph.GetVertex(names[i]).ComponentID = 0;
				minimumSpanningTree.ContextComponents[0].Vertices.Add(graph.GetVertex(names[i]));
			}
				

			//Second component
			//Edges
			List<string> namesEdges2 = new List<string>() { "BC", "CF", "FC", "IF" };
			for(int i=0;i<namesEdges2.Count;i++)
				minimumSpanningTree.ContextComponents[1].Edges.Add(graph.GetEdge(namesEdges2[i]));
			//Vertices
			List<string> names2 = new List<string>() { "B", "C", "F", "I" };
			for (int i = 0; i < names2.Count; i++)
			{
				graph.GetVertex(names2[i]).ComponentID = 1;
				minimumSpanningTree.ContextComponents[1].Vertices.Add(graph.GetVertex(names2[i]));
			}

			graph.GetVertex("A").OutEdges.Remove(graph.GetEdge("AD"));
			graph.GetVertex("B").OutEdges.Remove(graph.GetEdge("BC"));
			graph.GetVertex("C").OutEdges.Remove(graph.GetEdge("CB"));
			graph.GetVertex("C").OutEdges.Remove(graph.GetEdge("CF"));
			graph.GetVertex("D").OutEdges.Remove(graph.GetEdge("DA"));
			graph.GetVertex("D").OutEdges.Remove(graph.GetEdge("DG"));
			graph.GetVertex("E").OutEdges.Remove(graph.GetEdge("EH"));
			graph.GetVertex("F").OutEdges.Remove(graph.GetEdge("FC"));
			graph.GetVertex("F").OutEdges.Remove(graph.GetEdge("FI"));
			graph.GetVertex("G").OutEdges.Remove(graph.GetEdge("GD"));
			graph.GetVertex("G").OutEdges.Remove(graph.GetEdge("GH"));
			graph.GetVertex("H").OutEdges.Remove(graph.GetEdge("HE"));
			graph.GetVertex("H").OutEdges.Remove(graph.GetEdge("HG"));
			graph.GetVertex("I").OutEdges.Remove(graph.GetEdge("IF"));

			minimumSpanningTree.Edges.Add("AD",(EdgeWithLength<BoruvkaVertex>)graph.GetEdge("AD"));
			minimumSpanningTree.Edges.Add("DG", (EdgeWithLength<BoruvkaVertex>)graph.GetEdge("DG"));
			minimumSpanningTree.Edges.Add("GD", (EdgeWithLength<BoruvkaVertex>)graph.GetEdge("GD"));
			minimumSpanningTree.Edges.Add("HG", (EdgeWithLength<BoruvkaVertex>)graph.GetEdge("HG"));
			minimumSpanningTree.Edges.Add("EH", (EdgeWithLength<BoruvkaVertex>)graph.GetEdge("EH"));
			minimumSpanningTree.Edges.Add("BC", (EdgeWithLength<BoruvkaVertex>)graph.GetEdge("BC"));
			minimumSpanningTree.Edges.Add("CF", (EdgeWithLength<BoruvkaVertex>)graph.GetEdge("CF"));
			minimumSpanningTree.Edges.Add("FC", (EdgeWithLength<BoruvkaVertex>)graph.GetEdge("FC"));
			minimumSpanningTree.Edges.Add("IF", (EdgeWithLength<BoruvkaVertex>)graph.GetEdge("IF"));

		}

		[TestMethod]
		public void FindLightestEdge_fromComponent() 
		{
			var graph = CreateGraph();
			graph.InitializeVertices(); // to get OutEdges sorted
			boruvkaAlgorithm.graph = graph;
			TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree = new TreeWithContextComponents<BoruvkaVertex>();
			minimumSpanningTree.Vertices = new List<BoruvkaVertex>(graph.Vertices.Values);
			List<int> ids = new List<int>();
			CreateComponents(graph, minimumSpanningTree,ids);

			Assert.AreEqual(graph.GetEdge("EF"), boruvkaAlgorithm.FindLightestEdgeFromComponent(minimumSpanningTree, minimumSpanningTree.ContextComponents[0]));
			// from second component it is the same edge, only in opposite direction, which has been removed already
			
		}

		[TestMethod]
		public void MergeContextComponents_TwoComponents()
		{
			var graph = CreateGraph();
			graph.InitializeVertices(); // to get OutEdges sorted
			boruvkaAlgorithm.graph = graph;
			TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree = new TreeWithContextComponents<BoruvkaVertex>();
			minimumSpanningTree.Vertices = new List<BoruvkaVertex>(graph.Vertices.Values);
			List<int> ids = new List<int>();
			CreateComponents(graph, minimumSpanningTree,ids);

			minimumSpanningTree.NewEdges.Add((EdgeWithLength<BoruvkaVertex>)graph.GetEdge("EF")); // new lightest edge
			
			boruvkaAlgorithm.MergeContextComponents(minimumSpanningTree,ids);

			Assert.AreEqual(1, minimumSpanningTree.ContextComponents.Count);
		}

		[TestMethod]
		public void MergeContextComponents_NineComponents()
		{
			var graph = CreateGraph();
			graph.InitializeVertices(); // to get OutEdges sorted
			boruvkaAlgorithm.graph = graph;
			TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree = new TreeWithContextComponents<BoruvkaVertex>();
			minimumSpanningTree.Vertices = new List<BoruvkaVertex>(graph.Vertices.Values);
			List<int> ids = new List<int>();
			boruvkaAlgorithm.Initialize(minimumSpanningTree,ids); // each vertex is a context component

			List<string> newEdges = new List<string>() { "AD", "DG", "HG", "EH", "BC", "CF", "IF" };
			for (int i = 0; i < newEdges.Count; i++)
				minimumSpanningTree.NewEdges.Add((EdgeWithLength<BoruvkaVertex>)graph.GetEdge(newEdges[i]));

			
			boruvkaAlgorithm.MergeContextComponents(minimumSpanningTree,ids);

			Assert.AreEqual(2, minimumSpanningTree.ContextComponents.Count);
		}
		
		[TestMethod]
		public void Run_MinimumSpanningTree()
		{
			var graph = CreateGraph();
			graph.InitializeVertices();

			boruvkaAlgorithm.Run(graph, graph.GetVertex("A"));
			var mst = boruvkaAlgorithm.MinimumSpanningTree;

			Assert.AreEqual(9, mst.Vertices.Count);
			Assert.AreEqual(8, mst.Edges.Count);
			Assert.AreEqual(1, mst.ContextComponents.Count);
			Assert.AreEqual(0, mst.NewEdges.Count);
			Assert.AreEqual(32, mst.GetWeight());
		}
	}
}
