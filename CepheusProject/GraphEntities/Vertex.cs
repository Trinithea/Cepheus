using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cepheus
{
	
	abstract class Vertex
	{
		public List<Edge<Vertex>> OutEdges = new List<Edge<Vertex>>();
		public List<Edge<Vertex>> InEdges = new List<Edge<Vertex>>();
		public string Name { get; }
		public Vertex(string name)
		{
			Name = name;
		}
	}

	class StateVertex : Vertex
	{
		public StateVertex(string name) : base(name) { }
		public enum States { Open, Closed, Unvisited };
		public States State = States.Unvisited;
	}

	class BfsVertex : StateVertex
	{
		public List<Edge<BfsVertex>> OutEdges = new List<Edge<BfsVertex>>();
		public List<Edge<BfsVertex>> InEdges = new List<Edge<BfsVertex>>();
		public BfsVertex(string name) :base(name) 
		{
			
			State = States.Unvisited;
			Predecessor = null;
			Distance = null;
		}
		public BfsVertex Predecessor { get; set; }
		public int? Distance = null;
	}

	class DfsVertex : StateVertex
	{
		public DfsVertex(string name) : base(name) { }
		public int? InTime = null;
		public int? OutTime = null;
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