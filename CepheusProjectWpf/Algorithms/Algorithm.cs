﻿using System;
using System.Collections.Generic;
using System.Text;
using CepheusProjectWpf;
namespace Cepheus
{
	public abstract class Algorithm 
	{
		public abstract string Name { get; }
		public abstract string TimeComplexity { get; }
		public abstract void Accept(VisitorGraphCreator visitor);
	}
	public abstract class Algorithm<TVertex> : Algorithm where TVertex : VertexBase<TVertex>, new()
	{
		public Graph<TVertex> graph;
		public TVertex initialVertex; //TODO kliděn by mohlo být i uvnitř grafu
		public void CreateGraph()
		{
			graph = new Graph<TVertex>();
			foreach (var vertex in MainWindow.Vertices.Keys)
			{
				graph.AddVertex(vertex.UniqueId);
				graph.UltimateVertices.Add(graph.GetVertex(vertex.UniqueId), vertex);
			}
			foreach (var edge in MainWindow.Edges.Keys)
			{
				graph.AddEdge(graph.GetVertex(edge.FromVertex.UniqueId), graph.GetVertex(edge.ToVertex.UniqueId), edge.Name, edge.Length);
				graph.UltimateEdges.Add(graph.GetEdge(edge.Name), edge);
			}
			initialVertex = graph.GetVertex((int)MainWindow.initialVertex);
		}
		protected void ColorEdge(Edge<TVertex> edge)
		{

		}
	}
	public abstract class FlowAlgorithm<TVertex> : Algorithm where TVertex : VertexBase<TVertex>, new()
	{
		public FlowNetwork<TVertex> graph;
		public void CreateGraph()
		{
			graph = new FlowNetwork<TVertex>();
			foreach (var vertex in MainWindow.Vertices.Keys)
			{
				graph.AddVertex(vertex.UniqueId);

			}
			foreach (var edge in MainWindow.Edges.Keys)
			{
				graph.AddEdge(graph.GetVertex(edge.FromVertex.UniqueId), graph.GetVertex(edge.ToVertex.UniqueId), edge.Name, edge.Length);
			}
			graph.Source = graph.Vertices[(int)MainWindow.initialVertex];
			graph.Sink = graph.Vertices[(int)MainWindow.sinkVertex];
		}
	}
}
