using System;
using System.Collections.Generic;
using System.Text;
using Cepheus;

namespace Cepheus.DataStructures
{
	class Tree<TVertex> where TVertex : VertexBase<TVertex>
	{
		public List<TVertex> Vertices = new List<TVertex>();
		public List<Edge<TVertex>> Edges = new List<Edge<TVertex>>();
	}

}
