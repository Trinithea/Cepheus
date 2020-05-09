using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	
	class Goldberg : IAlgorithm
	{
		public string Name => "Goldberg's algorithm";

		public string TimeComplexity => "O(n^2 * m)";

		public int MaximumFlow { get; private set; }
		private FlowNetwork<GoldbergVertex> graph;

		public void Run(FlowNetwork<GoldbergVertex> graph, GoldbergVertex source) //TODO is this source necessary
		{
			graph.InitializeVertices();
			graph.Source.Height = graph.Vertices.Count;
			graph.InitializeEdges();
			InitializeEdgesFromSource(graph.Source);
			this.graph = graph;

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
