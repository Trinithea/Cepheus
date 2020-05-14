using System;
using System.Collections.Generic;
using System.Text;
using Cepheus.DataStructures;
using CepheusProjectWpf;

namespace Cepheus
{
	public class Jarnik : IAlgorithm
	{
		public void Accept(Visitor visitor)
		{
			visitor.Visit(this);
		}
		//TODO Jarník je na neorientovnaý grafy
		public string Name => "Jarnik's algorithm";

		public string TimeComplexity => "O(m * log(n))";
		Tree<JarnikVertex> MinimumSpanningTree;
		public void Run(Graph<JarnikVertex> graph, JarnikVertex initialVertex)
		{
			graph.InitializeVertices();

			MinimumSpanningTree = new Tree<JarnikVertex>();
			initialVertex.State = JarnikVertex.States.Neighbour;
			initialVertex.Rating = 0;

			SortedList<int?, JarnikVertex> neighbours = new SortedList<int?, JarnikVertex>();
			neighbours.Add(initialVertex.Rating,initialVertex);

			while(neighbours.Count > 0)
			{
				var vertex = neighbours[neighbours.Keys[0]]; // neighbour with minimal rating
				vertex.State = JarnikVertex.States.Inside;
				neighbours.RemoveAt(0);
				if (vertex.Predecessor != null)
					MinimumSpanningTree.Edges.Add(graph.GetEdge(vertex.Predecessor, vertex));

				foreach(Edge<JarnikVertex> edge in vertex.OutEdges)
				{
					if((edge.To.State == JarnikVertex.States.Neighbour || edge.To.State == JarnikVertex.States.Outside)
						&& (edge.To.Rating == null || edge.To.Rating > edge.Length))
					{
						edge.To.State = JarnikVertex.States.Neighbour;
						edge.To.Rating = edge.Length;
						edge.To.Predecessor = vertex;
						if(!neighbours.ContainsValue(edge.To))
							neighbours.Add(edge.To.Rating, edge.To);
					}
				}	

			}
		}

		public Tree<JarnikVertex> GetMinimumSpan() => MinimumSpanningTree; //TODO can't be null
		public int GetWeightOfMinimumSpan()
		{
			int sum = 0;
			foreach (Edge<JarnikVertex> edge in MinimumSpanningTree.Edges)
				sum += edge.Length;
			return sum;
		} //TODO do this in datastructure

		public Graph CreateGraph(List<MainWindow.EllipseVertex> vertices, List<MainWindow.ArrowEdge> edges)
		{
			Graph<JarnikVertex> graph = new Graph<JarnikVertex>();
			foreach (var vertex in vertices)
			{
				graph.AddVertex(new JarnikVertex(vertex.Name));
			}
			foreach (var edge in edges)
			{
				graph.AddEdge(graph.GetVertex(edge.FromVertex.Name), graph.GetVertex(edge.ToVertex.Name), edge.Length);
			}
			return graph;
		}
	}
}
