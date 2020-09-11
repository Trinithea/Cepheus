using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf;

namespace Cepheus
{
	public class BellmanFord : Algorithm<BfsVertex>
	{
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override string Name => "Bellman-Ford's algorithm";
		public override string TimeComplexity => "O(n * m)";

		public override string Description => "The Bellman–Ford algorithm is an algorithm that computes shortest paths from a single source vertex to all of the other vertices in a weighted digraph. It is slower than Dijkstra's algorithm for the same problem, but more versatile, as it is capable of handling graphs in which some of the edge weights are negative numbers.";

		public async Task Run()
		{
			Graph.InitializeVertices();
			PrintVerticesInitialized(Graph);

			initialVertex.State = States.Open;

			initialVertex.Distance = 0;
			PrintVertex(initialVertex);
			Queue<BfsVertex> openVertices = new Queue<BfsVertex>();

			openVertices.Enqueue(initialVertex);
			PrintQueued(initialVertex);
			while (openVertices.Count > 0)
			{
				var vertex = openVertices.Dequeue(); // some open vertex
				PrintDequeued(vertex);
				foreach (Edge<BfsVertex> edge in vertex.OutEdges)
				{
					
					if (edge.To.Distance == Int32.MaxValue || edge.To.Distance > (vertex.Distance + edge.Length))
					{
						ColorEdge(edge);
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = States.Open;
						openVertices.Enqueue(edge.To);
						await Task.Delay(delay - 250);
						ColorVertex(edge.To);
						PrintVertex(edge.To);
						PrintQueued(edge.To);
						edge.To.Predecessor = vertex;
					}
					await Task.Delay(delay);
				}
				vertex.State = States.Closed;
				UncolorVertex(vertex);
				PrintVertex(vertex);
				foreach (var edge in vertex.OutEdges)
					UncolorEdge(edge);
				await Task.Delay(delay);
			}
		}


	}
}
