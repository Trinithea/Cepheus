﻿using System;
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
	class ComponentTree<TVertex> : Tree<TVertex> where TVertex : VertexBase<TVertex>
	{
		public int ID { get; set; }
	}

	class TreeWithContextComponents<TVertex> where TVertex : VertexBase<TVertex>
	{
		public List<TVertex> Vertices = new List<TVertex>();
		public Dictionary<string, Edge<TVertex>> Edges = new Dictionary<string, Edge<TVertex>>();
		public List<Edge<TVertex>> NewEdges = new List<Edge<TVertex>>();
		public List<Edge<TVertex>> EdgesToRemove = new List<Edge<TVertex>>();
		public Dictionary<int,ComponentTree<TVertex>> ContextComponents = new Dictionary<int, ComponentTree<TVertex>>();

		public int GetWeight()
		{
			int sum = 0;
			foreach (Edge<TVertex> edge in Edges.Values)
				sum += edge.Length;
			return sum;
		}
	}
}
