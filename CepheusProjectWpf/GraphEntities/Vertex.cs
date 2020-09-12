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
		public static string Print(this int i)
		{
			if (i == Int32.MaxValue)
				return CepheusProjectWpf.Properties.Resources.Infinity;
			else
				return i.ToString();
		}
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
		public List<Edge<Vertex>> OutEdges = new List<Edge<Vertex>>();
		public List<Edge<Vertex>> InEdges = new List<Edge<Vertex>>();
		public TextBox txtName;
		public abstract string Informations { get; }
		public int UniqueId { get; set; }
		public string Name { get; set; }
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
		public new List<Edge<T>> OutEdges = new List<Edge<T>>();
		public new List<Edge<T>> InEdges = new List<Edge<T>>();
		public abstract void Initialize();
		
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
		public BfsVertex Predecessor { get; set; }
		public void UpdateVertexInfo() => txtName.Text = Name + " (" + GetStateName(State) + ", " + Distance.Print() + ")";
		public States State { get; set; }
		public void ColorEdgeWithPredecessor(Algorithm<BfsVertex> algorithm) => algorithm.ColorEdge(algorithm.Graph.GetEdge(Predecessor.UniqueId + "->" + UniqueId));
		public int Distance { get; set; }

		public override void Initialize()
		{
			State = States.Unvisited;
			Distance = Int32.MaxValue;
			Predecessor = null;
			UpdateVertexInfo();
		}
		public override string Informations => "\n"+ CepheusProjectWpf.Properties.Resources.NLVertexSpace + Name + CepheusProjectWpf.Properties.Resources.HasState + GetStateName(State) + CepheusProjectWpf.Properties.Resources.IsInDistance + Distance.Print() + CepheusProjectWpf.Properties.Resources.FromInitVPred + Predecessor?.Name;
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
			UpdateVertexInfo();
		}
		public void UpdateVertexInfo() => txtName.Text = Name + " (" + GetStateName(State) + ", " + InTime.Print() +", "+OutTime.Print() + ")";
		public override string Informations =>"\n"+ CepheusProjectWpf.Properties.Resources.NLVertexSpace+Name+ CepheusProjectWpf.Properties.Resources.HasState + State + "\nInTime: " + InTime.Print() + "\nOutTime" + OutTime.Print();
		}//TODO is intime/outime really inifinity by default?

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
		public States State { get; set; }
		public int Rating { get; set; }
		public JarnikVertex Predecessor { get; set; }
		public override void Initialize()
		{
			State = States.Outside;
			Rating = Int32.MaxValue;
			Predecessor = null;
			UpdateVertexInfo();
		}
		public void UpdateVertexInfo() => txtName.Text = Name + " (" + GetState(State) + ", " + Rating.Print() + ")";
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
		public override string Informations => "\n"+CepheusProjectWpf.Properties.Resources.NLVertexSpace + Name+ CepheusProjectWpf.Properties.Resources.HasState+ GetState(State) + CepheusProjectWpf.Properties.Resources.WithRating + Rating.Print() + CepheusProjectWpf.Properties.Resources.WithPred + Predecessor?.Name;
	}

	public class BoruvkaVertex : VertexBase<BoruvkaVertex>
	{
		public int ComponentID { get; set; }
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
		public void UpdateHeightInName() => txtName.Text = Name + " (" + Height + ")";
		public override void Initialize() 
		{
			Height = 0;
		}
		public override string Informations => "\n"+ CepheusProjectWpf.Properties.Resources.NLVertexSpace + Name+ CepheusProjectWpf.Properties.Resources.HasHeight + Height + CepheusProjectWpf.Properties.Resources.AndSurplus + Surplus;
	}
}
