using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class DFS : IAlgorithm
	{
		public string Name => "Depth-first search";
		int Time = 0;
		public void Run(Graph<DfsVertex> graph,DfsVertex initialVertex)
		{
			//Initialized vertices and StepCount by default
			
			Recursion(initialVertex);
		}
		void Recursion(DfsVertex vertex)
		{
			vertex.State = IStateVertex.States.Open;
			Time++;
			vertex.InTime = Time;
		}
	}
}
