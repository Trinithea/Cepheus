using System;
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
		
	}
	abstract class VertexBase<T> : Vertex where T : Vertex
	{
		public new List<Edge<T>> OutEdges = new List<Edge<T>>();
		public new List<Edge<T>> InEdges = new List<Edge<T>>();
		public VertexBase(string name) : base(name) { }
		public abstract void Initialize();

	}

	interface IStateVertex 
	{
		public enum States { Open, Closed, Unvisited };
		public States State { get; set; }
	}


	class BfsVertex : VertexBase<BfsVertex>,IStateVertex
	{
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
		public override void Initialize()
		{
			State = IStateVertex.States.Unvisited;
			Distance = null;
			Predecessor = null;
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
		public override void Initialize()
		{
			State = IStateVertex.States.Unvisited;
			InTime = null;
			OutTime = null;
		}
	}

	// Dijkstra vertex is same as BFS vertex
	// Bellman-Ford could be BFS vertex also
	
	class FloydWarschallVertex : VertexBase<FloydWarschallVertex>
	{ 
		public FloydWarschallVertex (string name) : base(name) { }

		public int ID { get; set; }
		public override void Initialize()
		{
		}

	class JarnikVertex : VertexBase<JarnikVertex>
	{
		
			public enum States { Inside, Neighboring, Outside}
		public States State { get; set; }
		
		public override void Initialize()
		{
				State = States.Outside;

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