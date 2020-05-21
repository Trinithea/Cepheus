using CepheusProjectWpf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	public class BFS : Algorithm<BfsVertex>
	{
		public override string Name => "Breadth-first search";
		public override string TimeComplexity => "O(n + m)";

		public void Run ()
		{
			graph.InitializeVertices();

			initialVertex.State = States.Open;
			initialVertex.Distance = 0;

			Queue<BfsVertex> queue = new Queue<BfsVertex>();
			queue.Enqueue(initialVertex);

			while(queue.Count > 0)
			{
				BfsVertex vertex = queue.Dequeue();
				foreach(Edge<BfsVertex> edge in vertex.OutEdges)
				{
					if (edge.To.State ==States.Unvisited)
					{
						edge.To.State = States.Open;
						edge.To.Distance = vertex.Distance + 1;
						edge.To.Predecessor = vertex;
						queue.Enqueue(edge.To);
					}
					
				}
				vertex.State = States.Closed;
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



		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
	}
}
