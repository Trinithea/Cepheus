﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTestCepheusAlgorithms")]
namespace Cepheus
{
	abstract class Graph { }
	class Graph<TVertex> : Graph where TVertex : VertexBase<TVertex>
	{
		public Graph()
		{
			Edges = new Dictionary<string, Edge<TVertex>>();
			Vertices = new Dictionary<string, TVertex>();
		}
		public Dictionary<string, Edge<TVertex>> Edges { get; private set; }
		public Dictionary<string, TVertex> Vertices { get; private set; }
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
			Edges.Add(from.Name+to.Name, edge);
		}
		public void AddEdgeWithLength(string name, TVertex from, TVertex to, int length)
		{
			EdgeWithLength<TVertex> edge = new EdgeWithLength<TVertex>();
			edge.Name = name;
			edge.From = from;
			edge.To = to;
			edge.Length = length;
			from.OutEdges.Add(edge);
			to.InEdges.Add(edge);
			Edges.Add(from.Name + to.Name, edge);
		}

		public void InitializeVertices()
		{
			foreach (TVertex vertex in Vertices.Values)
			{
				vertex.Initialize();
			}

		}
		public TVertex GetVertex(string name)
		{
			if (Vertices.ContainsKey(name))
				return Vertices[name];
			else
				return null;
		}

		public Edge<TVertex> GetEdge(string fromNameToName)
		{
			if (Edges.ContainsKey(fromNameToName))
				return Edges[fromNameToName];
			else
				return null;
		}

		public Edge<TVertex> GetEdge(TVertex from, TVertex to)
		{
			string name = from.Name + to.Name;
			if (Edges.ContainsKey(name))
				return Edges[name];
			else
				return null;
		}
		public SortedList<int,EdgeWithLength<TVertex>> GetEdgesSortedByLength()
		{
			var edges = new SortedList<int, EdgeWithLength<TVertex>>();
			foreach (EdgeWithLength<TVertex> edge in Edges.Values)
				edges.Add(edge.Length, edge);
			return edges;
		}
	}
}
