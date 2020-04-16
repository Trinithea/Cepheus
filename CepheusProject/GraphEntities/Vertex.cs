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
		public override string ToString()
		{
			return Name + " - in: " + InEdges.Count + ", out: " + OutEdges.Count;
		}
	}

	interface IStateVertex
	{
		public enum States { Open, Closed, Unvisited };
		public States State { get; set; }
	}


	class BfsVertex : VertexBase<BfsVertex>, IStateVertex
	{
		public BfsVertex(string name) : base(name)
		{
			State = IStateVertex.States.Unvisited;
			Predecessor = null;
			Distance = null;
		}
		public BfsVertex Predecessor { get; set; }
		public IStateVertex.States State { get; set; }

		public int? Distance = null;

		public override void Initialize()
		{
			State = IStateVertex.States.Unvisited;
			Distance = null;
			Predecessor = null;
		}
	}

	class DfsVertex : VertexBase<DfsVertex>, IStateVertex
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
		public FloydWarschallVertex(string name) : base(name) { }

		public int ID { get; set; }
		public override void Initialize()
		{
		}
	}
	class JarnikVertex : VertexBase<JarnikVertex>
	{
		public JarnikVertex(string name) : base(name) { }
		public enum States { Inside, Neighbour, Outside }
		public States State { get; set; }
		public int? Rating { get; set; }
		public JarnikVertex Predecessor { get; set; }
		public override void Initialize()
		{
			State = States.Outside;
			Rating = null;
			Predecessor = null;
		}
	}

	class BoruvkaVertex : VertexBase<BoruvkaVertex>
	{
		public int ComponentID { get; set; }
		public BoruvkaVertex(string name) : base(name) { }
		public override void Initialize()
		{
			OutEdges.Sort((x, y) => ((EdgeWithLength<BoruvkaVertex>)x).Length.CompareTo(((EdgeWithLength<BoruvkaVertex>)y).Length));
		}
	}


	//TODO delete this
	class FlowVertex : VertexBase<FlowVertex>
	{
		public FlowVertex(string name) : base(name) { }
		public override void Initialize() { }
	}

	class GoldbergVertex : VertexBase<GoldbergVertex>
	{
		public GoldbergVertex(string name) : base(name) { }
		public int Height { get; set; }
		public int Surplus { get; set; }
		public int UpdateSurplus()
		{
			int sum = 0;
			for (int i = 0; i < InEdges.Count; i++)
			{
				sum += ((FlowEdge<GoldbergVertex>)InEdges[i]).Flow;
			}
			for (int i = 0; i < OutEdges.Count; i++)
			{
				sum -= ((FlowEdge<GoldbergVertex>)OutEdges[i]).Flow;
			}
			Surplus = sum;
			return sum;
		}
		public override void Initialize() 
		{
			Height = 0;
		}
		public override string ToString()
		{
			return String.Format("{0} - surplus: {1}, height: {2}, in: {3}, out: {4}", Name, Surplus, Height, InEdges.Count, OutEdges.Count);
		}
	}
}
