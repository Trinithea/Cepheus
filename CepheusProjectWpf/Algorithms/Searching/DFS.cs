﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf;

namespace Cepheus
{
	public class DFS : Algorithm<DfsVertex>
	{
		public override string Name => "Depth-first search";
		public override string TimeComplexity => "O(n + m)";

		public override string Description => "Depth-first search (DFS) is an algorithm for traversing or searching tree or graph data structures. The algorithm starts at the root node (selecting some arbitrary node as the root node in the case of a graph) and explores as far as possible along each branch before backtracking. ";

		static int Time = 0;
		public async Task Run()
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
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
	}
}
