using System;
using System.Collections.Generic;
using System.Text;
using CepheusProjectWpf;

namespace Cepheus
{
	public sealed class Relaxation : Algorithm<BfsVertex>
	{
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public override void Accept(VisitorRunner visitor)
		{
			visitor.Visit(this);
		}
		public override string Name => "Relaxation algorithm";
		public override string TimeComplexity => "O(n^2)";

		public void Run()
		{
			graph.InitializeVertices();

			initialVertex.State = States.Open;

			initialVertex.Distance = 0;

			List<BfsVertex> openVertices = new List<BfsVertex>();

			openVertices.Add(initialVertex);

			while(openVertices.Count > 0)
			{
				var vertex = openVertices[0]; // some open vertex
				foreach(Edge<BfsVertex> edge in vertex.OutEdges)
				{
					if (edge.To.Distance == null || edge.To.Distance > (vertex.Distance + edge.Length))
					{
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = States.Open;
						openVertices.Add(edge.To);
						edge.To.Predecessor = vertex;
					}
				}
				vertex.State = States.Closed;
				openVertices.RemoveAt(0);
			}
		}

	}
}
