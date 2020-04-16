using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	
	class Goldberg : IAlgorithm
	{
		public string Name => "Goldberg's algorithm";

		public string TimeComplexity => "O(n^2 * m)";

		public int MaximalFlow { get; private set; }

		public void Run(FlowNetwork<GoldbergVertex> graph, GoldbergVertex source) //TODO is this source necessary
		{
			graph.InitializeVertices();
			graph.Source.Height = graph.Vertices.Count;
			graph.InitializeEdges();
			InitializeEdgesFromSource(graph.Source);

			var positiveSurplusVertices = GetVerticesWithPositiveSurplus(graph);

			while (positiveSurplusVertices.Count > 0)
			{
				var vertex = positiveSurplusVertices[positiveSurplusVertices.Count - 1]; //last item for faster removing
				if (!TransferSurplus(vertex, vertex.GetSurplus())) // if doesn't exist an edge with postive reserve and from.Height > to.Height
					vertex.Height++;
				if (vertex.GetSurplus() <= 0)
					positiveSurplusVertices.RemoveAt(positiveSurplusVertices.Count - 1);
			}

			MaximalFlow = graph.GetMaximalFlow();
		}
		void InitializeEdgesFromSource(GoldbergVertex source)
		{
			foreach (FlowEdge<GoldbergVertex> edge in source.OutEdges)
				edge.Flow = edge.Capacity;
		}
		List<GoldbergVertex> GetVerticesWithPositiveSurplus(FlowNetwork<GoldbergVertex> graph)
		{
			var vertices = new List<GoldbergVertex>();
			foreach (GoldbergVertex vertex in graph.Vertices.Values)
				if (vertex != graph.Source && vertex != graph.Sink && vertex.GetSurplus() > 0)
					vertices.Add(vertex);

			return vertices;
		}
		//TODO maybe there could be a datafield graph so it won't be necessary to have graph in every argument
		bool TransferSurplus(GoldbergVertex from, int fromSurplus)
		{
			bool transfered=false;
			foreach(FlowEdge<GoldbergVertex> edge in from.OutEdges)
			{
				if(edge.Reserve > 0 && from.Height > edge.To.Height) 
				{
					int delta = Math.Min(fromSurplus, edge.Reserve);
					edge.Flow += delta;
					transfered = true;
				}
			}
			return transfered;
		}
	}
}
