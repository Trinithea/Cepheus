using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf;
namespace Cepheus
{
	public class FordFulkerson : FlowAlgorithm<BfsVertex>
	{
		
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override string Name => "Ford-Fulkerson's algorithm";

		public override string TimeComplexity => "O(m * f)";


		public override string Description => "The Ford–Fulkerson method or Ford–Fulkerson algorithm (FFA) is a greedy algorithm that computes the maximum flow in a flow network. It is sometimes called a \"method\" instead of an \"algorithm\" as the approach to finding augmenting paths in a residual graph is not fully specified or it is specified in several implementations with different running times. It was published in 1956 by L. R. Ford Jr. and D. R. Fulkerson. The name \"Ford–Fulkerson\" is often also used for the Edmonds–Karp algorithm, which is a fully defined implementation of the Ford–Fulkerson method. ";
		public List<FlowEdge<BfsVertex>> PathFromSourceToSink;
		public async Task Run()
		{
			graph.InitializeEdges();
			PrintEdgesAreInitialized();

			
			await GetUnsaturatedPathFromSourceToSink();

			while(PathFromSourceToSink != null ) // must be nenasycená and nemusí být nejkratší
			{
				int minimalReserve = GetMinimalReserve();
				await ImproveFlowOnPath(minimalReserve);
				await GetUnsaturatedPathFromSourceToSink();
			}
			MaximumFlow = graph.GetMaximumFlow();
			PrintMaximumFlow();
		}
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
			
			outputConsole.Text += "\nThe lowest reserve is " + min + " of the edge " + minEdge.From.Name+"->"+minEdge.To.Name;
			return min;

		}
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
		public async Task GetUnsaturatedPathFromSourceToSink()
		{
			// why I don't use BFS algorithm which is already implemeted? Because I need edge.Reserve in condition...
			outputConsole.Text += "\n\nLooking for a path from source to sink by BFS algorithm...";

			graph.InitializeVertices();
			foreach (FlowEdge<BfsVertex> edge in graph.Edges.Values)
					UncolorEdge(edge);

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
						ColorEdge(edge);
						await Task.Delay(delay - 250);
						ColorVertex(edge.To);
						edge.To.State = States.Open;
						edge.To.Distance = vertex.Distance + 1;
						edge.To.Predecessor = vertex;
						queue.Enqueue(edge.To);
					}
				}
				vertex.State = States.Closed;
				UncolorVertex(vertex);
				foreach (FlowEdge<BfsVertex> edge in vertex.OutEdges)
					UncolorEdge(edge);
			}

			await GetPath( graph.Source, graph.Sink);
		}

		public async Task GetPath( BfsVertex from, BfsVertex to)
		{
			outputConsole.Text += "\nConstructing the path from source to sink...";
			if (to.Predecessor == null) //'to' is not reachable from 'from'
			{
				PathFromSourceToSink = null;
				outputConsole.Text += "\nThe sink is not reachable from the source.";
			}
			else
			{
				var currentVertex = to;
				var path = new List<FlowEdge<BfsVertex>>();
				while (currentVertex.Predecessor != null)
				{
					var edge = (FlowEdge<BfsVertex>)graph.GetEdge(currentVertex.Predecessor, currentVertex);
					path.Insert(0, edge);
					await Task.Delay(delay);
					ColorEdge(edge);
					currentVertex = currentVertex.Predecessor;
				}
				PathFromSourceToSink = path;
			}
		}
	}
}
