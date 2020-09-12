﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf;

namespace Cepheus
{
	public class BellmanFord : Algorithm<BfsVertex>
	{
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override string Name => CepheusProjectWpf.Properties.Resources.BFAlgo;
		public override string TimeComplexity => CepheusProjectWpf.Properties.Resources.BFTime;

		public override string Description => CepheusProjectWpf.Properties.Resources.BFDesc;

		public async Task Run()
		{
			PrintInfoStateDistance();
			Graph.InitializeVertices();
			PrintVerticesInitialized(Graph);

			initialVertex.State = States.Open;

			initialVertex.Distance = 0;
			initialVertex.UpdateVertexInfo();
			PrintVertex(initialVertex);
			Queue<BfsVertex> openVertices = new Queue<BfsVertex>();

			openVertices.Enqueue(initialVertex);
			PrintQueued(initialVertex);
			while (openVertices.Count > 0)
			{
				var vertex = openVertices.Dequeue(); // some open vertex
				PrintDequeued(vertex);
				foreach (Edge<BfsVertex> edge in vertex.OutEdges)
				{
					
					if (edge.To.Distance == Int32.MaxValue || edge.To.Distance > (vertex.Distance + edge.Length))
					{
						ColorEdge(edge);
						edge.To.Distance = vertex.Distance + edge.Length;
						edge.To.State = States.Open;
						openVertices.Enqueue(edge.To);
						await Task.Delay(delay - 250);
						ColorVertex(edge.To);
						edge.To.UpdateVertexInfo();
						PrintVertex(edge.To);
						PrintQueued(edge.To);
						edge.To.Predecessor = vertex;
					}
					await Task.Delay(delay);
				}
				vertex.State = States.Closed;
				vertex.UpdateVertexInfo();
				UncolorVertex(vertex);
				PrintVertex(vertex);
				foreach (var edge in vertex.OutEdges)
					UncolorEdge(edge);
				await Task.Delay(delay);
			}
			ColorShortestPaths();
		}

		void ColorShortestPaths()
		{
			foreach (var vertex in Graph.Vertices.Values)
			{
				if (vertex.Predecessor != null)
					vertex.ColorEdgeWithPredecessor(this);
			}
		}
	}
}
