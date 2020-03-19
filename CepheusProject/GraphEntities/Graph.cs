using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTestCepheusAlgorithms")]
namespace Cepheus
{
	class Graph<TVertex> where TVertex : VertexBase<TVertex>
	{
		private Dictionary<string, Edge<TVertex>> Edges = new Dictionary<string, Edge<TVertex>>();
		private Dictionary<string, TVertex> Vertices = new Dictionary<string, TVertex>();
		public void AddVertex(TVertex vertex)
		{
			Vertices.Add(vertex.Name,vertex);
		}

		public void AddEdge(string name, TVertex from, TVertex to)
		{
			Edge<TVertex> edge = new Edge<TVertex>();
			edge.Name = name;
			edge.From = from;
			edge.To = to;
			from.OutEdges.Add(edge);
			to.InEdges.Add(edge);
			Edges.Add(name, edge);
		}

		public TVertex GetVertex(string name) => Vertices[name];
		public Edge<TVertex> GetEdge(string name) => Edges[name];

	}
}
