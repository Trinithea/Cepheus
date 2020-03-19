﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cepheus
{
	class Vertex 
	{
		public List<Edge<Vertex>> OutEdges = new List<Edge<Vertex>>();
		public List<Edge<Vertex>> InEdges = new List<Edge<Vertex>>();
		public string Name { get; }
		public Vertex(string name)
		{
			Name = name;
		}
		public void AddToOutEdges(Edge<Vertex> edge)
		{

		}
	}
	abstract class VertexBase<T> : Vertex where T : Vertex
	{
		public List<Edge<T>> OutEdges = new List<Edge<T>>();
		public List<Edge<T>> InEdges = new List<Edge<T>>();
		public string Name { get; }
		public VertexBase(string name) : base(name) { }
		
		
	}

	interface IStateVertex 
	{
		public enum States { Open, Closed, Unvisited };
		public States State { get; set; }
	}


	class BfsVertex : VertexBase<BfsVertex>,IStateVertex
	{
		public List<Edge<BfsVertex>> OutEdges = new List<Edge<BfsVertex>>();
		public List<Edge<BfsVertex>> InEdges = new List<Edge<BfsVertex>>();
		public BfsVertex(string name) :base (name)
		{
			State = IStateVertex.States.Unvisited;
			Predecessor = null;
			Distance = null;
		}
		public BfsVertex Predecessor { get; set; }
		public IStateVertex.States State { get; set; }

		public int? Distance = null;
		public void AddToOutEdges(Edge<BfsVertex> edge)
		{
			OutEdges.Add(edge);
		}
	}

	class DfsVertex : VertexBase<DfsVertex>,IStateVertex
	{
		public DfsVertex(string name) : base(name) 
		{
			State = IStateVertex.States.Unvisited;
		}
		public int? InTime = null;
		public int? OutTime = null;

		public IStateVertex.States State { get; set; }
	}

	// Dijkstra vertex is same as BFS vertex
	// Bellman-Ford could be BFS vertex also
	
	class FloydWarschallVertex : Vertex
	{ 
		public FloydWarschallVertex (string name) : base(name) { }
		// TODO some ID 
	}

	class JarnikVertex
	{
		public string Name { get; }
		public enum States { Inside, Neighboring, Outside}
		public States State { get; set; }
		public JarnikVertex(string name)
		{
			Name = name;
			State = States.Neighboring;
		}
	}

	class TreeVertex 
	{
		public TreeVertex(string name)
		{
			//Name = name;
		}
		public List<TreeVertex> Sons = new List<TreeVertex>();
	}
}