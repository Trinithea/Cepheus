﻿using CepheusProjectWpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cepheus
{
	public class BFS : Algorithm<BfsVertex>
	{
		public override string Name => "Breadth-first search";
		public override string TimeComplexity => "O(n + m)";

		public override string Description => "Breadth-first search (BFS) is an algorithm for traversing or searching tree or graph data structures. It starts at the tree root (or some arbitrary node of a graph, sometimes referred to as a 'search key'), and explores all of the neighbor nodes at the present depth prior to moving on to the nodes at the next depth level. ";

		public async Task Run ()
		{
			graph.InitializeVertices();
			outputConsole.Text += "\n\nVertices are inicialized.";

			initialVertex.State = States.Open;
			initialVertex.Distance = 0;
			PrintVertex(initialVertex);



			Queue<BfsVertex> queue = new Queue<BfsVertex>();
			queue.Enqueue(initialVertex);

			while(queue.Count > 0)
			{
				BfsVertex vertex = queue.Dequeue();
				ColorVertex(vertex);
				foreach(Edge<BfsVertex> edge in vertex.OutEdges)
				{
					ColorEdge(edge);
					if (edge.To.State ==States.Unvisited)
					{
						edge.To.State = States.Open;
						edge.To.Distance = vertex.Distance + 1;
						edge.To.Predecessor = vertex;
						PrintVertex(edge.To);
						PrintQueued(edge.To);
						queue.Enqueue(edge.To);
						await Task.Delay(delay-250);
						ColorVertex(edge.To);
						await Task.Delay(delay);
					}
				}
				vertex.State = States.Closed;
				PrintVertex(vertex);
				PrintDequeued(vertex);
				UncolorVertex(vertex);
				foreach (var edge in vertex.OutEdges)
					UncolorEdge(edge);
				await Task.Delay(delay);
			}

		}

		void PrintVertex(BfsVertex vertex)
		{
			outputConsole.Text += vertex.Informations;
		}
		void PrintQueued(BfsVertex vertex)
		{
			outputConsole.Text += "\nVertex " + vertex.Name + " has been enqueued.";
		}
		void PrintDequeued(BfsVertex vertex)
		{
			outputConsole.Text += "\nVertex " + vertex.Name + " has been dequeued.";
		}

		//TODO udělat generický před edge, ať to neni tak hnusně nakopírovaný ve FF
		public List<Edge<BfsVertex>> GetPath(Graph<BfsVertex> graph, BfsVertex from, BfsVertex to)
		{
			if (to.Predecessor == null) //'to' is not reachable from 'from'
				return null;
			else
			{
				var currentVertex = to;
				var path = new List<Edge<BfsVertex>>();
				while (currentVertex.Predecessor != null)
				{
					path.Insert(0, (Edge<BfsVertex>)graph.GetEdge(currentVertex.Predecessor, currentVertex));
					currentVertex = currentVertex.Predecessor;
				}
				return path;
			}
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
