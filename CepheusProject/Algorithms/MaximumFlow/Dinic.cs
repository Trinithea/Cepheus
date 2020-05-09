using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class Dinic : IAlgorithm
	{
		//TODO try again to do it like in Pruvodce
		public string Name => "Dinic's algorithm";

		public string TimeComplexity => "O(n^2 * m)";
		public int MaximumFlow { get; private set; }
		private FlowNetwork<BfsVertex> graph;
		public void Run(FlowNetwork<BfsVertex> graph,BfsVertex initialVertex)
		{
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

				//save edges in reserve network before they will be removed
				var edges = new List<FlowEdge<BfsVertex>>();
				foreach (var edge in reserveNetwork.Edges.Values)
					edges.Add((FlowEdge<BfsVertex>)edge);

				GetBlockingFlow(reserveNetwork);
				ImproveFlow(graph,edges);
			}

			MaximumFlow = graph.GetMaximumFlow();
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
					reserveNetwork.AddVertex(new BfsVertex(vertex.Name));
			}

			foreach (FlowEdge<BfsVertex> edge in graph.Edges.Values)
			{
				if (edge.Capacity > edge.Flow)
					reserveNetwork.AddEdge(edge.From.Name + edge.To.Name, reserveNetwork.GetVertex(edge.From.Name), reserveNetwork.GetVertex(edge.To.Name), edge.Capacity - edge.Flow);
				if (edge.Flow > 0)
					reserveNetwork.AddEdge(edge.To.Name + edge.From.Name, reserveNetwork.GetVertex(edge.To.Name), reserveNetwork.GetVertex(edge.From.Name), edge.Flow);
			}
			
			return reserveNetwork;
		}

		
		void CleanUpNetwork(FlowNetwork<BfsVertex> network)
		{
			BFS bfs = new BFS();
			bfs.Run(network, network.Source);

			RemoveVerticesAfterSink(network);

			RemoveNotForwardEdges(network);

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
		void RemoveNotForwardEdges(FlowNetwork<BfsVertex> network)
		{
			// remove every edge to previous layers or edges inside layers
			var edgesToRemove = new List<FlowEdge<BfsVertex>>();

			foreach (FlowEdge<BfsVertex> edge in network.Edges.Values)
				if (edge.To.Distance <= edge.From.Distance)
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
			network.SetFlowTo(0);
			BFS bfs = new BFS();
			bfs.Run(network, network.Source);
			FordFulkerson ff = new FordFulkerson();
			var path = ff.GetPath(network, network.Source, network.Sink);
			//TODO do this with BFS
			while(path !=  null)
			{
				int min = path[0].Capacity - path[0].Flow;
				for (int i = 0; i < path.Count; i++)
				{
					int diff = path[i].Capacity - path[i].Flow;
					if (min > diff)
						min = diff;
				}

				for (int i = 0; i < path.Count; i++)
				{
					path[i].Flow += min;
					if (path[i].Flow == path[i].Capacity)
						network.RemoveEdge(path[i]);
						
				}

				CleanUpNetwork(network);
				bfs.Run(network, network.Source);
				path = ff.GetPath(network, network.Source, network.Sink);
			}
		}
		
		void ImproveFlow(FlowNetwork<BfsVertex> network, List<FlowEdge<BfsVertex>> edgesFromReserveNetwork)
		{
			for (int i = 0; i < edgesFromReserveNetwork.Count; i++)
			{
				var edge = network.GetEdge(edgesFromReserveNetwork[i].Name);
				if (edge != null)
					((FlowEdge<BfsVertex>)edge).Flow += edgesFromReserveNetwork[i].Flow;
				var oppositeEdge = network.GetEdge(edgesFromReserveNetwork[i].To, edgesFromReserveNetwork[i].From);
				if(oppositeEdge != null)
					((FlowEdge<BfsVertex>)oppositeEdge).Flow -= edgesFromReserveNetwork[i].Flow;
			}

		}
	}
}
