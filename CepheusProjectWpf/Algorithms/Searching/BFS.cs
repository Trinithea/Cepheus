using CepheusProjectWpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cepheus
{
	public class BFS : Algorithm<BfsVertex>
	{
		public override string Name => CepheusProjectWpf.Properties.Resources.BFSAlgo;
		public override string TimeComplexity => CepheusProjectWpf.Properties.Resources.BFSTime;

		public override string Description => CepheusProjectWpf.Properties.Resources.BFSDesc;

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
						ColorEdge(edge);
						edge.To.State = States.Open;
						edge.To.Distance = vertex.Distance + 1;
						edge.To.Predecessor = vertex;
						edge.To.UpdateVertexInfo();
						PrintVertex(edge.To);
						PrintQueued(edge.To);
						queue.Enqueue(edge.To);
						await Task.Delay(delay-250);
						ColorVertex(edge.To);
						await Task.Delay(delay);
					}
				}
				vertex.State = States.Closed;
				vertex.UpdateVertexInfo();
				PrintVertex(vertex);
				PrintDequeued(vertex);
				UncolorVertex(vertex);
				foreach (var edge in vertex.OutEdges)
					UncolorEdge(edge);
				await Task.Delay(delay);
			}
			ColorShortestPaths();
		}

		void ColorShortestPaths()
		{
			foreach(var vertex in Graph.Vertices.Values)
			{
				if (vertex.Predecessor != null)
					vertex.ColorEdgeWithPredecessor(this);
			}
		}
		

		//TODO udělat generický před edge, ať to neni tak hnusně nakopírovaný ve FF
		public List<Edge<BfsVertex>> GetPath(Graph<BfsVertex> graph, BfsVertex from, BfsVertex to)
		{
			if (to.Predecessor == null) //'to' is not reachable from 'from'
				return null;
			else
			{
				var currentVertex = to;
				var path = new List<Edge<BfsVertex>>();
				while (currentVertex.Predecessor != null)
				{
					path.Insert(0, (Edge<BfsVertex>)graph.GetEdge(currentVertex.Predecessor, currentVertex));
					currentVertex = currentVertex.Predecessor;
				}
				return path;
			}
		}

		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}

		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
	}
}
