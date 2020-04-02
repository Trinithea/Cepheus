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

	class TreeWithContextComponents<TVertex> where TVertex : VertexBase<TVertex>
	{
		public List<TVertex> Vertices = new List<TVertex>();
		public Dictionary<string, EdgeWithLength<TVertex>> Edges = new Dictionary<string, EdgeWithLength<TVertex>>();
		public List<EdgeWithLength<TVertex>> NewEdges = new List<EdgeWithLength<TVertex>>();
		public List<Tree<TVertex>> ContextComponents = new List<Tree<TVertex>>();
	}
}
