using System;
using System.Collections.Generic;
using System.Text;
using CepheusProjectWpf;

namespace Cepheus
{
	public class DFS : Algorithm<DfsVertex>
	{
		public override string Name => "Depth-first search";
		public override string TimeComplexity => "O(n + m)";
		static int Time = 0;
		public void Run(Graph<DfsVertex> graph,DfsVertex initialVertex)
		{
			graph.InitializeVertices();

			Time = 0;
			
			Recursion(initialVertex);
		}
		static void Recursion(DfsVertex vertex)
		{
			vertex.State = States.Open;
			Time++;
			vertex.InTime = Time;
			foreach(Edge<DfsVertex> edge in vertex.OutEdges)
			{
				if (edge.To.State == States.Unvisited)
					Recursion(edge.To);
			}
			vertex.State = States.Closed;
			Time++;
			vertex.OutTime = Time;
		}
		
		public override void Accept(Visitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
