using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf;
namespace Cepheus
{
	 public class Dinic : FlowAlgorithm<BfsVertex>
	 {
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		//TODO try again to do it like in Pruvodce
		public override string Name => "Dinic's algorithm";

		public override string TimeComplexity => "O(n^2 * m)";
		public int MaximumFlow { get; private set; }

		public override string Description => "Dinic's algorithm or Dinitz's algorithm is a strongly polynomial algorithm for computing the maximum flow in a flow network, conceived in 1970 by Israeli (formerly Soviet) computer scientist Yefim (Chaim) A. Dinitz. The algorithm runs in O(n^2 * m) time and is similar to the Edmonds–Karp algorithm, which runs in O(n * m^2) time, in that it uses shortest augmenting paths. The introduction of the concepts of the level graph and blocking flow enable Dinic's algorithm to achieve its performance. ";

		public async Task Run()
		{
			BFS bfs = new BFS();

			while (true)
			{
				var reserveNetwork = GetReserveNetwork();
				bfs.Graph = reserveNetwork;
				bfs.initialVertex = reserveNetwork.Source;
				await bfs.Run();
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
			FlowNetwork<BfsVertex> reserveNetwork = new FlowNetwork<BfsVertex>() ;

			foreach (BfsVertex vertex in graph.Vertices.Values)
			{
				if (vertex == graph.Source)
					reserveNetwork.AddVertex(graph.Source.UniqueId, graph.Source.Name);
				else if (vertex == graph.Sink)
					reserveNetwork.AddVertex(graph.Sink.UniqueId, graph.Sink.Name);
				else
					reserveNetwork.AddVertex(vertex.UniqueId, vertex.Name);
			}

			foreach (FlowEdge<BfsVertex> edge in graph.Edges.Values)
			{
				if (edge.Capacity > edge.Flow)
					reserveNetwork.AddEdge( reserveNetwork.GetVertex(edge.From.UniqueId), reserveNetwork.GetVertex(edge.To.UniqueId), edge.From.UniqueId+"->" + edge.To.UniqueId, edge.Capacity - edge.Flow);
				if (edge.Flow > 0)
					reserveNetwork.AddEdge( reserveNetwork.GetVertex(edge.To.UniqueId), reserveNetwork.GetVertex(edge.From.UniqueId), edge.To.UniqueId+"->" + edge.From.UniqueId, edge.Flow);
			}
			
			return reserveNetwork;
		}

		
		void CleanUpNetwork(FlowNetwork<BfsVertex> network)
		{
			BFS bfs = new BFS();
			bfs.Graph = network;
			bfs.initialVertex = network.Source;
			bfs.Run();

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
			bfs.Graph = network;
			bfs.initialVertex = network.Source;
			bfs.Run();
			FordFulkerson ff = new FordFulkerson();
			ff.graph = network;
			var path = ff.GetPath( network.Source, network.Sink);
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
				bfs.Graph = network;
				bfs.initialVertex = network.Source;
				bfs.Run();
				ff.graph = network;
				path = ff.GetPath(network.Source, network.Sink);
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
