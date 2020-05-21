using System;
using System.Collections.Generic;
using System.Text;
using CepheusProjectWpf;

namespace Cepheus
{
	public class Bellman_Ford : Algorithm<BfsVertex>
	{
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public override string Name => "Bellman-Ford's algorithm";
		public override string TimeComplexity => "O(n * m)";
		public void Run()
		{
			graph.InitializeVertices();

			initialVertex.State = States.Open;

			initialVertex.Distance = 0;

			Queue<BfsVertex> openVertices = new Queue<BfsVertex>();

			openVertices.Enqueue(initialVertex);

			while (openVertices.Count > 0)
			{
				var vertex = openVertices.Dequeue(); // some open vertex
				foreach (Edge<BfsVertex> edge in vertex.OutEdges)
				{
					if (edge.To.Distance == null || edge.To.Distance > (vertex.Distance + edge.Length))
					{
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = States.Open;
						openVertices.Enqueue(edge.To);
						edge.To.Predecessor = vertex;
					}
				}
				vertex.State = States.Closed;
			}
		}

	}
}
