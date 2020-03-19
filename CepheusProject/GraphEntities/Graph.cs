using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTestCepheusAlgorithms")]
namespace Cepheus
{
	class Graph<T> where T : Vertex
	{
		public Dictionary<string, Edge<T>> Edges = new Dictionary<string, Edge<T>>();
		public Dictionary<string, T> Vertices = new Dictionary<string, T>();
		public void AddVertex(T vertex)
		{

		}

		public void AddEdge(T from, T to)
		{
			
		}
		
	}
}
