using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTestCepheusAlgorithms")]
namespace Cepheus
{
	class Graph<TVertex,TEdge> where TVertex : Vertex where TEdge : Edge<Vertex>, new ()
	{
		private Dictionary<string, TEdge> Edges = new Dictionary<string, TEdge>();
		private Dictionary<string, TVertex> Vertices = new Dictionary<string, TVertex>();
		public void AddVertex(TVertex vertex)
		{
			Vertices.Add(vertex.Name,vertex);
		}

		public void AddEdge(string name, TVertex from, TVertex to)
		{
			TEdge edge = new TEdge();
			edge.Name = name;
			edge.From = from;
			edge.To = to;
			from.OutEdges.Add(edge);
			to.InEdges.Add(edge);
			Edges.Add(name, edge);
		}
		
	}
}
