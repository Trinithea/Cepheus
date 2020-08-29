using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf;

namespace Cepheus
{
	public class Dijkstra : Algorithm<BfsVertex>
	{
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override string Name => "Dijkstra's algorithm";
		public override string TimeComplexity => "O((n + m) * log(n))";

		public override string Description => "Dijkstra's algorithm (or Dijkstra's Shortest Path First algorithm, SPF algorithm) is an algorithm for finding the shortest paths between nodes in a graph, which may represent, for example, road networks.";

		public async Task Run()
		{
			// We can use vertex class for BFS algortihm, Distance property will be considered as rating.

			graph.InitializeVertices();
			PrintVerticesInitialized(graph);

			initialVertex.State = States.Open;
			initialVertex.Distance = 0;
			PrintVertex(initialVertex);
			ColorVertex(initialVertex);

			SortedList<int?, BfsVertex> openVertices = new SortedList<int?, BfsVertex>();
			openVertices.Add(initialVertex.Distance, initialVertex);
			while(openVertices.Count > 0)
			{
				var vertex = openVertices[openVertices.Keys[0]]; // vertex with minimal rating
				
				foreach(Edge<BfsVertex> edge in vertex.OutEdges)
				{
					
					if(edge.To.Distance == null || edge.To.Distance > (vertex.Distance + edge.Length))
					{
						ColorEdge(edge);
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = States.Open;
						await Task.Delay(delay - 250);
						ColorVertex(edge.To);
						PrintVertex(edge.To);
						openVertices.Add(edge.To.Distance, edge.To);
						edge.To.Predecessor = vertex;
					}
					await Task.Delay(delay);
				}
				vertex.State = States.Closed;
				UncolorVertex(vertex);
				PrintVertex(vertex);
				openVertices.RemoveAt(0);
				PrintOpenvertices(openVertices);
				foreach (var edge in vertex.OutEdges)
					UncolorEdge(edge);
				await Task.Delay(delay);
			}
		}

		void PrintOpenvertices(SortedList<int?, BfsVertex> openVertices)
		{
			outputConsole.Text += "\nOpen vertices in sorted order (distance is in brackets): ";
			foreach(var vertex in openVertices)
				outputConsole.Text += String.Format("{0} ({1}), ", vertex.Value.Name, vertex.Key);
		}

		//TODO why is this here?
		//public int? LengthOfShortestPathFromTo(Graph<BfsVertex> graph, BfsVertex from, BfsVertex to)
		//{

		//	Run(graph, from); 

		//	var currentVertex = to;
		//	int length = 0;

		//	while(currentVertex.Predecessor != null)
		//	{
		//		length += (graph.GetEdge(currentVertex.Predecessor, currentVertex)).Length;
		//		currentVertex = currentVertex.Predecessor;
		//	}

		//	if (currentVertex == from)
		//		return length;
		//	else
		//		return null;
		//}

	}
}
