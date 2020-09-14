using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cepheus
{
	public static class IntExtensions
	{
		/// <summary>
		/// Prints the value of the passed argument. If it is the maximum int value, then it prints 'Infinity'.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public static string Print(this int i)
		{
			if (i == Int32.MaxValue)
				return CepheusProjectWpf.Properties.Resources.Infinity;
			else
				return i.ToString();
		}
		/// <summary>
		/// Prints the value of the passed argument. If it is a null value, then it prints 'Infinity'.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public static string Print(this int? i)
		{
			if (i == null)
				return CepheusProjectWpf.Properties.Resources.Infinity;
			else
				return i.ToString();
		}
	}
	public abstract class Vertex
	{
		/// <summary>
		/// Edges that come out of the vertex.
		/// </summary>
		public List<Edge<Vertex>> OutEdges = new List<Edge<Vertex>>();
		/// <summary>
		/// Edges that enter the vertex.
		/// </summary>
		public List<Edge<Vertex>> InEdges = new List<Edge<Vertex>>();
		/// <summary>
		/// A text box that the user sees above the vertex.
		/// </summary>
		public TextBox txtName;
		/// <summary>
		/// Current listing of vertex property values.
		/// </summary>
		public abstract string Informations { get; }
		/// <summary>
		/// Unique vertex ID identical to the vertex ID the user sees.
		/// </summary>
		public int UniqueId { get; set; }
		/// <summary>
		/// The name of the vertex set by the user.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Return name of predecessor vertex or Resources.None, if predecessor doesn't exist.
		/// </summary>
		/// <param name="predecessor"></param>
		/// <returns></returns>
		protected string GetPredecessorName(Vertex predecessor)
		{
			if (predecessor == null)
				return CepheusProjectWpf.Properties.Resources.None;
			else
				return predecessor.Name;
		}

	}
	public abstract class VertexBase<T> : Vertex where T : Vertex
	{
		/// <summary>
		/// Edges that come out of the vertex.
		/// </summary>
		public new List<Edge<T>> OutEdges = new List<Edge<T>>();
		/// <summary>
		/// Edges that enter the vertex.
		/// </summary>
		public new List<Edge<T>> InEdges = new List<Edge<T>>();
		/// <summary>
		/// Initializes the vertex accordingly to its needs.
		/// </summary>
		public abstract void Initialize();
		/// <summary>
		/// Correctly writes the state of the vertex.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public string GetStateName(States state) //should be in interface IStateVertex but the can!t be an explicit implementation in C# 7.3
		{
			switch (state)
			{
				case States.Closed:
					return CepheusProjectWpf.Properties.Resources.Closed;
				case States.Open:
					return CepheusProjectWpf.Properties.Resources.Open;
				default:
					return CepheusProjectWpf.Properties.Resources.Unvisited;
			}
		}
	}
	public enum States { Open, Closed, Unvisited };
	
	
	interface IStateVertex
	{
		/// <summary>
		/// Vertex current state (Open/Closed/Unvisited).
		/// </summary>
		States State { get; set; }
	}


	public class BfsVertex : VertexBase<BfsVertex>, IStateVertex
	{
		public BfsVertex()
		{
			State = States.Unvisited;
			Predecessor = null;
			Distance = Int32.MaxValue;
		}
		/// <summary>
		/// The vertex from which the algorithm came to this vertex.
		/// </summary>
		public BfsVertex Predecessor { get; set; }
		/// <summary>
		/// Adds the State and Distance from the initial vertex to the name in the text box above the vertex.
		/// </summary>
		public void UpdateVertexInfo() => txtName.Text = Name + " (" + GetStateName(State) + ", " + Distance.Print() + ")";
		public States State { get; set; }
		/// <summary>
		/// Highlights the edge that leads from its predecessor to this vertex.
		/// </summary>
		/// <param name="algorithm"></param>
		public void ColorEdgeWithPredecessor(Algorithm<BfsVertex> algorithm) => algorithm.ColorEdge(algorithm.Graph.GetEdge(Predecessor.UniqueId + "->" + UniqueId));
		/// <summary>
		/// Distance from the initial vertex.
		/// </summary>
		public int Distance { get; set; }

		public override void Initialize()
		{
			State = States.Unvisited;
			Distance = Int32.MaxValue;
			Predecessor = null;
			UpdateVertexInfo();
		}
		public override string Informations => "\n"+ CepheusProjectWpf.Properties.Resources.NLVertexSpace + Name + CepheusProjectWpf.Properties.Resources.HasState + GetStateName(State) + CepheusProjectWpf.Properties.Resources.IsInDistance + Distance.Print() + CepheusProjectWpf.Properties.Resources.FromInitVPred + GetPredecessorName(Predecessor);
	}

	public class DfsVertex : VertexBase<DfsVertex>, IStateVertex
	{
		public DfsVertex()
		{
			State =States.Unvisited;
		}
		/// <summary>
		/// The time when the vertex was opened.
		/// </summary>
		public int? InTime = null;
		/// <summary>
		/// The time when the vertex was closed.
		/// </summary>
		public int? OutTime = null;

		public States State { get; set; }
		public override void Initialize()
		{
			State = States.Unvisited;
			InTime = null;
			OutTime = null;
			UpdateVertexInfo();
		}
		/// <summary>
		/// Adds the State and the value of properties InTime, OutTime to the name in the text box above the vertex.
		/// </summary>
		public void UpdateVertexInfo() => txtName.Text = Name + " (" + GetStateName(State) + ", " + InTime.Print() +", "+OutTime.Print() + ")";
		public override string Informations =>"\n"+ CepheusProjectWpf.Properties.Resources.NLVertexSpace+Name+ CepheusProjectWpf.Properties.Resources.HasState + State + "\nInTime: " + InTime.Print() + "\nOutTime" + OutTime.Print();
		}

	// Dijkstra vertex is same as BFS vertex
	// Bellman-Ford could be BFS vertex also

	public class FloydWarshallVertex : VertexBase<FloydWarshallVertex>
	{

		public int ID { get; set; }
		public override void Initialize()
		{
		}
		public override string Informations => "\n"+ CepheusProjectWpf.Properties.Resources.NLVertexSpace+ Name+ CepheusProjectWpf.Properties.Resources.HasId + ID;
	}
	public class JarnikVertex : VertexBase<JarnikVertex>
	{
		public enum States { Inside, Neighbour, Outside }
		/// <summary>
		/// Vertex current state (Inside/Neighbour/Outside).
		/// </summary>
		public States State { get; set; }
		/// <summary>
		/// Vertex rating.
		/// </summary>
		public int Rating { get; set; }
		/// <summary>
		/// The vertex from which the algorithm came to this vertex.
		/// </summary>
		public JarnikVertex Predecessor { get; set; }
		public override void Initialize()
		{
			State = States.Outside;
			Rating = Int32.MaxValue;
			Predecessor = null;
			UpdateVertexInfo();
		}
		/// <summary>
		/// Adds the State and Rating to the name in the text box above the vertex.
		/// </summary>
		public void UpdateVertexInfo() => txtName.Text = Name + " (" + GetState(State) + ", " + Rating.Print() + ")";
		/// <summary>
		/// Correctly writes the state of the vertex.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		string GetState(States state)
		{
			switch (state)
			{
				case States.Inside:
					return CepheusProjectWpf.Properties.Resources.Inside;
				case States.Neighbour:
					return CepheusProjectWpf.Properties.Resources.Neighbour;
				default:
					return CepheusProjectWpf.Properties.Resources.Outside;
			}
		}
		public override string Informations => "\n"+CepheusProjectWpf.Properties.Resources.NLVertexSpace + Name+ CepheusProjectWpf.Properties.Resources.HasState+ GetState(State) + CepheusProjectWpf.Properties.Resources.WithRating + Rating.Print() + CepheusProjectWpf.Properties.Resources.WithPred + GetPredecessorName(Predecessor);
	}

	public class BoruvkaVertex : VertexBase<BoruvkaVertex>
	{
		/// <summary>
		/// The ID of the context component in which the vertex is located.
		/// </summary>
		public int ComponentID { get; set; }
		/// <summary>
		/// Adds the ComponentID to the name in the text box above the vertex.
		/// </summary>
		public void UpdateVertexInfo() => txtName.Text = Name + " (" + ComponentID + ")";
		public override void Initialize()
		{
			OutEdges.Sort((x, y) => ((Edge<BoruvkaVertex>)x).Length.CompareTo(((Edge<BoruvkaVertex>)y).Length));
			UpdateVertexInfo();
		}
		public override string Informations => "\n"+ CepheusProjectWpf.Properties.Resources.NLVertexSpace+ Name+ CepheusProjectWpf.Properties.Resources.IsInComp+ ComponentID;
	}

	public class GoldbergVertex : VertexBase<GoldbergVertex>
	{
		/// <summary>
		/// Vertex height.
		/// </summary>
		public int Height { get; set; }
		/// <summary>
		/// Vertex surplus (inflow minus outflow).
		/// </summary>
		public int Surplus { get; set; }
		/// <summary>
		/// Recalculates surplus.
		/// </summary>
		/// <returns></returns>
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
		/// <summary>
		/// Adds the Height and Surplus to the name in the text box above the vertex.
		/// </summary>
		public void UpdateHeightInName() => txtName.Text = Name + " (" + Height + ", "+Surplus+")";
		public override void Initialize() 
		{
			Height = 0;
		}
		public override string Informations => "\n"+ CepheusProjectWpf.Properties.Resources.NLVertexSpace + Name+ CepheusProjectWpf.Properties.Resources.HasHeight + Height + CepheusProjectWpf.Properties.Resources.AndSurplus + Surplus;
	}
}
