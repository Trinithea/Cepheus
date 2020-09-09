using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CepheusProjectWpf;
using CepheusProjectWpf.UIWindows__remove__;
using CepheusProjectWpf.GraphShapes;
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
		Canvas netOfReservesCanvas;
		NetOfReservesWindow netOfReservesWindow;
		double windowWidth;
		double windowHeight;
		double leftDifference;
		double topDifference;
		public override string Description => "Dinic's algorithm or Dinitz's algorithm is a strongly polynomial algorithm for computing the maximum flow in a flow network, conceived in 1970 by Israeli (formerly Soviet) computer scientist Yefim (Chaim) A. Dinitz. The algorithm runs in O(n^2 * m) time and is similar to the Edmonds–Karp algorithm, which runs in O(n * m^2) time, in that it uses shortest augmenting paths. The introduction of the concepts of the level graph and blocking flow enable Dinic's algorithm to achieve its performance. ";

		public async Task Run()
		{
			BFS bfs = new BFS();
			netOfReservesWindow = new NetOfReservesWindow();
			netOfReservesCanvas = netOfReservesWindow.NetCanvas;
			GetDimensionsOfNetOfReservesWindow();
			netOfReservesWindow.Width = windowWidth;
			netOfReservesWindow.Height = windowHeight;
			while (true)
			{
				ShowNetOfReservesWindow();
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
				await Task.Delay(2 * delay);
				netOfReservesWindow.Close();
				ImproveFlow(graph,edges);

			}

			MaximumFlow = graph.GetMaximumFlow();
		}
		void ShowNetOfReservesWindow()
		{
			netOfReservesWindow = new NetOfReservesWindow();
			netOfReservesWindow.Width = windowWidth;
			netOfReservesWindow.Height = windowHeight;
			netOfReservesCanvas = netOfReservesWindow.NetCanvas;
			GetReserveNetwork();
			netOfReservesWindow.ShowDialog();
			
		}

		//pro posunutí grafu ke kraji
		void GetDimensionsOfNetOfReservesWindow()
		{
			var originCanvas = graph.UltimateVertices[graph.Source].GraphCanvas;
			double vertexWidth = graph.UltimateVertices[graph.Source].MainEllipse.Width;
			double vertexHeight = graph.UltimateVertices[graph.Source].MainEllipse.Height;
			double left = originCanvas.ActualWidth; //the left coordinate of vertex which is leftmost
			double top = originCanvas.ActualHeight; //the top coordinate of vertex which is topmost
			double right = 0; //the left coordinate of vertex which is rightmost
			double bottom = 0; //the top coordinate of vertex which is most down

			foreach (var vertex in graph.UltimateVertices)
			{
				if (vertex.Value.Left < left)
					left = vertex.Value.Left;
				if (vertex.Value.Top < top)
					top = vertex.Value.Top;
				if (vertex.Value.Left > right)
					right = vertex.Value.Left;
				if (vertex.Value.Top > bottom)
					bottom = vertex.Value.Top;
			}

			double reserve = 50;
			leftDifference = left - reserve;
			topDifference = top - reserve;
			windowWidth = right - leftDifference + vertexWidth + reserve +netOfReservesCanvas.Margin.Left+ netOfReservesCanvas.Margin.Right;
			windowHeight = bottom - topDifference + vertexHeight + reserve + netOfReservesCanvas.Margin.Top + netOfReservesCanvas.Margin.Bottom;
		}

		FlowNetwork<BfsVertex> GetReserveNetwork()
		{
			outputConsole.Text += "\nCreating network of reserves...";
			FlowNetwork<BfsVertex> reserveNetwork = new FlowNetwork<BfsVertex>() ;

			
			foreach (var vertex in graph.UltimateVertices)
			{
				BfsVertex newVertex = null;
				if (vertex.Key == graph.Source)
					newVertex = reserveNetwork.AddVertex(graph.Source.UniqueId, graph.Source.Name);					
				else if (vertex.Key == graph.Sink)
					newVertex = reserveNetwork.AddVertex(graph.Sink.UniqueId, graph.Sink.Name);
				else
					newVertex = reserveNetwork.AddVertex(vertex.Key.UniqueId, vertex.Key.Name);

				var copy = vertex.Value.DrawThisOnCanvasAndReturnCopy(netOfReservesCanvas, leftDifference,topDifference);
				reserveNetwork.UltimateVertices.Add(newVertex, copy);
			}
			foreach (var edge in graph.UltimateEdges)
			{
				FlowEdge<BfsVertex> flowEdge = (FlowEdge<BfsVertex>)edge.Key;
				FlowEdge<BfsVertex> newEdge = null;
				if (flowEdge.Capacity > flowEdge.Flow)
				{
					newEdge = reserveNetwork.AddEdge(reserveNetwork.GetVertex(flowEdge.From.UniqueId), reserveNetwork.GetVertex(flowEdge.To.UniqueId), flowEdge.From.UniqueId + "->" + flowEdge.To.UniqueId, flowEdge.Capacity - flowEdge.Flow, null); //TODO tady ještě přidat argument na konec s TextBoxem, jinak špatnej overload
					outputConsole.Text += "\nEdge " + flowEdge.From.Name + "->" + flowEdge.To.Name +" added with reserve (length)";
					
				}
				if (flowEdge.Flow > 0)
				{
					newEdge = reserveNetwork.AddEdge(reserveNetwork.GetVertex(flowEdge.To.UniqueId), reserveNetwork.GetVertex(flowEdge.From.UniqueId), flowEdge.To.UniqueId + "->" + flowEdge.From.UniqueId, flowEdge.Flow, null);
				}
				if (newEdge != null)
				{
					var copy = edge.Value.DrawThisOnCanvasAndReturnCopy(netOfReservesCanvas, reserveNetwork.UltimateVertices[newEdge.From], reserveNetwork.UltimateVertices[newEdge.To],leftDifference,topDifference);
					newEdge.currentFlowInfo = copy.txtLength;
					reserveNetwork.UltimateEdges.Add(newEdge, copy);
				}
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
			ff.GetPath(network.Source, network.Sink);
			var path = ff.PathFromSourceToSink;
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
				ff.GetPath(network.Source, network.Sink);
				path = ff.PathFromSourceToSink;
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
