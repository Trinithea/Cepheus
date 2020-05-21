using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cepheus
{
	public class Vertex
	{
		public List<Edge<Vertex>> OutEdges = new List<Edge<Vertex>>();
		public List<Edge<Vertex>> InEdges = new List<Edge<Vertex>>();

		public int UniqueId { get; set; }
		
		
	}
	public abstract class VertexBase<T> : Vertex where T : Vertex
	{
		public new List<Edge<T>> OutEdges = new List<Edge<T>>();
		public new List<Edge<T>> InEdges = new List<Edge<T>>();
		public abstract void Initialize();
		
	}
	public enum States { Open, Closed, Unvisited };
	interface IStateVertex
	{
		States State { get; set; }
	}


	public class BfsVertex : VertexBase<BfsVertex>, IStateVertex
	{
		public BfsVertex()
		{
			State = States.Unvisited;
			Predecessor = null;
			Distance = null;
		}
		public BfsVertex Predecessor { get; set; }
		public States State { get; set; }

		public int? Distance = null;

		public override void Initialize()
		{
			State = States.Unvisited;
			Distance = null;
			Predecessor = null;
		}
	}

	public class DfsVertex : VertexBase<DfsVertex>, IStateVertex
	{
		public DfsVertex()
		{
			State =States.Unvisited;
		}
		public int? InTime = null;
		public int? OutTime = null;

		public States State { get; set; }
		public override void Initialize()
		{
			State = States.Unvisited;
			InTime = null;
			OutTime = null;
		}
	}

	// Dijkstra vertex is same as BFS vertex
	// Bellman-Ford could be BFS vertex also

	public class FloydWarshallVertex : VertexBase<FloydWarshallVertex>
	{

		public int ID { get; set; }
		public override void Initialize()
		{
		}
	}
	public class JarnikVertex : VertexBase<JarnikVertex>
	{
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

	public class BoruvkaVertex : VertexBase<BoruvkaVertex>
	{
		public int ComponentID { get; set; }

		public override void Initialize()
		{
			OutEdges.Sort((x, y) => ((Edge<BoruvkaVertex>)x).Length.CompareTo(((Edge<BoruvkaVertex>)y).Length));
		}
	}


	//TODO delete this
	public class FlowVertex : VertexBase<FlowVertex>
	{
		public override void Initialize() { }
	}

	public class GoldbergVertex : VertexBase<GoldbergVertex>
	{
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
		
	}
}
