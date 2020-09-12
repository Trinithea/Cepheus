using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf;

namespace Cepheus
{
	
	public class Goldberg : FlowAlgorithm<GoldbergVertex>
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
		public override string Name => CepheusProjectWpf.Properties.Resources.GoldbergAlgo;

		public override string TimeComplexity => CepheusProjectWpf.Properties.Resources.GoldbergTime;
		List<GoldbergVertex> positiveSurplusVertices;

		public override string Description => CepheusProjectWpf.Properties.Resources.GoldbergDesc;
		bool transfered=false;
		public async Task Run()
		{
			outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.BracketsHeight;
			
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
			ColorVertex(graph,graph.Source);
			foreach (FlowEdge<GoldbergVertex> edge in source.OutEdges)
			{
				await Task.Delay(delay);
				ColorEdge(graph,edge);
				edge.Flow = edge.Capacity;
				edge.UpdateCurrentFlowInfo();
			}
			await Task.Delay(delay);
			UncolorVertex(graph,graph.Source);
			foreach (FlowEdge<GoldbergVertex> edge in source.OutEdges)
				UncolorEdge(graph,edge);

		}
		async Task GetVerticesWithPositiveSurplus()
		{
			outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.MarkingSurplus;
			var vertices = new List<GoldbergVertex>();
			foreach (GoldbergVertex vertex in graph.Vertices.Values)
				if (vertex != graph.Source && vertex != graph.Sink && vertex.UpdateSurplus() > 0)
				{
					vertices.Add(vertex);
					ColorVertex(graph,vertex);
					await Task.Delay(delay);
				}
			positiveSurplusVertices = vertices;
		}
		async Task TransferSurplus(GoldbergVertex from)
		{
			outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.TransferingSurplus;
			outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.VerticisSurplus;
			transfered = false;
			foreach (FlowEdge<GoldbergVertex> edge in from.OutEdges)
			{
				if(edge.Reserve > 0 && from.Height > edge.To.Height) 
				{
					await Task.Delay(delay);
					ColorEdge(graph,edge);
					int delta = Math.Min(from.Surplus, edge.Reserve);
					edge.Flow += delta;
					edge.UpdateCurrentFlowInfo();
					transfered = true;
					await Task.Delay(delay - 250);
					ColorVertex(graph,edge.To);
					outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.NLTransfering+ delta+ CepheusProjectWpf.Properties.Resources.OnEdge + from.Name + "->" + edge.To.Name;
					edge.To.Surplus += delta;
					edge.To.UpdateHeightInName();
					PrintVertex(edge.To);
					from.Surplus -= delta;
					from.UpdateHeightInName();
					PrintVertex(from);

					if (from.Surplus <= 0)
					{
						positiveSurplusVertices.RemoveAt(positiveSurplusVertices.Count - 1); // that's from position
						await Task.Delay(delay);
						UncolorVertex(graph,from);
					}
						

					if (edge.To.Surplus > 0 && !positiveSurplusVertices.Contains(edge.To) && edge.To != graph.Sink && edge.To != graph.Source)
						positiveSurplusVertices.Add(edge.To);
					else
					{
						await Task.Delay(delay);
						UncolorVertex(graph,edge.To);
					}
					UncolorEdge(graph,edge);
					break;
				}
				if (!transfered)
					outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.NothingTransfered;
			}
		}
	}
}
