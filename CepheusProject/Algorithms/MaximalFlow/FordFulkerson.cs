using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class FordFulkerson : IAlgorithm
	{
		public string Name => "Ford-Fulkerson's algorithm";

		public string TimeComplexity => "O(m * f)";
		public int MaximalFlow = 0;
		public void Run(FlowNetwork<BfsVertex> graph, BfsVertex initialVertex)
		{
			graph.InitializeEdges();
			var path = GetUnsaturatedPath(graph);
			while(path != null ) // must be nenasycená and nemusí být nejkratší
			{
				int minimalReserve = GetMinimalReserve(path);
				ImproveFlowOnPath(path,minimalReserve);
				path = GetUnsaturatedPath(graph);
			}
			MaximalFlow = graph.GetMaximalFlow();
		}
		int GetMinimalReserve(List<FlowEdge<BfsVertex>> path)
		{
			int min = path[0].Reserve;
			for (int i = 0; i < path.Count; i++)
				if (path[i].Reserve < min)
					min = path[i].Reserve;
			return min;
		}
		void ImproveFlowOnPath(List<FlowEdge<BfsVertex>> path,int minimalReserve)
		{
			for (int i = 0; i < path.Count; i++)
			{
				var edge = path[i];
				int delta = Math.Min(edge.OppositeEdge.Flow, minimalReserve);
				edge.OppositeEdge.Flow -= delta;
				edge.Flow += minimalReserve - delta;
			}
				
		}
		List<FlowEdge<BfsVertex>> GetUnsaturatedPath(FlowNetwork<BfsVertex> graph)
		{
			// why I don't use BFS algorithm which is already implemeted? Because I have to use some flow network properties
			
			graph.InitializeVertices();

			graph.Source.State = IStateVertex.States.Open;

			Queue<BfsVertex> queue = new Queue<BfsVertex>();
			queue.Enqueue(graph.Source);

			while (queue.Count > 0)
			{
				BfsVertex vertex = queue.Dequeue();
				foreach (FlowEdge<BfsVertex> edge in vertex.OutEdges)
				{
					if (edge.To.State == IStateVertex.States.Unvisited && edge.Reserve > 0)
					{
						edge.To.State = IStateVertex.States.Open;
						edge.To.Predecessor = vertex;
						queue.Enqueue(edge.To);
					}
				}
				vertex.State = IStateVertex.States.Closed;
			}

			return GetPath(graph, graph.Source, graph.Sink);
		}

		public List<FlowEdge<BfsVertex>> GetPath(Graph<BfsVertex> graph, BfsVertex from, BfsVertex to)
		{
			if (to.Predecessor == null) //'to' is not reachable from 'from'
				return null;
			else
			{
				var currentVertex = to;
				var edges = new List<FlowEdge<BfsVertex>>();
				var vertices = new List<BfsVertex>();
				while (currentVertex.Predecessor != null)
				{
					vertices.Add(currentVertex);
					currentVertex = currentVertex.Predecessor;
				}
				vertices.Add(currentVertex);

				for (int i = vertices.Count - 1; i > 0; i--)
					edges.Add((FlowEdge<BfsVertex>)graph.GetEdge(vertices[i], vertices[i - 1]));

				return edges;
			}
		}
	}
}
