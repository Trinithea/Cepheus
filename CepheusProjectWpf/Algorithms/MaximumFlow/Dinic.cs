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
		
		public override string Name => CepheusProjectWpf.Properties.Resources.DinicAlgo;

		public override string TimeComplexity => CepheusProjectWpf.Properties.Resources.DinicTime;
		/// <summary>
		/// A canvas on which will draw a network of reserves.
		/// </summary>
		Canvas netOfReservesCanvas;
		/// <summary>
		/// A UI Window in which will be a network of reserves. Its dimensions are reduced to the size of the graph.
		/// </summary>
		NetOfReservesWindow netOfReservesWindow;
		/// <summary>
		/// The width of the netOfReservesWindow.
		/// </summary>
		double windowWidth;
		/// <summary>
		/// The height of the netOfReservesWindow.
		/// </summary>
		double windowHeight;
		/// <summary>
		/// How much to move the GraphShapes to the left.
		/// </summary>
		double leftDifference;
		/// <summary>
		/// How much to move the GraphShapes up.
		/// </summary>
		double topDifference;
		/// <summary>
		/// Central instance of the BFS algorithm for finding paths in a graph.
		/// </summary>
		BFS bfs;
		public override string Description => CepheusProjectWpf.Properties.Resources.DinicDescription;

		public override bool IsFlowAlgorithm => true;
		public override bool NeedsOnlyNonNegativeEdgeLenghts => true;
		public override bool DontNeedInitialVertex => false;
		/// <summary>
		/// The main method of Dinic's algorithm. This is where the whole calculation takes place.
		/// </summary>
		/// <returns></returns>
		public async Task Run()
		{
			bfs = new BFS();
			netOfReservesWindow = new NetOfReservesWindow();
			netOfReservesCanvas = netOfReservesWindow.NetCanvas; //
			GetDimensionsOfNetOfReservesWindow();
			netOfReservesWindow.Width = windowWidth;
			netOfReservesWindow.Height = windowHeight;
			while (true)
			{
				var reserveNetwork = ShowNetOfReservesWindow();
				bfs.Graph = reserveNetwork;
				bfs.initialVertex = reserveNetwork.Source;
				bfs.SetOutputConsole(outputConsole); // You cannot write an output without an output console.
				PrintLookingForPath();
				await bfs.Run();
				int? lengthOfShortestPath = reserveNetwork.Sink.Distance;
				if (lengthOfShortestPath == Int32.MaxValue)
				{
					outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.PathDoesntExist;
					netOfReservesWindow.Close();
					break;
				}
				else
					outputConsole.Text += "\n"+CepheusProjectWpf.Properties.Resources.ShortestPathContains + lengthOfShortestPath + CepheusProjectWpf.Properties.Resources.SpaceEdges;
					
				await CleanUpNetwork(reserveNetwork);

				//save edges from a network of reserves before they will be removed
				var edges = new List<FlowEdge<BfsVertex>>();
				foreach (var edge in reserveNetwork.Edges.Values)
					edges.Add((FlowEdge<BfsVertex>)edge);

				await GetBlockingFlow(reserveNetwork);
				await Delay(2 * delay);
				netOfReservesWindow.Close();
				await ImproveFlow(graph,edges); //using saved edges

			}

			MaximumFlow = graph.GetMaximumFlow();
			PrintMaximumFlow();
		}
		/// <summary>
		/// Show UI Window with a network of reserves with correct dimensions. Returns a network of reserves.
		/// </summary>
		/// <returns></returns>
		FlowNetwork<BfsVertex> ShowNetOfReservesWindow()
		{
			netOfReservesWindow = new NetOfReservesWindow();
			netOfReservesWindow.Width = windowWidth;
			netOfReservesWindow.Height = windowHeight;
			netOfReservesCanvas = netOfReservesWindow.NetCanvas;
			var reserveNetwork = GetReserveNetwork();
			netOfReservesWindow.Show();
			netOfReservesCanvas.IsEnabled = false;
			return reserveNetwork;
			
		}

		/// <summary>
		/// Calculation of leftDifference, topDifference, windowWidth and windowHeight.
		/// </summary>
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

		/// <summary>
		/// It creates a network of reserves from the original graph. Returns a network of reserves.
		/// </summary>
		/// <returns></returns>
		FlowNetwork<BfsVertex> GetReserveNetwork()
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.CreatingReserveNet;
			FlowNetwork<BfsVertex> reserveNetwork = new FlowNetwork<BfsVertex>() ;

			foreach (var vertex in graph.UltimateVertices)
			{
				
				var copy = vertex.Value.DrawThisOnCanvasAndReturnCopy(netOfReservesCanvas, leftDifference, topDifference);
				BfsVertex newVertex = reserveNetwork.AddVertex(copy);
				if (vertex.Key == graph.Source)
					reserveNetwork.Source = newVertex;
				else if (vertex.Key == graph.Sink)
					reserveNetwork.Sink = newVertex;
			}
			foreach (var edge in graph.UltimateEdges)
			{
				FlowEdge<BfsVertex> flowEdge = (FlowEdge<BfsVertex>)edge.Key;
				FlowEdge<BfsVertex> newEdge = null;
				if (flowEdge.Capacity > flowEdge.Flow)
				{
					newEdge = reserveNetwork.AddEdge(reserveNetwork.GetVertex(flowEdge.From.UniqueId), reserveNetwork.GetVertex(flowEdge.To.UniqueId), flowEdge.From.UniqueId + "->" + flowEdge.To.UniqueId, flowEdge.Capacity - flowEdge.Flow, null); 
					outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.NLEdgeSpace + flowEdge.From.Name + "->" + flowEdge.To.Name + CepheusProjectWpf.Properties.Resources .AddedWithReserve+ (flowEdge.Capacity - flowEdge.Flow);
					var copy = edge.Value.DrawThisOnCanvasAndReturnCopy(netOfReservesCanvas, reserveNetwork.UltimateVertices[newEdge.From], reserveNetwork.UltimateVertices[newEdge.To], leftDifference, topDifference);
					newEdge.currentFlowInfo = copy.txtLength;
					newEdge.UpdateCurrentFlowInfo();
					reserveNetwork.UltimateEdges.Add(newEdge, copy);
				}
				if (flowEdge.Flow > 0)
				{
					newEdge = reserveNetwork.AddEdge(reserveNetwork.GetVertex(flowEdge.To.UniqueId), reserveNetwork.GetVertex(flowEdge.From.UniqueId), flowEdge.To.UniqueId + "->" + flowEdge.From.UniqueId, flowEdge.Flow, null);
					outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.NLEdgeSpace + flowEdge.From.Name + "->" + flowEdge.To.Name + CepheusProjectWpf.Properties.Resources.AddedWithReserve + flowEdge.Flow;
				}
				
			}
			return reserveNetwork;
		}

		/// <summary>
		/// Network cleaning
		/// </summary>
		/// <param name="network">The network to be cleaned</param>
		/// <returns></returns>
		async Task CleanUpNetwork(FlowNetwork<BfsVertex> network)
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.CleaningReserveNet;
			bfs.Graph = network;
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.DividingVertices;
			await bfs.Run(); // Divides the vertices into layers according to the distance from the source.

			await RemoveVerticesAfterSink(network);
			await Delay(delay);

			await RemoveNotForwardEdges(network);
			await Delay(delay);

			await RemoveVerticesWithZeroOutEdges(network);
			await Delay(delay);
		}
		/// <summary>
		/// It removes all vertices that are farther from the source than the sink from the method argument network.
		/// </summary>
		/// <param name="network"></param>
		/// <returns></returns>
		async Task RemoveVerticesAfterSink(FlowNetwork<BfsVertex> network)
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.RemovingFarVertices;
			PrintVertex(network.Sink);
			var verticesToRemove = new List<BfsVertex>();
			int? maxDistance = network.Sink.Distance;

			foreach (var vertex in network.Vertices.Values)
				if (vertex.Distance > maxDistance)
				{
					verticesToRemove.Add(vertex);
					ColorVertex(network,vertex);
					await Delay(delay-250);
				}

			for (int i = 0; i < verticesToRemove.Count; i++)
			{
				network.RemoveVertex(verticesToRemove[i]);
				PrintVertexRemovedFromReserveNetwork(verticesToRemove[i]);
			}
			verticesToRemove.Clear();
		}
		/// <summary>
		/// Removes all edges leading to the previous or same layers from the method argument network.
		/// </summary>
		/// <param name="network"></param>
		/// <returns></returns>
		async Task RemoveNotForwardEdges(FlowNetwork<BfsVertex> network)
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.RemovingBackEdges;
			var edgesToRemove = new List<FlowEdge<BfsVertex>>();

			foreach (FlowEdge<BfsVertex> edge in network.Edges.Values)
				if (edge.To.Distance <= edge.From.Distance)
				{
					edgesToRemove.Add(edge);
					ColorEdge(network, edge);
					await Delay(delay-250);
				}
					
					
			for (int i = 0; i < edgesToRemove.Count; i++)
			{
				network.RemoveEdge(edgesToRemove[i]);
				PrintEdgeRemovedFromReserveNetwork(edgesToRemove[i]);
			}
				
		}
		/// <summary>
		/// Removes so-called dead ends from the method argument network.
		/// </summary>
		/// <param name="network"></param>
		/// <returns></returns>
		async Task RemoveVerticesWithZeroOutEdges(FlowNetwork<BfsVertex> network)
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.RemovingNoOutVertices;
			var verticesToRemove = new Queue<BfsVertex>();

			foreach (var vertex in network.Vertices.Values)
				if (vertex.OutEdges.Count == 0 && vertex != network.Sink && vertex != network.Source)
				{
					verticesToRemove.Enqueue(vertex);
					ColorVertex(network,vertex);
					await Delay(delay);
				}

			while (verticesToRemove.Count > 0)
			{
				var vertex = verticesToRemove.Dequeue();
				var fromVertices = new List<BfsVertex>();
				for (int i = 0; i < vertex.InEdges.Count; i++)
					fromVertices.Add(vertex.InEdges[i].From);
				network.RemoveVertex(vertex);
				PrintVertexRemovedFromReserveNetwork(vertex);

				//Removing the vertex could create another dead end.
				for (int i = 0; i < fromVertices.Count; i++)
					if (fromVertices[i].OutEdges.Count == 0)
					{
						verticesToRemove.Enqueue(fromVertices[i]);
						ColorVertex(network, fromVertices[i]);
						await Delay(delay);
					}
			}
		}
		/// <summary>
		/// Increases the flow in the reserve network. If the edge is saturated, it removes it from the reserve network. It then cleans the network.
		/// </summary>
		/// <param name="network"></param>
		/// <returns></returns>
		async Task GetBlockingFlow(FlowNetwork<BfsVertex> network)
		{
			network.SetFlowTo(0);
			bfs.Graph = network;
			bfs.initialVertex = network.Source;
			PrintLookingForPath();
			await bfs.Run(); // for finding a path from source to sink
			FordFulkerson ff = new FordFulkerson();
			ff.graph = network;
			ff.SetOutputConsole(outputConsole);
			await ff.GetPath(network.Source, network.Sink);
			var path = ff.PathFromSourceToSink;
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
					outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.RemovingSaturatedEdges;
					path[i].Flow += min;
					path[i].UpdateCurrentFlowInfo();
					if (path[i].Flow == path[i].Capacity)
					{
						ColorEdge(network, path[i]);
						await Delay(delay);
						network.RemoveEdge(path[i]);
						PrintEdgeRemovedFromReserveNetwork(path[i]);
					}
				}

				await CleanUpNetwork(network);
				bfs.Graph = network;
				bfs.initialVertex = network.Source;
				if (network.UltimateVertices.ContainsKey(network.Source)) // if the source has not yet been deleted, find another path from the source to the sink
				{
					PrintLookingForPath();
					await bfs.Run();
					ff.graph = network;
					await ff.GetPath(network.Source, network.Sink);
					path = ff.PathFromSourceToSink;
				}
				else
					path = null;
				
			}
		}
		/// <summary>
		/// Increases flow at specific edges. 
		/// </summary>
		/// <param name="network">The network in which the flow is increasing.</param>
		/// <param name="edgesFromReserveNetwork">Edges on which the flow will be increased.</param>
		/// <returns></returns>
		async Task ImproveFlow(FlowNetwork<BfsVertex> network, List<FlowEdge<BfsVertex>> edgesFromReserveNetwork)
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.ImproveFlow;
			for (int i = 0; i < edgesFromReserveNetwork.Count; i++)
			{
				var edge = network.GetEdge(edgesFromReserveNetwork[i].Name);
				if (edge != null)
				{
					((FlowEdge<BfsVertex>)edge).Flow += edgesFromReserveNetwork[i].Flow;
					await UpdateEdge(network,(FlowEdge<BfsVertex>)edge);
				}
					
				var oppositeEdge = network.GetEdge(edgesFromReserveNetwork[i].To, edgesFromReserveNetwork[i].From);
				if(oppositeEdge != null)
				{
					((FlowEdge<BfsVertex>)oppositeEdge).Flow -= edgesFromReserveNetwork[i].Flow;
					await UpdateEdge(network,(FlowEdge<BfsVertex>)oppositeEdge);
				}
			}
		}
		/// <summary>
		/// Highlight an ongoing change on a specific edge.
		/// </summary>
		/// <param name="network"></param>
		/// <param name="edge"></param>
		/// <returns></returns>
		async Task UpdateEdge(FlowNetwork<BfsVertex> network, FlowEdge<BfsVertex> edge)
		{
			ColorEdge(network, edge);
			(edge).UpdateCurrentFlowInfo();
			await Delay(delay);
			UncolorEdge(network,edge);
		}

		void PrintVertexRemovedFromReserveNetwork(BfsVertex vertex) => outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.NLVertexSpace + vertex.Name + CepheusProjectWpf.Properties.Resources.RemovedFromReserveNet;
		void PrintEdgeRemovedFromReserveNetwork(FlowEdge<BfsVertex> edge) => outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.NLEdgeSpace + edge.From.Name+"->"+edge.To.Name + CepheusProjectWpf.Properties.Resources.RemovedFromReserveNet;

		void PrintLookingForPath() => outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.SearchingPath;
	 }
}
