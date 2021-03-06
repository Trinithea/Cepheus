﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cepheus
{
	class Edge<T> where T: Vertex
	{
		public T From { get; set; }
		public T To { get; set; }
		public string Name { get; set; }
		public int Length { get; set; }

	}

	class FlowEdge<T> :Edge<T> where T : Vertex
	{
		public int Capacity { get; set; } //TODO only non-negative numbers
		public int Flow { get; set; } //TODO only non-negative numbers
		public int Reserve => Capacity - Flow + OppositeEdge.Flow;
	
		public FlowEdge<T> OppositeEdge { get; set; } // TODO set visibility is discutable //TODO isnt ReverseEdge better?
		public FlowEdge(int capacity)
		{
			Capacity = capacity;
			Flow = 0;
		}
		public override string ToString()
		{
			return String.Format("name: {0}, cap: {1}, flow: {2}, opp: {3}, oppFlow: {4}", Name, Capacity, Flow, OppositeEdge.Name, OppositeEdge.Flow);
		}
	}
}
