using System;
using System.Collections.Generic;
using System.Text;
using CepheusProjectWpf;

namespace Cepheus
{
	public class Dijkstra : Algorithm<BfsVertex>
	{
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public override void Accept(VisitorRunner visitor)
		{
			visitor.Visit(this);
		}
		public override string Name => "Dijkstra's algorithm";
		public override string TimeComplexity => "O((n + m) * log(n))";

		public void Run()
		{
			// We can use vertex class for BFS algortihm, Distance property will be considered as rating.

			graph.InitializeVertices();

			initialVertex.State = States.Open;
			initialVertex.Distance = 0;

			SortedList<int?, BfsVertex> openVertices = new SortedList<int?, BfsVertex>();
			openVertices.Add(initialVertex.Distance, initialVertex);
			while(openVertices.Count > 0)
			{
				var vertex = openVertices[openVertices.Keys[0]]; // vertex with minimal rating
				foreach(Edge<BfsVertex> edge in vertex.OutEdges)
				{
					if(edge.To.Distance == null || edge.To.Distance > (vertex.Distance + edge.Length))
					{
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = States.Open;
						openVertices.Add(edge.To.Distance, edge.To);
						edge.To.Predecessor = vertex;
					}
				}
				vertex.State = States.Closed;
				openVertices.RemoveAt(0);
			}
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
