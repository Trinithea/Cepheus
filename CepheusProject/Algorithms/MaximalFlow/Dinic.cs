using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class Dinic : IAlgorithm
	{
		public string Name => "Dinic's algorithm";

		public string TimeComplexity => "O(n^2 * m)";
		public int MaximalFlow { get; private set; }
		private FlowNetwork<BfsVertex> graph;
		public void Run(FlowNetwork<BfsVertex> graph,BfsVertex initialVertex)
		{
			graph.InitializeEdges();
			this.graph = graph;

			BFS bfs = new BFS();

			while (true)
			{
				var reserveNetwork = GetReserveNetwork();
				bfs.Run(reserveNetwork, reserveNetwork.Source);
				int? lengthOfShortestPath = reserveNetwork.Sink.Distance;
				if (lengthOfShortestPath == null)
					break;
				CleanUpNetwork(reserveNetwork);
				GetBlockingFlow(reserveNetwork);
				ImproveFlow(graph,reserveNetwork);
			}

			MaximalFlow = graph.GetMaximalFlow();
		}
		FlowNetwork<BfsVertex> GetReserveNetwork()
		{
			var source = new BfsVertex(graph.Source.Name);
			var sink = new BfsVertex(graph.Sink.Name);
			FlowNetwork<BfsVertex> reserveNetwork = new FlowNetwork<BfsVertex>(source,sink) ;

			foreach (BfsVertex vertex in graph.Vertices.Values)
			{
				if (vertex == graph.Source)
					reserveNetwork.AddVertex(source);
				else if (vertex == graph.Sink)
					reserveNetwork.AddVertex(sink);
				else
					reserveNetwork.AddVertex(vertex);
			}
				
			foreach (FlowEdge<BfsVertex> edge in graph.Edges.Values)
				if(edge.Reserve>0)
					reserveNetwork.AddEdge(edge.From.Name + edge.To.Name, edge.From, edge.To, edge.Reserve); 
			return reserveNetwork;
		}
		void CleanUpNetwork(FlowNetwork<BfsVertex> network)
		{
			RemoveVerticesAfterSink(network);

			RemoveEdgesInsideLayersOrToPreviousLayers(network);

			RemoveVerticesWithZeroOutEdges(network);
		}
		void RemoveVerticesAfterSink(FlowNetwork<BfsVertex> network)
		{
			// remove every vertex after sink
			var verticesToRemove = new List<BfsVertex>();
			int? maxDistance = network.Sink.Distance;

			foreach (var vertex in network.Vertices.Values)
				if (vertex.Distance > maxDistance)
					verticesToRemove.Add(vertex);

			for (int i = 0; i < verticesToRemove.Count; i++)
				network.RemoveVertex(verticesToRemove[i]);
			verticesToRemove.Clear();
		}
		void RemoveEdgesInsideLayersOrToPreviousLayers(FlowNetwork<BfsVertex> network)
		{
			// remove every edge to previous layers or edges inside layers
			var edgesToRemove = new List<FlowEdge<BfsVertex>>();

			foreach (FlowEdge<BfsVertex> edge in network.Edges.Values)
				if (edge.To.Distance >= edge.From.Distance)
					edgesToRemove.Add(edge);
			for (int i = 0; i < edgesToRemove.Count; i++)
				network.RemoveEdge(edgesToRemove[i]);
		}
		void RemoveVerticesWithZeroOutEdges(FlowNetwork<BfsVertex> network)
		{
			var verticesToRemove = new Queue<BfsVertex>();

			foreach (var vertex in network.Vertices.Values)
				if (vertex.OutEdges.Count == 0 && vertex != network.Sink && vertex != network.Source)
					verticesToRemove.Enqueue(vertex);

			while (verticesToRemove.Count > 0)
			{
				var vertex = verticesToRemove.Dequeue();
				var fromVertices = new List<BfsVertex>();
				for (int i = 0; i < vertex.InEdges.Count; i++)
					fromVertices.Add(vertex.InEdges[i].From);
				network.RemoveVertex(vertex);
				for (int i = 0; i < fromVertices.Count; i++)
					if (fromVertices[i].OutEdges.Count == 0)
						verticesToRemove.Enqueue(fromVertices[i]);
			}
		}
		void GetBlockingFlow(FlowNetwork<BfsVertex> network)
		{
			network.InitializeEdges();
			FordFulkerson fordFulkerson = new FordFulkerson();
			var path = fordFulkerson.GetUnsaturatedPathFromSourceToSink(network);
			while(path.Count > 0)
			{
				int min = path[0].Reserve - path[0].Flow;
				for (int i = 0; i < path.Count; i++)
				{
					int diff = path[i].Reserve - path[i].Flow;
					if (min > diff)
						min = diff;
				}

				for (int i = 0; i < path.Count; i++)
				{
					path[i].Flow += min;
					if (path[i].Flow == path[i].Reserve)
						network.RemoveEdge(path[i]);
				}

				CleanUpNetwork(network);
			}
		}
		void ImproveFlow(FlowNetwork<BfsVertex> network, FlowNetwork<BfsVertex> reserveNetwork)
		{
			foreach (FlowEdge<BfsVertex> edge in network.Edges.Values)
				if(reserveNetwork.Edges.ContainsKey(edge.Name))
					edge.Flow += ((FlowEdge<BfsVertex>)reserveNetwork.GetEdge(edge.Name)).Flow;
		}
	}
}
