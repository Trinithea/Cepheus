using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cepheus.DataStructures;
using CepheusProjectWpf;

namespace Cepheus
{
	public class Jarnik : Algorithm<JarnikVertex>
	{
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		//TODO Jarník je na neorientovnaý grafy
		public override string Name => "Jarnik's algorithm";

		public override string TimeComplexity => "O(m * log(n))";

		public override string Description => "In computer science, Prim's (also known as Jarník's) algorithm is a greedy algorithm that finds a minimum spanning tree for a weighted undirected graph. This means it finds a subset of the edges that forms a tree that includes every vertex, where the total weight of all the edges in the tree is minimized. The algorithm operates by building this tree one vertex at a time, from an arbitrary starting vertex, at each step adding the cheapest possible connection from the tree to another vertex. ";

		Tree<JarnikVertex> MinimumSpanningTree;
		public async Task Run()
		{
			graph.InitializeVertices();
			PrintVerticesInitialized(graph);

			MinimumSpanningTree = new Tree<JarnikVertex>();
			initialVertex.State = JarnikVertex.States.Neighbour;
			initialVertex.Rating = 0;
			PrintVertex(initialVertex);
			ColorVertex(initialVertex);

			SortedList<int?, JarnikVertex> neighbours = new SortedList<int?, JarnikVertex>();
			neighbours.Add(initialVertex.Rating,initialVertex);
			PrintSortedNeighbours(neighbours);

			while(neighbours.Count > 0)
			{
				var vertex = neighbours[neighbours.Keys[0]]; // neighbour with minimal rating
				vertex.State = JarnikVertex.States.Inside;
				neighbours.RemoveAt(0);
				if (vertex.Predecessor != null)
				{
					MinimumSpanningTree.Edges.Add(graph.GetEdge(vertex.Predecessor, vertex));
					ColorVertex(vertex.Predecessor);
					ColorVertex(vertex);
					ColorEdge(graph.GetEdge(vertex.Predecessor, vertex));
				}
					

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
		void PrintSortedNeighbours(SortedList<int?, JarnikVertex> neighbours)
		{
			outputConsole.Text += "\nSorted neighbours are (rating is in parenthesses): ";
			foreach (var neighbour in neighbours)
				outputConsole.Text += neighbour.Value.Name + " (" + neighbour.Key + "), ";
		}

		public Tree<JarnikVertex> GetMinimumSpan() => MinimumSpanningTree; //TODO can't be null
		public int GetWeightOfMinimumSpan()
		{
			int sum = 0;
			foreach (Edge<JarnikVertex> edge in MinimumSpanningTree.Edges)
				sum += edge.Length;
			return sum;
		} //TODO do this in datastructure

	}
}
