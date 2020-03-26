using System;

namespace Cepheus
{
	class Program
	{
		static void Main(string[] args)
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

			Floyd_Warshall fw = new Floyd_Warshall();

			fw.Run(graph, graph.GetVertex("A"));
		}
	}
}
