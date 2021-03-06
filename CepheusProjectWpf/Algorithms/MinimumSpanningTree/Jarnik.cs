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
		public override bool IsFlowAlgorithm => false;
		public override bool NeedsOnlyNonNegativeEdgeLenghts => false;
		public override bool DontNeedInitialVertex => false;
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override string Name => CepheusProjectWpf.Properties.Resources.JarnikAlgo;

		public override string TimeComplexity => CepheusProjectWpf.Properties.Resources.JarnikTime;

		public override string Description => CepheusProjectWpf.Properties.Resources.JarnikDesc;
		/// <summary>
		/// Gradually formed minimum spanning tree.
		/// </summary>
		public Tree<JarnikVertex> MinimumSpanningTree { get; private set; }
		/// <summary>
		/// The main method of Jarnik's algorithm. This is where the whole calculation takes place.
		/// </summary>
		/// <returns></returns>
		public async Task Run()
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.StateRating;
			Graph.InitializeVertices();
			PrintVerticesInitialized(Graph);

			//initializing the initial vertex
			MinimumSpanningTree = new Tree<JarnikVertex>();
			initialVertex.State = JarnikVertex.States.Neighbour;
			initialVertex.Rating = 0;
			
			PrintVertex(initialVertex);
			ColorVertex(initialVertex);
			initialVertex.UpdateVertexInfo();

			var neighbours = new MinimumBinaryHeap<int, JarnikVertex>(Graph.Vertices.Count);
			neighbours.Insert(initialVertex.Rating,initialVertex);
			PrintSortedNeighbours(neighbours);

			while(neighbours.Count > 0)
			{
				var vertex = neighbours.ExtractMin(); // neighbour with minimal rating
				vertex.State = JarnikVertex.States.Inside;
				vertex.UpdateVertexInfo();
				if (vertex.Predecessor != null)
				{
					MinimumSpanningTree.Edges.Add(Graph.GetEdge(vertex.Predecessor, vertex));
					MinimumSpanningTree.Vertices.Add(vertex.Predecessor);
					MinimumSpanningTree.Vertices.Add(vertex);
					ColorVertex(vertex.Predecessor);
					ColorVertex(vertex);
					await Delay(delay-350);
					ColorEdge(Graph.GetEdge(vertex.Predecessor, vertex));
					PrintEdgeAddedToMinimumSpanningTree(vertex,vertex.Predecessor);
				}
					

				foreach(Edge<JarnikVertex> edge in vertex.OutEdges)
				{
					if((edge.To.State == JarnikVertex.States.Neighbour || edge.To.State == JarnikVertex.States.Outside)
						&& (edge.To.Rating > edge.Length))
					{
						await Delay(delay);
						ColorEdge(edge);
						edge.To.State = JarnikVertex.States.Neighbour;
						edge.To.Rating = edge.Length;
						edge.To.Predecessor = vertex;
						await Delay(delay-250);
						ColorVertex(edge.To);
						edge.To.UpdateVertexInfo();
						PrintVertex(edge.To);
						if (neighbours.Contains(edge.To)==-1)
						{
							neighbours.Insert(edge.To.Rating, edge.To);
							PrintSortedNeighbours(neighbours);
						}
					}
				}
				if(!MinimumSpanningTree.Vertices.Contains(vertex))
					UncolorVertex(vertex);
				foreach (var edge in vertex.OutEdges)
				{
					if(!MinimumSpanningTree.Edges.Contains(edge))
						UncolorEdge(edge);
				}
				await Delay(delay);
			}
		}
		/// <summary>
		/// Prints sorted neighbors from nearest to farthest.
		/// </summary>
		/// <param name="neighbours"></param>
		void PrintSortedNeighbours(MinimumBinaryHeap<int, JarnikVertex> neighbours)
		{
			outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.SortedNeighbours;
			for (int i = 1; i <= neighbours.Count; i++)
			{
				outputConsole.Text += String.Format("{0} ({1}), ", neighbours.Heap[i].Item2.Name, neighbours.Heap[i].Item1);
			}
		}

		// These methods are here for algorithm unit tests.
		public Tree<JarnikVertex> GetMinimumSpan() => MinimumSpanningTree; 
		public int GetWeightOfMinimumSpan()
		{
			int sum = 0;
			foreach (Edge<JarnikVertex> edge in MinimumSpanningTree.Edges)
				sum += edge.Length;
			return sum;
		} 

	}
}
