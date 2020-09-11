﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cepheus.DataStructures;
using CepheusProjectWpf;
using CepheusProjectWpf.DataStructures;

namespace Cepheus
{
	public class Jarnik : Algorithm<JarnikVertex>
	{
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		//TODO Jarník je na neorientovnaý grafy
		public override string Name => CepheusProjectWpf.Properties.Resources.JarnikAlgo;

		public override string TimeComplexity => CepheusProjectWpf.Properties.Resources.JarnikTime;

		public override string Description => CepheusProjectWpf.Properties.Resources.JarnikDesc;

		Tree<JarnikVertex> MinimumSpanningTree;
		public async Task Run()
		{
			Graph.InitializeVertices();
			PrintVerticesInitialized(Graph);

			MinimumSpanningTree = new Tree<JarnikVertex>();
			initialVertex.State = JarnikVertex.States.Neighbour;
			initialVertex.Rating = 0;
			PrintVertex(initialVertex);
			ColorVertex(initialVertex);

			var neighbours = new BinaryHeap<int, JarnikVertex>(Graph.Vertices.Count);
			neighbours.Insert(initialVertex.Rating,initialVertex);
			PrintSortedNeighbours(neighbours);

			while(neighbours.Count > 0)
			{
				var vertex = neighbours.ExtractMin(); // neighbour with minimal rating
				vertex.State = JarnikVertex.States.Inside;
				
				if (vertex.Predecessor != null)
				{
					MinimumSpanningTree.Edges.Add(Graph.GetEdge(vertex.Predecessor, vertex));
					MinimumSpanningTree.Vertices.Add(vertex.Predecessor);
					MinimumSpanningTree.Vertices.Add(vertex);
					ColorVertex(vertex.Predecessor);
					ColorVertex(vertex);
					await Task.Delay(delay - 350);
					ColorEdge(Graph.GetEdge(vertex.Predecessor, vertex));
					PrintEdgeAddedToMinimumSpanningTree(vertex,vertex.Predecessor);
				}
					

				foreach(Edge<JarnikVertex> edge in vertex.OutEdges)
				{
					if((edge.To.State == JarnikVertex.States.Neighbour || edge.To.State == JarnikVertex.States.Outside)
						&& (edge.To.Rating == Int32.MaxValue || edge.To.Rating > edge.Length))
					{
						ColorEdge(edge);
						edge.To.State = JarnikVertex.States.Neighbour;
						edge.To.Rating = edge.Length;
						edge.To.Predecessor = vertex;
						await Task.Delay(delay - 250);
						ColorVertex(edge.To);
						PrintVertex(edge.To);
						if (!neighbours.ContainsValue(edge.To))
						{
							neighbours.Insert(edge.To.Rating, edge.To);
							PrintSortedNeighbours(neighbours);
						}
						await Task.Delay(delay);
					}
				}
				if(!MinimumSpanningTree.Vertices.Contains(vertex))
					UncolorVertex(vertex);
				foreach (var edge in vertex.OutEdges)
				{
					if(!MinimumSpanningTree.Edges.Contains(edge))
						UncolorEdge(edge);
				}
				await Task.Delay(delay);
			}
		}
		void PrintSortedNeighbours(BinaryHeap<int, JarnikVertex> neighbours)
		{
			outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.SortedNeighbours;
			for (int i = 1; i <= neighbours.Count; i++)
			{
				outputConsole.Text += String.Format("{0} ({1}), ", neighbours.Heap[i].Item2.Name, neighbours.Heap[i].Item1);
			}
		}

		public Tree<JarnikVertex> GetMinimumSpan() => MinimumSpanningTree; //TODO can't be null
		public int GetWeightOfMinimumSpan()
		{
			int sum = 0;
			foreach (Edge<JarnikVertex> edge in MinimumSpanningTree.Edges)
				sum += edge.Length;
			return sum;
		} //TODO do this in datastructure

	}
}
