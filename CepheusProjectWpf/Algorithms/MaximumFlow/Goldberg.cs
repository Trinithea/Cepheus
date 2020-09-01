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


		public override string Description => "In mathematical optimization, the push–relabel algorithm (alternatively, preflow–push algorithm) is an algorithm for computing maximum flows in a flow network. The name \"push–relabel\" comes from the two basic operations used in the algorithm. Throughout its execution, the algorithm maintains a \"preflow\" and gradually converts it into a maximum flow by moving flow locally between neighboring nodes using push operations under the guidance of an admissible network maintained by relabel operations. In comparison, the Ford–Fulkerson algorithm performs global augmentations that send flow following paths from the source all the way to the sink.";

		public async Task Run()
		{
			graph.InitializeVertices();
			graph.Source.Height = graph.Vertices.Count;
			graph.InitializeEdges();
			InitializeEdgesFromSource(graph.Source);

			var positiveSurplusVertices = GetVerticesWithPositiveSurplus();

			while (positiveSurplusVertices.Count > 0)
			{
				var vertex = positiveSurplusVertices[positiveSurplusVertices.Count - 1]; //last item for faster removing
				if (!TransferSurplus(vertex, vertex.Surplus,positiveSurplusVertices)) // if doesn't exist an edge with postive reserve and from.Height > to.Height
					vertex.Height++;

			}

			MaximumFlow = graph.GetMaximumFlow();
		}
		void InitializeEdgesFromSource(GoldbergVertex source)
		{
			foreach (FlowEdge<GoldbergVertex> edge in source.OutEdges)
				edge.Flow = edge.Capacity;
		}
		List<GoldbergVertex> GetVerticesWithPositiveSurplus()
		{
			var vertices = new List<GoldbergVertex>();
			foreach (GoldbergVertex vertex in graph.Vertices.Values)
				if (vertex != graph.Source && vertex != graph.Sink && vertex.UpdateSurplus() > 0)
					vertices.Add(vertex);

			return vertices;
		}
		//TODO maybe there could be a datafield graph so it won't be necessary to have graph in every argument
		bool TransferSurplus(GoldbergVertex from, int fromSurplus, List<GoldbergVertex> positiveSurplusVertices)
		{
			bool transfered=false;
			foreach(FlowEdge<GoldbergVertex> edge in from.OutEdges)
			{
				if(edge.Reserve > 0 && from.Height > edge.To.Height) 
				{
					int delta = Math.Min(fromSurplus, edge.Reserve);
					edge.Flow += delta;
					transfered = true;
					edge.To.Surplus += delta;
					from.Surplus -= delta;
					if (from.Surplus <= 0)
						positiveSurplusVertices.RemoveAt(positiveSurplusVertices.Count - 1); // that's from position

					if (edge.To.Surplus > 0 && !positiveSurplusVertices.Contains(edge.To) && edge.To != graph.Sink&& edge.To != graph.Source )
						positiveSurplusVertices.Add(edge.To);
					break;
				}
			}
			return transfered;
		}
	}
}
