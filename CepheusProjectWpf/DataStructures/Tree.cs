﻿using System;
using System.Collections.Generic;
using System.Text;
using Cepheus;

namespace Cepheus.DataStructures
{
	public class Tree<TVertex> where TVertex : VertexBase<TVertex>
	{
		public List<TVertex> Vertices = new List<TVertex>();
		public List<Edge<TVertex>> Edges = new List<Edge<TVertex>>();
	}
	public class ComponentTree<TVertex> : Tree<TVertex> where TVertex : VertexBase<TVertex>
	{
		/// <summary>
		/// ID of this component.
		/// </summary>
		public int ID { get; set; }
	}

	public class TreeWithContextComponents<TVertex> where TVertex : VertexBase<TVertex>
	{
		public List<TVertex> Vertices = new List<TVertex>();
		public Dictionary<string, Edge<TVertex>> Edges = new Dictionary<string, Edge<TVertex>>();
		/// <summary>
		/// Edges that have been newly added to the tree.
		/// </summary>
		public List<Edge<TVertex>> NewEdges = new List<Edge<TVertex>>();
		/// <summary>
		/// Edges to be erased.
		/// </summary>
		public List<Edge<TVertex>> EdgesToRemove = new List<Edge<TVertex>>();
		public Dictionary<int,ComponentTree<TVertex>> ContextComponents = new Dictionary<int, ComponentTree<TVertex>>();

		// this method is here for algorithm unit tests
		public int GetWeight()
		{
			int sum = 0;
			foreach (Edge<TVertex> edge in Edges.Values)
				sum += edge.Length;
			return sum;
		}
	}
}
