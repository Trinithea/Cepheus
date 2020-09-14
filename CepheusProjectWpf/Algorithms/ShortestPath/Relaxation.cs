using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf;

namespace Cepheus
{
	public sealed class Relaxation : Algorithm<BfsVertex>
	{
		public override bool IsFlowAlgorithm => false;
		public override bool NeedsOnlyNonNegativeEdgeLenghts => false;
		public override bool DontNeedInitialVertex => false;
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override string Name => CepheusProjectWpf.Properties.Resources.RelaxAlgo;
		public override string TimeComplexity => CepheusProjectWpf.Properties.Resources.RelaxTime;

		public override string Description => CepheusProjectWpf.Properties.Resources.RelaxDesc;
		/// <summary>
		/// The main method of Relaxation's algorithm. This is where the whole calculation takes place.
		/// </summary>
		/// <returns></returns>
		public async Task Run()
		{
			Graph.InitializeVertices();
			PrintVerticesInitialized(Graph);

			initialVertex.State = States.Open;
			initialVertex.Distance = 0;
			initialVertex.UpdateVertexInfo();
			PrintVertex(initialVertex);

			Queue<BfsVertex> openVertices = new Queue<BfsVertex>();

			openVertices.Enqueue(initialVertex);

			while(openVertices.Count > 0)
			{
				var vertex = openVertices.Peek(); // some open vertex 
				foreach(Edge<BfsVertex> edge in vertex.OutEdges)
				{
					if (edge.To.Distance > (vertex.Distance + edge.Length))
					{
						await Task.Delay(delay);
						ColorEdge(edge);
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = States.Open;
						await Task.Delay(delay - 250);
						ColorVertex(edge.To);
						edge.To.UpdateVertexInfo();
						PrintVertex(edge.To);
						openVertices.Enqueue(edge.To);
						edge.To.Predecessor = vertex;
					}
				}
				vertex.State = States.Closed;
				vertex.UpdateVertexInfo();
				openVertices.Dequeue();
				UncolorVertex(vertex);
				PrintVertex(vertex);
				foreach (var edge in vertex.OutEdges)
					UncolorEdge(edge);
				await Task.Delay(delay);
			}
			ColorShortestPaths(this);
		}

	}
}
