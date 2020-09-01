using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf;

namespace Cepheus
{
	
	public class Goldberg : FlowAlgorithm<GoldbergVertex>
	{
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override string Name => "Goldberg's algorithm";

		public override string TimeComplexity => "O(n^2 * m)";
		List<GoldbergVertex> positiveSurplusVertices;

		public override string Description => "In mathematical optimization, the push–relabel algorithm (alternatively, preflow–push algorithm) is an algorithm for computing maximum flows in a flow network. The name \"push–relabel\" comes from the two basic operations used in the algorithm. Throughout its execution, the algorithm maintains a \"preflow\" and gradually converts it into a maximum flow by moving flow locally between neighboring nodes using push operations under the guidance of an admissible network maintained by relabel operations. In comparison, the Ford–Fulkerson algorithm performs global augmentations that send flow following paths from the source all the way to the sink.";
		bool transfered=false;
		public async Task Run()
		{
			outputConsole.Text += "\nIn brackets after name of a vertex you see its height.";
			
			graph.InitializeVertices();
			graph.Source.Height = graph.Vertices.Count;
			InitializeVerticesNameTextBox();

			graph.InitializeEdges();
			await InitializeEdgesFromSource(graph.Source);
			PrintEdgesAreInitialized();

			await GetVerticesWithPositiveSurplus();
			PrintVerticesInitialized(graph);

			while (positiveSurplusVertices.Count > 0)
			{
				var vertex = positiveSurplusVertices[positiveSurplusVertices.Count - 1]; //last item for faster removing
				await TransferSurplus(vertex);
				if (!transfered) // if doesn't exist an edge with postive reserve and from.Height > to.Height
				{
					vertex.Height++;
					vertex.UpdateHeightInName();
				}
			}

			MaximumFlow = graph.GetMaximumFlow();
			PrintMaximumFlow();
		}
		void InitializeVerticesNameTextBox()
		{
			foreach(var vertex in graph.UltimateVertices)
			{
				vertex.Key.txtName = vertex.Value.txtName;
				vertex.Key.UpdateHeightInName();
			}
				
		}
		async Task InitializeEdgesFromSource(GoldbergVertex source)
		{
			ColorVertex(graph.Source);
			foreach (FlowEdge<GoldbergVertex> edge in source.OutEdges)
			{
				await Task.Delay(delay);
				ColorEdge(edge);
				edge.Flow = edge.Capacity;
				edge.UpdateCurrentFlowInfo();
			}
			await Task.Delay(delay);
			UncolorVertex(graph.Source);
			foreach (FlowEdge<GoldbergVertex> edge in source.OutEdges)
				UncolorEdge(edge);

		}
		async Task GetVerticesWithPositiveSurplus()
		{
			outputConsole.Text += "\nMarking vertices with positive surplus...";
			var vertices = new List<GoldbergVertex>();
			foreach (GoldbergVertex vertex in graph.Vertices.Values)
				if (vertex != graph.Source && vertex != graph.Sink && vertex.UpdateSurplus() > 0)
				{
					await Task.Delay(delay);
					vertices.Add(vertex);
					ColorVertex(vertex);
				}
			positiveSurplusVertices = vertices;
		}
		async Task TransferSurplus(GoldbergVertex from)
		{
			outputConsole.Text += "\nTransfering surplus...";
			outputConsole.Text += "\nVertices with positive surplus are staying marked.";
			transfered = false;
			foreach (FlowEdge<GoldbergVertex> edge in from.OutEdges)
			{
				if(edge.Reserve > 0 && from.Height > edge.To.Height) 
				{
					await Task.Delay(delay);
					ColorEdge(edge);
					int delta = Math.Min(from.Surplus, edge.Reserve);
					edge.Flow += delta;
					edge.UpdateCurrentFlowInfo();
					transfered = true;
					await Task.Delay(delay - 250);
					ColorVertex(edge.To);
					outputConsole.Text += "\nTransfering "+delta+" on edge " + from.Name + "->" + edge.To.Name;
					edge.To.Surplus += delta;
					PrintVertex(edge.To);
					from.Surplus -= delta;
					PrintVertex(from);

					if (from.Surplus <= 0)
					{
						positiveSurplusVertices.RemoveAt(positiveSurplusVertices.Count - 1); // that's from position
						await Task.Delay(delay);
						UncolorVertex(from);
					}
						

					if (edge.To.Surplus > 0 && !positiveSurplusVertices.Contains(edge.To) && edge.To != graph.Sink && edge.To != graph.Source)
						positiveSurplusVertices.Add(edge.To);
					else
					{
						await Task.Delay(delay);
						UncolorVertex(edge.To);
					}
					UncolorEdge(edge);
					break;
				}
				if (!transfered)
					outputConsole.Text += "\nNothing was transfered.";
			}
		}
	}
}
