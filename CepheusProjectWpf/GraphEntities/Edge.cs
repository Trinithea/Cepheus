using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cepheus
{
	public class Edge<T> where T: Vertex
	{
		public T From { get; set; }
		public T To { get; set; }
		public string Name { get; set; }
		public int Length { get; set; }

	}

	public class FlowEdge<T> :Edge<T> where T : Vertex
	{
		public int Capacity { get; set; } 
		public int Flow { get; set; } 
		public int Reserve => Capacity - Flow + OppositeEdge.Flow;
		public TextBox currentFlowInfo { get; set; }
		public void UpdateCurrentFlowInfo()
		{
			if(currentFlowInfo!=null)
				currentFlowInfo.Text = Flow + "/" + Capacity;
		}


		public FlowEdge<T> OppositeEdge { get; set; } 
		public FlowEdge(int capacity, TextBox txtLength)
		{
			Capacity = capacity;
			Flow = 0;
			currentFlowInfo = txtLength;
		}
		
	}
}
