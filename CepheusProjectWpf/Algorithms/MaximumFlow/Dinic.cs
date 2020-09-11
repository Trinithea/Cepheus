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
		Canvas netOfReservesCanvas;
		NetOfReservesWindow netOfReservesWindow;
		double windowWidth;
		double windowHeight;
		double leftDifference;
		double topDifference;
		BFS bfs;
		public override string Description => CepheusProjectWpf.Properties.Resources.DinicDescription;

		public async Task Run()
		{
			bfs = new BFS();
			netOfReservesWindow = new NetOfReservesWindow();
			netOfReservesCanvas = netOfReservesWindow.NetCanvas;
			GetDimensionsOfNetOfReservesWindow();
			netOfReservesWindow.Width = windowWidth;
			netOfReservesWindow.Height = windowHeight;
			while (true)
			{
				var reserveNetwork = ShowNetOfReservesWindow();
				bfs.Graph = reserveNetwork;
				bfs.initialVertex = reserveNetwork.Source;
				bfs.SetOutputConsole(outputConsole);
				PrintSearchingPath();
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

				//save edges in reserve network before they will be removed
				var edges = new List<FlowEdge<BfsVertex>>();
				foreach (var edge in reserveNetwork.Edges.Values)
					edges.Add((FlowEdge<BfsVertex>)edge);

				await GetBlockingFlow(reserveNetwork);
				await Task.Delay(2 * delay);
				netOfReservesWindow.Close();
				await ImproveFlow(graph,edges);

			}

			MaximumFlow = graph.GetMaximumFlow();
			PrintMaximumFlow();
		}
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
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.CreatingReserveNet;
			FlowNetwork<BfsVertex> reserveNetwork = new FlowNetwork<BfsVertex>() ;

			
			foreach (var vertex in graph.UltimateVertices)
			{
				BfsVertex newVertex = null;
				if (vertex.Key == graph.Source)
				{
					newVertex = reserveNetwork.AddVertex(graph.Source.UniqueId, graph.Source.Name);
					reserveNetwork.Source = newVertex;
				}		
				else if (vertex.Key == graph.Sink)
				{
					newVertex = reserveNetwork.AddVertex(graph.Sink.UniqueId, graph.Sink.Name);
					reserveNetwork.Sink = newVertex;
				}
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

		
		async Task CleanUpNetwork(FlowNetwork<BfsVertex> network)
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.CleaningReserveNet;
			bfs.Graph = network;
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.DividingVertices;
			await bfs.Run(); 

			await RemoveVerticesAfterSink(network);
			await Task.Delay(delay);

			await RemoveNotForwardEdges(network);
			await Task.Delay(delay);

			await RemoveVerticesWithZeroOutEdges(network);
			await Task.Delay(delay);
		}
		async Task RemoveVerticesAfterSink(FlowNetwork<BfsVertex> network)
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.RemovingFarVertices;
			PrintVertex(network.Sink);
			// remove every vertex after sink
			var verticesToRemove = new List<BfsVertex>();
			int? maxDistance = network.Sink.Distance;

			foreach (var vertex in network.Vertices.Values)
				if (vertex.Distance > maxDistance)
				{
					verticesToRemove.Add(vertex);
					ColorVertex(network,vertex);
					await Task.Delay(delay - 250);
				}

			for (int i = 0; i < verticesToRemove.Count; i++)
			{
				network.RemoveVertex(verticesToRemove[i]);
				PrintVertexRemovedFromReserveNetwork(verticesToRemove[i]);
			}
			verticesToRemove.Clear();
		}
		async Task RemoveNotForwardEdges(FlowNetwork<BfsVertex> network)
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.RemovingBackEdges;
			// remove every edge to previous layers or edges inside layers
			var edgesToRemove = new List<FlowEdge<BfsVertex>>();

			foreach (FlowEdge<BfsVertex> edge in network.Edges.Values)
				if (edge.To.Distance <= edge.From.Distance)
				{
					edgesToRemove.Add(edge);
					ColorEdge(network, edge);
					await Task.Delay(delay-250);
				}
					
					
			for (int i = 0; i < edgesToRemove.Count; i++)
			{
				network.RemoveEdge(edgesToRemove[i]);
				PrintEdgeRemovedFromReserveNetwork(edgesToRemove[i]);
			}
				
		}
		async Task RemoveVerticesWithZeroOutEdges(FlowNetwork<BfsVertex> network)
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.RemovingNoOutVertices;
			var verticesToRemove = new Queue<BfsVertex>();

			foreach (var vertex in network.Vertices.Values)
				if (vertex.OutEdges.Count == 0 && vertex != network.Sink && vertex != network.Source)
				{
					verticesToRemove.Enqueue(vertex);
					ColorVertex(network,vertex);
					await Task.Delay(delay);
				}

			while (verticesToRemove.Count > 0)
			{
				var vertex = verticesToRemove.Dequeue();
				var fromVertices = new List<BfsVertex>();
				for (int i = 0; i < vertex.InEdges.Count; i++)
					fromVertices.Add(vertex.InEdges[i].From);
				network.RemoveVertex(vertex);
				PrintVertexRemovedFromReserveNetwork(vertex);
				for (int i = 0; i < fromVertices.Count; i++)
					if (fromVertices[i].OutEdges.Count == 0)
					{
						verticesToRemove.Enqueue(fromVertices[i]);
						ColorVertex(network, fromVertices[i]);
						await Task.Delay(delay);
					}
			}
		}
		
		async Task GetBlockingFlow(FlowNetwork<BfsVertex> network)
		{
			network.SetFlowTo(0);
			bfs.Graph = network;
			bfs.initialVertex = network.Source;
			PrintSearchingPath();
			await bfs.Run();
			FordFulkerson ff = new FordFulkerson();
			ff.graph = network;
			ff.SetOutputConsole(outputConsole);
			await ff.GetPath(network.Source, network.Sink);
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
					outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.RemovingSaturatedEdges;
					path[i].Flow += min;
					path[i].UpdateCurrentFlowInfo();
					if (path[i].Flow == path[i].Capacity)
					{
						ColorEdge(network, path[i]);
						await Task.Delay(delay);
						network.RemoveEdge(path[i]);
						PrintEdgeRemovedFromReserveNetwork(path[i]);
					}
				}

				await CleanUpNetwork(network);
				bfs.Graph = network;
				bfs.initialVertex = network.Source;
				if (network.UltimateVertices.ContainsKey(network.Source))
				{
					PrintSearchingPath();
					await bfs.Run();
					ff.graph = network;
					await ff.GetPath(network.Source, network.Sink);
					path = ff.PathFromSourceToSink;
				}
				else
					path = null;
				
			}
		}
		
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
		async Task UpdateEdge(FlowNetwork<BfsVertex> network, FlowEdge<BfsVertex> edge)
		{
			ColorEdge(network, edge);
			(edge).UpdateCurrentFlowInfo();
			await Task.Delay(delay);
			UncolorEdge(network,edge);
		}

		void PrintVertexRemovedFromReserveNetwork(BfsVertex vertex) => outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.NLVertexSpace + vertex.Name + CepheusProjectWpf.Properties.Resources.RemovedFromReserveNet;
		void PrintEdgeRemovedFromReserveNetwork(FlowEdge<BfsVertex> edge) => outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.NLEdgeSpace + edge.From.Name+"->"+edge.To.Name + CepheusProjectWpf.Properties.Resources.RemovedFromReserveNet;
		void PrintSearchingPath() => outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.SearchingPath;
	 }
}
