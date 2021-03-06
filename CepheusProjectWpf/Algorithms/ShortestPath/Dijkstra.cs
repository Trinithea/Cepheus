﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf;
using CepheusProjectWpf.DataStructures;

namespace Cepheus
{
	public class Dijkstra : Algorithm<BfsVertex>
	{
		public override bool IsFlowAlgorithm => false;
		public override bool NeedsOnlyNonNegativeEdgeLenghts => true;
		public override bool DontNeedInitialVertex => false;
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override string Name => CepheusProjectWpf.Properties.Resources.DijkstraAlgo;
		public override string TimeComplexity => CepheusProjectWpf.Properties.Resources.DijkstraTime;
		public override string Description => CepheusProjectWpf.Properties.Resources.DijkstraDesc;
		/// <summary>
		/// The main method of Dijkstra's algorithm. This is where the whole calculation takes place.
		/// </summary>
		/// <returns></returns>
		public async Task Run()
		{
			// We can use vertex class for BFS algortihm, Distance property will be considered as rating.
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.StateRating;
			Graph.InitializeVertices();
			PrintVerticesInitialized(Graph);

			initialVertex.State = States.Open;
			initialVertex.Distance = 0;
			initialVertex.UpdateVertexInfo();
			PrintVertex(initialVertex);
			ColorVertex(initialVertex);

			var openVertices = new MinimumBinaryHeap<int, BfsVertex>(Graph.Vertices.Count);
			openVertices.Insert(initialVertex.Distance, initialVertex);
			while(openVertices.Count > 0)
			{
				var vertex = openVertices.Min(); // vertex with minimal rating
				
				foreach(Edge<BfsVertex> edge in vertex.OutEdges)
				{
					
					if(edge.To.Distance > (vertex.Distance + edge.Length))
					{
						await Delay(delay);
						ColorEdge(edge);
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = States.Open;
						await Delay(delay-250);
						ColorVertex(edge.To);
						edge.To.UpdateVertexInfo();
						PrintVertex(edge.To);
						openVertices.Insert(edge.To.Distance, edge.To);
						edge.To.Predecessor = vertex;
					}
					
				}
				vertex.State = States.Closed;
				vertex.UpdateVertexInfo();
				UncolorVertex(vertex);
				PrintVertex(vertex);
				openVertices.ExtractMin();
				foreach (var edge in vertex.OutEdges)
					UncolorEdge(edge);
			}
			ColorShortestPaths(this);
		}

	}
}
