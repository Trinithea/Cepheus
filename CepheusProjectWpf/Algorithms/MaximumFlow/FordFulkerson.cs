using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf;
namespace Cepheus
{
	public class FordFulkerson : FlowAlgorithm<BfsVertex>
	{
		public override bool IsFlowAlgorithm => true;
		public override bool NeedsOnlyNonNegativeEdgeLenghts => true;
		public override bool DontNeedInitialVertex => false;
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override string Name => CepheusProjectWpf.Properties.Resources.FFAlgo;

		public override string TimeComplexity => CepheusProjectWpf.Properties.Resources.FFTime;


		public override string Description => CepheusProjectWpf.Properties.Resources.FFDesc;
		public List<FlowEdge<BfsVertex>> PathFromSourceToSink;
		/// <summary>
		/// The main method of Ford-Fulkerson's algorithm. This is where the whole calculation takes place.
		/// </summary>
		/// <returns></returns>
		public async Task Run()
		{
			graph.InitializeEdges();
			PrintEdgesAreInitialized();

			
			await GetUnsaturatedPathFromSourceToSink();

			while(PathFromSourceToSink != null) // The path must be unsaturated and may not be the shortest
			{
				int minimalReserve = GetMinimalReserve();
				await ImproveFlowOnPath(minimalReserve);
				await GetUnsaturatedPathFromSourceToSink();
			}
			MaximumFlow = graph.GetMaximumFlow();
			PrintMaximumFlow();
		}
		/// <summary>
		/// Returns the smallest edge reserve on the path from the source to the sink.
		/// </summary>
		/// <returns></returns>
		int GetMinimalReserve()
		{
			int min = PathFromSourceToSink[0].Reserve;
			var minEdge = PathFromSourceToSink[0];
			for (int i = 0; i < PathFromSourceToSink.Count; i++)
				if (PathFromSourceToSink[i].Reserve < min)
				{
					min = PathFromSourceToSink[i].Reserve;
					minEdge= PathFromSourceToSink[i];
				}
			
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.LowestReserve + min + CepheusProjectWpf.Properties.Resources.OfTheEdge + minEdge.From.Name+"->"+minEdge.To.Name;
			return min;
		}
		/// <summary>
		/// Improves the flow by minimalReserve in the argument on the path from source to sink.
		/// </summary>
		/// <param name="minimalReserve"></param>
		/// <returns></returns>
		async Task ImproveFlowOnPath(int minimalReserve)
		{
			for (int i = 0; i < PathFromSourceToSink.Count; i++)
			{
				var edge = PathFromSourceToSink[i];
				int delta = Math.Min(edge.OppositeEdge.Flow, minimalReserve);
				edge.OppositeEdge.Flow -= delta;
				edge.Flow += minimalReserve - delta;
				await Task.Delay(delay);
				edge.UpdateCurrentFlowInfo();
			}
		}
		/// <summary>
		/// Finds the unsaturated path from the source to the drain and saves it to PathFromSourceToSink.
		/// </summary>
		/// <returns></returns>
		public async Task GetUnsaturatedPathFromSourceToSink()
		{
			// why I don't use BFS algorithm which is already implemeted? Because I need edge.Reserve in condition...
			outputConsole.Text += "\n\n" + CepheusProjectWpf.Properties.Resources.SearchingPath;

			graph.InitializeVertices();
			foreach (FlowEdge<BfsVertex> edge in graph.Edges.Values)
					UncolorEdge(graph,edge);

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
						await Task.Delay(delay);
						ColorEdge(graph,edge);
						await Task.Delay(delay - 250);
						ColorVertex(graph,edge.To);
						edge.To.State = States.Open;
						edge.To.Distance = vertex.Distance + 1;
						edge.To.Predecessor = vertex;
						queue.Enqueue(edge.To);
					}
				}
				vertex.State = States.Closed;
				UncolorVertex(graph,vertex);
				foreach (FlowEdge<BfsVertex> edge in vertex.OutEdges)
					UncolorEdge(graph,edge);
			}

			await GetPath( graph.Source, graph.Sink);
		}
		/// <summary>
		/// From the predecessors, it constructs a path from the vertex 'from' to the vertex 'to' (method arguments) and stores it in PathFromSourceToSink.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public async Task GetPath( BfsVertex from, BfsVertex to)
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.ConstructingPath;
			if (to.Predecessor == null) //'to' is not reachable from 'from'
			{
				PathFromSourceToSink = null;
				outputConsole.Text += "\n"+CepheusProjectWpf.Properties.Resources.SinkNotReachable;
			}
			else
			{
				var currentVertex = to;
				var path = new List<FlowEdge<BfsVertex>>();
				while (currentVertex.Predecessor != null)
				{
					var edge = (FlowEdge<BfsVertex>)graph.GetEdge(currentVertex.Predecessor, currentVertex);
					path.Insert(0, edge); // Because we compose the path from the end, it must always add more edges to the beginning of the list.
					await Task.Delay(delay);
					ColorEdge(graph,edge);
					currentVertex = currentVertex.Predecessor;
				}
				PathFromSourceToSink = path;
			}
		}
	}
}
