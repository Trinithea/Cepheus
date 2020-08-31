using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf;

namespace Cepheus
{
	public sealed class Relaxation : Algorithm<BfsVertex>
	{
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override string Name => "Relaxation algorithm";
		public override string TimeComplexity => "O(n^2)";

		public override string Description => "Description is not implemented."; //TODO

		public async Task Run()
		{
			Graph.InitializeVertices();
			PrintVerticesInitialized(Graph);

			initialVertex.State = States.Open;

			initialVertex.Distance = 0;
			PrintVertex(initialVertex);

			List<BfsVertex> openVertices = new List<BfsVertex>();

			openVertices.Add(initialVertex);

			while(openVertices.Count > 0)
			{
				var vertex = openVertices[0]; // some open vertex //TODO couldn't be here the alst position for better efficiency?
				foreach(Edge<BfsVertex> edge in vertex.OutEdges)
				{
					if (edge.To.Distance == null || edge.To.Distance > (vertex.Distance + edge.Length))
					{
						ColorEdge(edge);
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = States.Open;
						await Task.Delay(delay - 250);
						ColorVertex(edge.To);
						PrintVertex(edge.To);
						openVertices.Add(edge.To);
						edge.To.Predecessor = vertex;
					}
					await Task.Delay(delay);
				}
				vertex.State = States.Closed;
				openVertices.RemoveAt(0);
				UncolorVertex(vertex);
				PrintVertex(vertex);
				foreach (var edge in vertex.OutEdges)
					UncolorEdge(edge);
				await Task.Delay(delay);
			}
		}

	}
}
