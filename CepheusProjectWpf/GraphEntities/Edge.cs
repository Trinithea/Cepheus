using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cepheus
{
	public class Edge<TVertex> where TVertex: Vertex
	{
		/// <summary>
		/// The vertex from which the edge leads.
		/// </summary>
		public TVertex From { get; set; }
		/// <summary>
		/// The vertex to which the edge leads.
		/// </summary>
		public TVertex To { get; set; }
		/// <summary>
		/// A unique edge name composed of a unique vertex 'From' ID, the arrow "->" and a unique vertex 'To' ID.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Edge length.
		/// </summary>
		public int Length { get; set; }

	}

	public class FlowEdge<T> :Edge<T> where T : Vertex
	{
		/// <summary>
		/// The capacity of an edge is taken as its length.
		/// </summary>
		public int Capacity { get; set; }
		/// <summary>
		/// The current flow that flows along the edge.
		/// </summary>
		public int Flow { get; set; }
		/// <summary>
		/// Current edge reserve.
		/// </summary>
		public int Reserve => Capacity - Flow + OppositeEdge.Flow;
		/// <summary>
		/// A TextBox where normally only the edge length is used is also used to display the current flow.
		/// </summary>
		public TextBox currentFlowInfo { get; set; }
		/// <summary>
		/// Overwrites the contents of the textbox above the edge to Flow / Capacity.
		/// </summary>
		public void UpdateCurrentFlowInfo()
		{
			if(currentFlowInfo!=null)
				currentFlowInfo.Text = Flow + "/" + Capacity;
		}
		/// <summary>
		/// The edge leading from the vertex 'To' to the vertex 'From'
		/// </summary>
		public FlowEdge<T> OppositeEdge { get; set; } 
		public FlowEdge(int capacity, TextBox txtLength)
		{
			Capacity = capacity;
			Flow = 0;
			currentFlowInfo = txtLength;
			UpdateCurrentFlowInfo();
		}
		
	}
}
