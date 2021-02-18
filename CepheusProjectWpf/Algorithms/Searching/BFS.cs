using CepheusProjectWpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cepheus
{
	public class BFS : Algorithm<BfsVertex>
	{
		public override bool IsFlowAlgorithm => false;
		public override bool NeedsOnlyNonNegativeEdgeLenghts => false;
		public override bool DontNeedInitialVertex => false;
		public override string Name => CepheusProjectWpf.Properties.Resources.BFSAlgo;
		public override string TimeComplexity => CepheusProjectWpf.Properties.Resources.BFSTime;
		public override string Description => CepheusProjectWpf.Properties.Resources.BFSDesc;
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		/// <summary>
		/// The main method of Breadth-first search algorithm. This is where the whole calculation takes place.
		/// </summary>
		/// <returns></returns>
		public async Task Run ()
		{
			PrintInfoStateDistance();
			Graph.InitializeVertices();
			PrintVerticesInitialized(Graph);
			
			initialVertex.State = States.Open;
			initialVertex.Distance = 0;
			PrintVertex(initialVertex);
			initialVertex.UpdateVertexInfo();

			Queue<BfsVertex> queue = new Queue<BfsVertex>();
			queue.Enqueue(initialVertex);

			while(queue.Count > 0)
			{
				BfsVertex vertex = queue.Dequeue();
				ColorVertex(vertex);
				foreach(Edge<BfsVertex> edge in vertex.OutEdges)
				{
					
					if (edge.To.State ==States.Unvisited)
					{
						await Delay(delay);
						ColorEdge(edge);
						edge.To.State = States.Open;
						edge.To.Distance = vertex.Distance + 1;
						edge.To.Predecessor = vertex;
						edge.To.UpdateVertexInfo();
						PrintVertex(edge.To);
						PrintQueued(edge.To);
						queue.Enqueue(edge.To);
						await Delay(delay-250);
						ColorVertex(edge.To);
					}
				}
				vertex.State = States.Closed;
				vertex.UpdateVertexInfo();
				PrintVertex(vertex);
				PrintDequeued(vertex);
				UncolorVertex(vertex);
				foreach (var edge in vertex.OutEdges)
					UncolorEdge(edge);
				await Delay(delay-250);
			}
			ColorShortestPaths(this);
		}

	}
}
