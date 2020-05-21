using System;
using System.Collections.Generic;
using System.Text;
using CepheusProjectWpf;
namespace Cepheus
{
	public class FordFulkerson : FlowAlgorithm<BfsVertex>
	{
		
		public override void Accept(Visitor visitor)
		{
			visitor.Visit(this);
		}
		public override string Name => "Ford-Fulkerson's algorithm";

		public override string TimeComplexity => "O(m * f)";

		public int MaximumFlow { get; private set; }
		public void Run(FlowNetwork<BfsVertex> graph, string sourceVertexName, string sinkVertexName)
		{
			graph.Source = graph.Vertices[sourceVertexName];
			graph.Sink = graph.Vertices[sinkVertexName];
			graph.InitializeEdges();
			var path = GetUnsaturatedPathFromSourceToSink(graph);
			while(path != null ) // must be nenasycená and nemusí být nejkratší
			{
				int minimalReserve = GetMinimalReserve(path);
				ImproveFlowOnPath(path,minimalReserve);
				path = GetUnsaturatedPathFromSourceToSink(graph);
			}
			MaximumFlow = graph.GetMaximumFlow();
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
		public List<FlowEdge<BfsVertex>> GetUnsaturatedPathFromSourceToSink(FlowNetwork<BfsVertex> graph)
		{
			// why I don't use BFS algorithm which is already implemeted? Because I have to use some flow network properties
			
			graph.InitializeVertices();

			graph.Source.State = States.Open;
			graph.Source.Distance = 0;

			Queue<BfsVertex> queue = new Queue<BfsVertex>();
			queue.Enqueue(graph.Source);

			while (queue.Count > 0)
			{
				BfsVertex vertex = queue.Dequeue();
				foreach (FlowEdge<BfsVertex> edge in vertex.OutEdges)
				{
					if (edge.To.State == States.Unvisited && edge.Reserve > 0)
					{
						edge.To.State = States.Open;
						edge.To.Distance = vertex.Distance + 1;
						edge.To.Predecessor = vertex;
						queue.Enqueue(edge.To);
					}
				}
				vertex.State = States.Closed;
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
				var path = new List<FlowEdge<BfsVertex>>();
				while (currentVertex.Predecessor != null)
				{
					path.Insert(0, (FlowEdge<BfsVertex>)graph.GetEdge(currentVertex.Predecessor, currentVertex));
					currentVertex = currentVertex.Predecessor;
				}
				return path;
			}
		}
	}
}
